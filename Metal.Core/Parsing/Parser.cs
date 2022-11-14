
namespace Metal.Core.Parsing
{
    using Metal.Core.Engine;
    using Metal.Core.Parsing.Keywords;
    using Metal.Core.Typing.Enums;
    using System.Text;

    public class Parser
    {
        #region REPL
        public static bool ReplEnabled = false;
        private static int ReplWait = 0;
        private static StringBuilder ReplText = new ();
        #endregion

        public static ParserResult ParseScript(string script)
        {
            var result = ParserResult.SuccessResult;

            try
            {
                var metal = System.IO.File.ReadAllText(script);
                return Parse(metal);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                result.Success = false;
            }

            return result;
        }

        public static ParserResult ReplParse(string input)
        {
            var result = ParserResult.SuccessResult;

            try
            {
                if (string.IsNullOrEmpty(input))
                {
                    return result;
                }

                TokenContainer container = Tokenizer.Tokenize(input);

                if (container.Tokens.Any(t => KeywordLookup.IsTerminatorKeyword(t)))
                {
                    --ReplWait;
                }
                else if (container.Tokens.Any(t => KeywordLookup.IsBlockKeyword(t))) 
                {
                    ++ReplWait;
                }

                ReplText.AppendLine(input);

                if (ReplWait == 0)
                {
                    ReplText.Append("exec_repl!");
                    Parse(ReplText.ToString());
                    ReplText.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                result.Success = false;
            }

            return result;
        }

        public static ParserResult Parse(string input)
        {
            var result = ParserResult.SuccessResult;

            try
            {
                if (string.IsNullOrEmpty(input))
                {
                    return result;
                }

                TokenContainer container = Tokenizer.Tokenize(input);
                result = ParseTokenContainer(container);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                result.Success = false;
            }

            StatePrinter.PrintState();

            return result;
        }

        private static ParserResult ParseTokenContainer(TokenContainer container)
        {
            var result = ParserResult.SuccessResult;

            while (!container.EndOfContainer)
            {
                var current = container.Current();
                var prev = container.PeekPrevious();
                var next = container.PeekNext();

                if (KeywordLookup.IsBangWord(current))
                {
                    result = ParseBangWord(current);
                }

                if (KeywordLookup.IsTerminatorKeyword(current))
                {
                    result = ParseTerminatorToken(current);
                }

                if (KeywordLookup.IsKeyword(current))
                {
                    if (KeywordLookup.IsBlockKeyword(current))
                    {
                        result = ParseBlockToken(container, current, next);
                        continue;
                    }
                    else if (KeywordLookup.IsGrouper(current))
                    {
                        result = ParseGrouperToken(container, current, prev, next);
                    }
                }
                else
                {
                    if (State.CurrentTypeDefinition == TypeDefinition.Method)
                    {
                        StateManager.AddInstruction(current);
                    }
                }

                container.Next();
            }

            return result;
        }

        private static ParserResult ParseBangWord(string bangWord)
        {
            // TODO.
            return ParserResult.SuccessResult;
        }

        private static ParserResult ParseTerminatorToken(string terminatorToken)
        {
            if (terminatorToken == "end")
            {
                State.EndBlock();
            }

            return ParserResult.SuccessResult;
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

            return ParserResult.SuccessResult;
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
                        StateManager.AddParameter(innerCurrent, defaultValue);
                    }
                    // Handle next parameter.
                    else if (innerNext == ",")
                    {
                        innerNext = container.Next();
                    }
                    else
                    {
                        StateManager.AddParameter(innerCurrent);
                    }
                }
            }

            return ParserResult.SuccessResult;
        }

        private static ParserResult ParseBlockToken(TokenContainer container, string blockToken, string nextToken)
        {
            switch (blockToken)
            {
                case "module":
                    StateManager.CreateModule(nextToken);
                    break;
                case "class":
                    StateManager.CreateClass(nextToken);
                    break;
                case "method":
                    StateManager.CreateMethod(nextToken);
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

                StateManager.InheritFrom(container.Next());
            }

            return ParserResult.SuccessResult;
        }
    }
}
