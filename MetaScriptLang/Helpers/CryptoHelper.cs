namespace MetaScriptLang.Helpers
{
    public class CryptoHelper
    {
        public static string Decrypt(string o)
        {
            System.Text.StringBuilder ax = new();
            int l = o.Length, s = 7;

            for (int i = 0; i < l; i++)
            {
                if (s == 7)
                {
                    ax.Append((char)(o[i] + 3));
                    s = 5;
                }
                else if (s == 5)
                {
                    ax.Append((char)(o[i] - 1));
                    s = 0;
                }
                else if (s == 0)
                {
                    ax.Append((char)(o[i] + 4));
                    s = 1;
                }
                else
                {
                    ax.Append((char)(o[i] - 2));
                    s = 7;
                }
            }

            return ax.ToString();
        }

        public static string Encrypt(string o)
        {
            System.Text.StringBuilder ax = new();
            int l = o.Length, s = 7;

            for (int i = 0; i < l; i++)
            {
                if (s == 7)
                {
                    ax.Append((char)(o[i] - 3));
                    s = 5;
                }
                else if (s == 5)
                {
                    ax.Append((char)(o[i] + 1));
                    s = 0;
                }
                else if (s == 0)
                {
                    ax.Append((char)(o[i] - 4));
                    s = 1;
                }
                else
                {
                    ax.Append((char)(o[i] + 2));
                    s = 7;
                }
            }

            return ax.ToString();
        }
    }
}
