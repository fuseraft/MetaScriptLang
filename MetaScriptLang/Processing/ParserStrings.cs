namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        #region Strings
        bool isArg(string s, string si)
        {
            return (s == ("-" + si) || s == ("--" + si) || s == ("/" + si));
        }

        bool isScript(string path)
        {
            return endsWith(path, ".ns");
        }


        System.Collections.Generic.List<string> split(string s, char delim)
        {
            return new System.Collections.Generic.List<string>(s.Split(delim));
        }

        string replace(string original, string target, string replacement)
        {
            return original.Replace(target, replacement);
        }

        string ltrim(string s)
        {
            return s.TrimStart();
        }
        string rtrim(string s)
        {
            return s.TrimEnd();
        }

        string trim(string s)
        {
            return ltrim(rtrim(s));
        }

        string trimLeadingWhitespace(string str)
        {
            return str.TrimStart();
        }

        bool isUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (!Char.IsUpper(c))
                    return false;
            }

            return true;
        }

        bool isUpperConstant(string input)
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

        bool isLower(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (!char.IsLower(c))
                    return false;
            }

            return true;
        }

        string getUpper(string input)
        {
            return input.ToUpper();
        }

        string getLower(string input)
        {
            return input.ToLower();
        }

        int dot_count(string s)
        {
            int l = s.Length, c = 0;
            for (int i = 0; i < l; i++)
            {
                if (s[i] == '.')
                    c++;
            }

            return (c);
        }

        bool contains(string s1, string s2)
        {
            return s1.Contains(s2);
        }

        bool containsTilde(string s)
        {
            return s.Contains("~");
        }

        bool containsParameters(string s)
        {
            int sl = s.Length;

            for (int i = 0; i < sl; i++)
            {
                if (s[i] == '(')
                    return true;
            }

            return false;
        }

        bool containsBrackets(string s)
        {
            int sl = s.Length;

            for (int i = 0; i < sl; i++)
            {
                if (s[i] == '[')
                    return true;
            }
            return false;
        }


        System.Collections.Generic.List<string> getParameters(string s)
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

        System.Collections.Generic.List<string> getBracketRange(string s)
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
                        doNothing();
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

        System.Collections.Generic.List<string> getRange(string s)
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
                        doNothing();
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

        string beforeParameters(string s)
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

        string beforeBrackets(string s)
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

        string afterBrackets(string s)
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

            s = subtractChar(s, "]");

            return var;
        }

        string afterDot(string s)
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

        string beforeDot(string s)
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

        string afterUS(string s)
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
                    if (s[i] == '_')
                        start_push = true;
                }
            }

            return (var);
        }

        string beforeUS(string s)
        {
            string @var = string.Empty;
            int sl = s.Length;
            bool start_push = true;

            for (int i = 0; i < sl; i++)
            {
                if (start_push)
                {
                    if (s[i] == '_')
                        start_push = false;
                    else
                        var.Append(s[i]);
                }
            }

            return var;
        }

        string getInner(string s, int left, int right)
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

        string subtractChar(string s1, string s2)
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

        bool isAlpha(string s)
        {
            int l = s.Length;

            for (int i = 0; i < l; i++)
            {
                if (!char.IsLetter(s[i]))
                    return false;
            }

            return true;
        }

        bool oneDot(string s)
        {
            bool found = false;

            int l = s.Length;

            for (int i = 0; i < l; i++)
            {
                if (s[i] == '.')
                {
                    if (found)
                        return false;
                    else
                        found = true;
                }
            }

            return true;
        }

        bool endsWith(string s, string end)
        {
            return s.EndsWith(end);
        }

        bool startsWith(string s1, string s2)
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

        bool zeroDots(string s)
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

        bool zeroNumbers(string s)
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

        string subtractString(string s1, string s2)
        {
            return s1.Replace(s2, "");
        }

        bool isTrue(string s)
        {
            return s == "true" || s == "1";
        }

        bool isFalse(string s)
        {
            return s == "false" || s == "0";
        }

        // NUMBER > STRING & VICE-VERSA

        int stoi(string s)
        {
            return Convert.ToInt32(s);
        }

        string itos(int i)
        {
            return Convert.ToString(i);
        }

        double stod(string s)
        {
            return Convert.ToDouble(s);
        }

        string dtos(double i)
        {
            return Convert.ToString(i);
        }

        int get_ascii_num(char c)
        {
            return ((int)c);
        }

        int get_alpha_num(char c)
        {
            return Char.ToLower(c) - 'a' + 1;
        }

        #endregion
    }
}
