
namespace Metal.Core.Parsing
{
    using Metal.Core.Engine;
    using Metal.Core.Parsing.Keywords;
    using Metal.Core.Typing.Enums;

    public class Parser
    {
        public static ParserResult Parse(string input)
        {
            IList<string> strings = Tokenizer.Tokenize(input);
            ParserResult result = new ();

            for (int si = 0; si < strings.Count; si++)
            {
                string s = strings[si];
                string n = si < strings.Count - 1 ? strings[si + 1] : string.Empty;
                string l = si > 0 ? strings[si - 1] : string.Empty;
                TypeDefinition currentType = State.CurrentTypeDefinition;

                if (KeywordLookup.IsTerminatorKeyword(s))
                {
                    result = HandleTerminatorToken(s);
                }
                else if (KeywordLookup.IsKeyword(s))
                {
                    if (KeywordLookup.IsBlockKeyword(s))
                    {
                        result = HandleBlockToken(s, n);
                        si++;
                    }
                    else if (KeywordLookup.IsGrouper(s))
                    {
                        var closeIndex = GetClosingGrouperToken(strings, si, s);
                        result = HandleGrouperToken(strings, si, closeIndex, s, l);

                        if (closeIndex != -1)
                        {
                            si += (closeIndex - si);
                        }
                    }
                }
                else 
                {
                    if (currentType == TypeDefinition.Method)
                    {
                        State.AddInstruction(s);
                    }
                }
            }

            return result;
        }

        private static ParserResult HandleTerminatorToken(string terminatorToken)
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

        private static int GetClosingGrouperToken(IList<string> strings, int si, string grouperToken)
        {
            string search = grouperToken == "(" ? ")" : "]";

            for (int i = si; i < strings.Count; i++)
            {
                if (strings[i] == search)
                {
                    return i;
                }
            }

            return -1;
        }

        private static ParserResult HandleGrouperToken(IList<string> strings, int si, int closeIndex, string grouperToken, string lastToken)
        {
            IList<string> innerStrings = new List<string>();

            switch (grouperToken)
            {
                case "(":
                case "[":
                    for (int i = si; i <= closeIndex; i++)
                    {
                        innerStrings.Add(strings[i]);
                    }
                    break;

                case ")":
                case "]":
                    break;
            }

            if (innerStrings.Count > 0)
            {
                if (State.CurrentTypeDefinition == TypeDefinition.Method)
                {
                    if (State.CurrentOwnerName == lastToken)
                    {
                        for (int i = 0; i < innerStrings.Count; i++)
                        {
                            string innerString = innerStrings[i];
                            string nextInnerString = i < innerStrings.Count - 1 ? innerStrings[i + 1] : string.Empty;

                            if (nextInnerString == "=")
                            {
                                string defaultValue = i + 1 < innerStrings.Count - 1 ? innerStrings[i + 2] : string.Empty;
                                State.AddParameter(innerString, defaultValue);
                            }
                        }
                    }
                    else
                    {
                        foreach (var innerString in innerStrings)
                        {
                            State.AddInstruction(innerString);
                        }
                    }
                }
            }

            return new ParserResult
            {
                Success = true,
            };
        }

        private static ParserResult HandleBlockToken(string blockToken, string nextToken)
        {
            switch (blockToken)
            {
                case "module":
                    State.CreateModule(nextToken);
                    break;
                case "class":
                    State.CreateObject(nextToken);
                    break;
                case "method":
                    State.CreateMethod(nextToken);
                    break;
            }

            return new ParserResult
            {
                Success = true,
            };
        }
    }
}
