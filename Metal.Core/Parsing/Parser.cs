
namespace Metal.Core.Parsing
{
    using Metal.Core.Engine;
    using Metal.Core.Parsing.Keywords;
    using Metal.Core.Typing.Enums;
    using System.Runtime.CompilerServices;

    public class Parser
    {
        public class ParseContainer
        {
            private int currentIndex = 0;
            private int amountMoved = 0;

            public ParseContainer(IList<string> strings)
            {
                Lines.AddRange(strings);
            }

            public int Length => Lines.Count;

            public int CurrentIndex => currentIndex;

            public bool EndOfContainer => CurrentIndex + 1 >= Length;

            public int AmountMoved => amountMoved;

            public string Current()
            {
                return Lines[currentIndex];
            }

            public string Next()
            {
                if (EndOfContainer)
                {
                    return string.Empty;
                }

                currentIndex++;
                amountMoved++;
                return Lines[currentIndex];
            }

            public string PeekPrevious()
            {
                if (currentIndex == 0)
                {
                    return string.Empty;
                }

                return Lines[currentIndex - 1];
            }

            public string PeekNext()
            {
                if (EndOfContainer)
                {
                    return string.Empty;
                }

                return Lines[currentIndex + 1];
            }

            public int Skip(int count)
            {
                var newIndex = CurrentIndex + count;

                if (newIndex >= Length)
                {
                    return -1;
                }

                currentIndex = newIndex;
                return count;
            }

            public List<string> Lines { get; } = new List<string>();
        }

        public static ParserResult Parse(string input)
        {
            IList<string> strings = Tokenizer.Tokenize(input);
            ParserResult result = new ();

            ParseContainer container = new (strings);
            while (!container.EndOfContainer)
            {
                TypeDefinition currentType = State.CurrentTypeDefinition;
                var current = container.Current();
                var prev = container.PeekPrevious();
                var next = container.PeekNext();

                if (KeywordLookup.IsTerminatorKeyword(current))
                {
                    ParseTerminatorToken(current);
                }
                else if (KeywordLookup.IsKeyword(current))
                {
                    if (KeywordLookup.IsBlockKeyword(current))
                    {
                        ParseBlockToken(container, current, next);
                        continue;
                    }
                    else if (KeywordLookup.IsGrouper(current))
                    {
                        ParseGrouperToken(container, current, prev, next);
                    }
                }
                else
                {
                    if (State.CurrentTypeDefinition == TypeDefinition.Method)
                    {
                        State.AddInstruction(current);
                    }
                }

                container.Next();
            }

            State.PrintState();

            return result;
        }

        private static ParserResult ParseTerminatorToken(string terminatorToken)
        {
            if (terminatorToken == "end")
            {
                State.EndBlock();
            }

            return new ParserResult
            {
                Success = true,
            };
        }

        private static ParserResult ParseGrouperToken(ParseContainer container, string current, string prev, string next)
        {
            // Determine the closing grouper token.
            string search = current == "(" ? ")" : "]";

            // Find all inner strings.
            IList<string> innerStrings = new List<string>();
            while ((next = container.Next()) != search)
            {
                if (container.EndOfContainer && string.IsNullOrEmpty(next))
                {
                    break;
                }

                innerStrings.Add(next);
            }

            // If inner strings were found, parse them.
            if (innerStrings.Count > 0)
            {
                ParseContainer innerContainer = new(innerStrings);

                if (State.CurrentTypeDefinition == TypeDefinition.Method)
                {
                    ParseMethodParameterGroup(innerContainer, current, prev);
                }

                container.Skip(innerContainer.AmountMoved - 2);
            }

            return new ParserResult
            {
                Success = true,
            };
        }

        private static ParserResult ParseMethodParameterGroup(ParseContainer container, string current, string prev)
        {
            // If the previous token is the method we are defining, then this is the method parameter list.
            if (State.CurrentOwnerName == prev && current == "(")
            {
                while (!container.EndOfContainer)
                {
                    var innerCurrent = container.Current();
                    var innerNext = container.Next();

                    // Handle optional parameter.
                    if (innerNext == "=")
                    {
                        var defaultValue = container.Next();
                        State.AddParameter(innerCurrent, defaultValue);
                    }
                    // Handle next parameter.
                    else if (innerNext == ",")
                    {
                        innerNext = container.Next();
                    }
                    else
                    {
                        State.AddParameter(innerCurrent);
                    }
                }
            }

            return new ParserResult 
            {
                Success = true
            };
        }

        private static ParserResult ParseBlockToken(ParseContainer container, string blockToken, string nextToken)
        {
            switch (blockToken)
            {
                case "module":
                    State.CreateModule(nextToken);
                    break;
                case "class":
                    State.CreateClass(nextToken);
                    break;
                case "method":
                    State.CreateMethod(nextToken);
                    break;
            }

            container.Skip(2);

            // Handle inheritance.
            if (container.Current() == "<")
            {
                if (State.CurrentTypeDefinition != TypeDefinition.Class)
                {
                    throw new Exception($"Syntax error. Inheritance operator found while defining type: {State.CurrentTypeDefinition}");
                }

                State.InheritFrom(container.Next());
            }

            return new ParserResult
            {
                Success = true,
            };
        }
    }
}
