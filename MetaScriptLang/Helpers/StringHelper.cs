namespace MetaScriptLang.Helpers
{
    public class StringHelper
    {
        public static bool IsNumeric(string s)
        {
            int l = s.Length;
            bool decimalPointFound = false, signFound = false;

            if (s.Trim().Length == 0) return false;

            for (int i = 0; i < l; i++)
            {
                if (!Char.IsDigit(s[i]))
                {
                    switch (s[i])
                    {
                        case '.':
                            if (i == 0)
                            {
                                return false;
                            }
                            else if (decimalPointFound)
                            {
                                return false;
                            }
                            else
                            {
                                decimalPointFound = true;
                            }
                            break;

                        case '-':
                            if (i != 0)
                            {
                                return false;
                            }
                            else if (signFound)
                            {
                                return false;
                            }
                            else
                            {
                                signFound = true;
                            }
                            break;

                        default:
                            return false;
                    }
                }
            }

            return true;
        }

        #region String Manipulation
        public static bool IsAlphabetical(string s)
        {
            int l = s.Length;

            for (int i = 0; i < l; i++)
            {
                if (!char.IsLetter(s[i]))
                    return false;
            }

            return true;
        }

        public static bool StringEndsWith(string s, string end)
        {
            return s.EndsWith(end);
        }

        public static bool StringStartsWith(string s1, string s2)
        {
            if (s1.Length >= s2.Length)
            {
                int s2l = s2.Length;

                for (int i = 0; i < s2l; i++)
                    if (s1[i] != s2[i])
                        return false;

                return true;
            }

            return false;
        }

        public static bool ZeroDots(string s)
        {
            bool none = true;
            int l = s.Length;

            for (int i = 0; i < l; i++)
            {
                if (s[i] == '.')
                {
                    none = false;
                }
            }

            return (none);
        }

        public static bool ZeroNumbers(string s)
        {
            int start = '0', stop = '9';

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] >= start && s[i] <= stop)
                {
                    return false;
                }
            }

            return true;
        }

        public static string ReplaceString(string original, string target, string replacement)
        {
            return original.Replace(target, replacement);
        }

        public static System.Collections.Generic.List<string> SplitString(string s, char delim)
        {
            return new System.Collections.Generic.List<string>(s.Split(delim));
        }

        public static string LTrim(string s)
        {
            return s.TrimStart();
        }

        public static string RTrim(string s)
        {
            return s.TrimEnd();
        }

        public static string GetInnerString(string s, int left, int right)
        {
            string inner = string.Empty;
            int len = s.Length;
            if (left > len || right > len)
            {
                // overflow error 
            }
            else if (left > right)
            {
                // invalid operation
            }
            else if (left == right)
            {
                inner.Append(s[left]);
            }
            else
            {
                for (int i = left; i <= right; i++)
                {
                    inner.Append(s[i]);
                }
            }

            return inner;
        }

        public static string SubtractChars(string s1, string s2)
        {
            string r = string.Empty;
            int len = s1.Length;

            for (int i = 0; i < len; i++)
            {
                if (s1[i] != s2[0])
                    r.Append(s1[i]);
            }

            return (r);
        }

        public static string SubtractString(string s1, string s2)
        {
            return s1.Replace(s2, "");
        }
        #endregion

        #region Strings
        public static bool IsArgument(string s, string si)
        {
            return (s == ("-" + si) || s == ("--" + si) || s == ("/" + si));
        }

        public static bool IsScript(string path)
        {
            return StringEndsWith(path, ".ns");
        }

        public static bool IsUppercase(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (!Char.IsUpper(c))
                    return false;
            }

            return true;
        }

        public static bool IsUppercaseConstant(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (!Char.IsUpper(c))
                {
                    if (c != '_')
                        return false;
                }
            }

            return true;
        }

        public static bool IsLowercase(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (!char.IsLower(c))
                    return false;
            }

            return true;
        }

        public static string ToUppercase(string input)
        {
            return input.ToUpper();
        }

        public static string ToLowercase(string input)
        {
            return input.ToLower();
        }

        public static bool ContainsString(string s1, string s2)
        {
            return s1.Contains(s2);
        }

        public static bool ContainsParameters(string s)
        {
            int sl = s.Length;

            for (int i = 0; i < sl; i++)
            {
                if (s[i] == '(')
                    return true;
            }

            return false;
        }

        public static bool ContainsBrackets(string s)
        {
            int sl = s.Length;

            for (int i = 0; i < sl; i++)
            {
                if (s[i] == '[')
                    return true;
            }
            return false;
        }

        public static System.Collections.Generic.List<string> GetParameters(string s)
        {
            System.Collections.Generic.List<string> parameters = new();

            int sl = s.Length;
            bool start_push = false;

            System.Text.StringBuilder new_name = new();

            for (int i = 0; i < sl; i++)
            {
                if (start_push)
                {
                    if (s[i] == ',')
                    {
                        parameters.Add(new_name.ToString());
                        new_name.Clear();
                    }
                    else if (s[i] == ')')
                        start_push = false;
                    else
                        new_name.Append(s[i]);
                }
                else
                {
                    if (s[i] == '(')
                        start_push = true;
                }
            }

            parameters.Add(new_name.ToString());

            return parameters;
        }

        public static System.Collections.Generic.List<string> GetBracketRange(string s)
        {
            System.Collections.Generic.List<string> parameters = new();

            int sl = s.Length;
            bool start_push = false, almost_push = false;

            System.Text.StringBuilder new_name = new();

            for (int i = 0; i < sl; i++)
            {
                if (start_push)
                {
                    if (s[i] == '.')
                    {
                        if (!almost_push)
                            almost_push = true;
                        else
                        {
                            parameters.Add(new_name.ToString());
                            new_name.Clear();
                        }
                    }
                    else if (s[i] == ']')
                        start_push = false;
                    else if (s[i] == ' ')
                    {
                        //doNothing();
                    }
                    else
                        new_name.Append(s[i]);
                }
                else if (s[i] == '[')
                    start_push = true;
            }

            parameters.Add(new_name.ToString());

            return parameters;
        }

        public static System.Collections.Generic.List<string> GetRange(string s)
        {
            System.Collections.Generic.List<string> parameters = new();

            int sl = s.Length;
            bool start_push = false, almost_push = false;

            System.Text.StringBuilder new_name = new();

            for (int i = 0; i < sl; i++)
            {
                if (start_push)
                {
                    if (s[i] == '.')
                    {
                        if (!almost_push)
                            almost_push = true;
                        else
                        {
                            parameters.Add(new_name.ToString());
                            new_name.Clear();
                        }
                    }
                    else if (s[i] == ')')
                        start_push = false;
                    else if (s[i] == ' ')
                    {
                        //doNothing();
                    }
                    else
                        new_name.Append(s[i]);
                }
                else if (s[i] == '(')
                    start_push = true;
            }

            parameters.Add(new_name.ToString());

            return (parameters);
        }

        public static string BeforeParameters(string s)
        {
            int sl = s.Length;
            bool stop_push = false;
            string new_str = string.Empty;

            for (int i = 0; i < sl; i++)
            {
                if (s[i] == '(')
                    stop_push = true;

                if (!stop_push)
                    new_str.Append(s[i]);
            }

            return new_str;
        }

        public static string BeforeBrackets(string s)
        {
            int sl = s.Length;
            bool stop_push = false;
            string new_str = string.Empty;

            for (int i = 0; i < sl; i++)
            {
                if (s[i] == '[')
                    stop_push = true;

                if (!stop_push)
                    new_str.Append(s[i]);
            }

            return (new_str);
        }

        public static string AfterBrackets(string s)
        {
            string @var = string.Empty;
            int sl = s.Length;
            bool start_push = false;

            for (int i = 0; i < sl; i++)
            {
                if (start_push)
                    var.Append(s[i]);
                else if (s[i] == '[')
                    start_push = true;
            }

            s = StringHelper.SubtractChars(s, "]");

            return var;
        }

        public static string AfterDot(string s)
        {
            string @var = string.Empty;
            int sl = s.Length;
            bool start_push = false;

            for (int i = 0; i < sl; i++)
            {
                if (start_push)
                    var.Append(s[i]);
                else
                {
                    if (s[i] == '.')
                        start_push = true;
                }
            }

            return var;
        }

        public static string BeforeDot(string s)
        {
            string @var = string.Empty;
            int sl = s.Length;
            bool start_push = true;

            for (int i = 0; i < sl; i++)
            {
                if (start_push)
                {
                    if (s[i] == '.')
                        start_push = false;
                    else
                        var.Append(s[i]);
                }
            }

            return var;
        }

        public static bool IsTrueString(string s)
        {
            return s == "true" || s == "1";
        }

        public static bool IsFalseString(string s)
        {
            return s == "false" || s == "0";
        }
        #endregion

        #region Conversion
        public static int StoI(string s)
        {
            return Convert.ToInt32(s);
        }

        public static string ItoS(int i)
        {
            return Convert.ToString(i);
        }

        public static string ItoS(double i)
        {
            return Convert.ToString((int)i);
        }

        public static double StoD(string s)
        {
            return Convert.ToDouble(s);
        }

        public static string DtoS(double i)
        {
            return Convert.ToString(i);
        }

        public static int GetCharAsInt32(char c)
        {
            return Char.ToLower(c) - 'a' + 1;
        }
        #endregion
    }
}
