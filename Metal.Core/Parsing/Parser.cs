
namespace Metal.Core.Parsing
{
    using Metal.Core.Engine;
    using Metal.Core.Parsing.Keywords;
    using Metal.Core.Typing.Enums;

    public class Parser
    {
        public static ParserResult ParseScript(string script)
        {
            try
            {
                var metal = System.IO.File.ReadAllText(script);
                return Parse(metal);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            return new ParserResult
            {
                Success = true
            };
        }

        public static ParserResult Parse(string input)
        {
            ParserResult result = new ();
            
            try
            {
                TokenContainer container = Tokenizer.Tokenize(input);
                result = ParseTokenContainer(container);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                result.Success = false;
            }

            State.PrintState();

            return result;
        }

        private static ParserResult ParseTokenContainer(TokenContainer container)
        {
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

            return new ParserResult
            {
                Success = true
            };
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

        private static ParserResult ParseGrouperToken(TokenContainer container, string current, string prev, string next)
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
                TokenContainer innerContainer = new(innerStrings);

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

        private static ParserResult ParseMethodParameterGroup(TokenContainer container, string current, string prev)
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

        private static ParserResult ParseBlockToken(TokenContainer container, string blockToken, string nextToken)
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
