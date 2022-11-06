namespace MetaScriptLang.Data
{
    public class Crypt
    {
        public Crypt() { }
        ~Crypt() { }

        public string d(string o)
        {
            return decrypt(o);
        }

        public string e(string o)
        {
            return encrypt(o);
        }

        private string decrypt(string o)
        {
            int l = o.Length, s = 7;
            string ax = string.Empty;

            for (int i = 0; i < l; i++)
            {
                if (s == 7)
                {
                    ax.Append(((char)((int)o[i] + 3)));
                    s = 5;
                }
                else if (s == 5)
                {
                    ax.Append(((char)((int)o[i] - 1)));
                    s = 0;
                }
                else if (s == 0)
                {
                    ax.Append(((char)((int)o[i] + 4)));
                    s = 1;
                }
                else
                {
                    ax.Append(((char)((int)o[i] - 2)));
                    s = 7;
                }
            }

            return (ax);
        }

        private string encrypt(string o)
        {
            int l = o.Length, s = 7;
            string ax = string.Empty;

            for (int i = 0; i < l; i++)
            {
                if (s == 7)
                {
                    ax.Append(((char)((int)o[i] - 3)));
                    s = 5;
                }
                else if (s == 5)
                {
                    ax.Append(((char)((int)o[i] + 1)));
                    s = 0;
                }
                else if (s == 0)
                {
                    ax.Append(((char)((int)o[i] - 4)));
                    s = 1;
                }
                else
                {
                    ax.Append(((char)((int)o[i] + 2)));
                    s = 7;
                }
            }

            return ax;
        }
    }
}
