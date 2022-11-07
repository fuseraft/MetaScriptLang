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
    }
}
