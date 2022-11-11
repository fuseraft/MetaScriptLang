namespace Metal.Core.Parsing
{
    using System.Text;

    public class Tokenizer
    {
        public static TokenContainer Tokenize(string input)
        {
            return new TokenContainer(TokenizeInternal(input));
        }

        private static IList<string> TokenizeInternal(string input)
        {
            StringBuilder sb = new StringBuilder();
            IList<string> split = new List<string>();
            bool buildingLiteral = false;
            bool buildingComment = false;
            bool newLine = true;

            input = input.Replace("\r\n", "^~").Replace("\n", "^~");

            for (int si = 0; si < input.Length; si++)
            {
                char c = input[si];
                char n = si < input.Length - 1 ? input[si + 1] : '\0';
                char l = si > 0 ? input[si - 1] : '\0';

                // Not every ^ is handled...
                if (c == '^' && n == '~')
                {
                    AddToken(sb, split);
                    newLine = true;
                    si++;
                    continue;
                }
                else if (l == '\\' && c == '"')
                {
                    sb.Append(c);
                    continue;
                }
                else if (l == '^' && c == '~')
                {
                    continue;
                }

                if (newLine && c == '#')
                {
                    buildingComment = true;
                    newLine = false;
                }
                else if (newLine && buildingComment)
                {
                    buildingComment = false;
                }

                if (buildingComment)
                {
                    continue;
                }

                newLine = false;

                switch (c)
                {
                    case ';':
                        if (AddToken(sb, split))
                        {
                            continue;
                        }
                        break;

                    case '(':
                    case '[':
                        if (buildingLiteral)
                        {
                            sb.Append(c);
                            continue;
                        }

                        if (AddToken(sb, split))
                        {
                            continue;
                        }

                        AddToken($"{c}", split);

                        break;

                    case ')':
                    case ']':
                        if (buildingLiteral)
                        {
                            sb.Append(c);
                            continue;
                        }

                        AddToken(sb, split);
                        AddToken($"{c}", split);

                        break;

                    case ' ':
                    case '\t':
                        if (buildingLiteral)
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            if (AddToken(sb, split))
                            {
                                continue;
                            }
                        }

                        break;

                    case '"':
                        buildingLiteral = !buildingLiteral;
                        sb.Append(c);
                        break;

                    default:
                        sb.Append(c);
                        break;
                }
            }

            return split;
        }

        static void AddToken(string s, IList<string> split)
        {
            if (!string.IsNullOrEmpty(s))
            {
                split.Add(s);
            }
        }

        static bool AddToken(StringBuilder sb, IList<string> split)
        {
            bool skip = sb.Length == 0;

            if (sb.Length != 0)
            {
                split.Add(sb.ToString());
                sb.Clear();
            }

            return skip;
        }
    }
}
