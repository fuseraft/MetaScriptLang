namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Helpers;

    public partial class Parser
    {
        public int main(int c, params string[] v)
        {
            string noctis = Environment.ProcessPath;
            setup();
            __Noctis = noctis;
            __InitialDirectory = System.Environment.CurrentDirectory;
            // __Logging = false;

            if (c == 1)
            {
                __CurrentScript = noctis;
                args.Add(noctis);
                __ArgumentCount = args.Count;
                loop(false);
            }
            else if (c == 2)
            {
                string opt = v[1];

                if (StringHelper.IsScript(opt))
                {
                    __CurrentScript = opt;
                    args.Add(opt);
                    __ArgumentCount = args.Count;
                    loadScript(opt);
                }
                else if (StringHelper.IsArgument(opt, "h") || StringHelper.IsArgument(opt, "help"))
                    help(noctis);
                else if (StringHelper.IsArgument(opt, "u") || StringHelper.IsArgument(opt, "uninstall"))
                    uninstall();
                else if (StringHelper.IsArgument(opt, "sl") || StringHelper.IsArgument(opt, "skipload"))
                {
                    __CurrentScript = noctis;
                    args.Add(opt);
                    __ArgumentCount = args.Count;
                    loop(true);
                }
                else if (StringHelper.IsArgument(opt, "n") || StringHelper.IsArgument(opt, "negligence"))
                {
                    __Negligence = true;
                    __CurrentScript = noctis;
                    args.Add(opt);
                    __ArgumentCount = args.Count;
                    loop(true);
                }
                else if (StringHelper.IsArgument(opt, "v") || StringHelper.IsArgument(opt, "version"))
                    displayVersion();
                else
                {
                    __CurrentScript = noctis;
                    args.Add(opt);
                    __ArgumentCount = args.Count;
                    loop(false);
                }
            }
            else if (c == 3)
            {
                string opt = v[1], script = v[2];

                if (StringHelper.IsArgument(opt, "sl") || StringHelper.IsArgument(opt, "skipload"))
                {
                    __CurrentScript = noctis;

                    if (StringHelper.IsScript(script))
                    {
                        __CurrentScript = script;
                        args.Add(opt);
                        args.Add(script);
                        __ArgumentCount = args.Count;
                        loadScript(script);
                    }
                    else
                    {
                        args.Add(opt);
                        args.Add(script);
                        __ArgumentCount = args.Count;
                        loop(true);
                    }
                }
                else if (StringHelper.IsArgument(opt, "n") || StringHelper.IsArgument(opt, "negligence"))
                {
                    __Negligence = true;
                    args.Add(opt);
                    args.Add(script);
                    __ArgumentCount = args.Count;
                    if (StringHelper.IsScript(script))
                    {
                        __CurrentScript = script;
                        loadScript(script);
                    }
                    else
                    {
                        __CurrentScript = noctis;
                        loop(true);
                    }
                }
                else if (StringHelper.IsArgument(opt, "p") || StringHelper.IsArgument(opt, "parse"))
                {
                    string stringBuilder = ("");

                    for (int i = 0; i < script.Length; i++)
                    {
                        if (script[i] == '\'')
                            stringBuilder += ('\"');
                        else
                            stringBuilder += (script[i]);
                    }

                    parse(stringBuilder);
                }
                else
                {
                    if (StringHelper.IsScript(opt))
                    {
                        __CurrentScript = opt;
                        args.Add(opt);
                        args.Add(script);
                        __ArgumentCount = args.Count;
                        loadScript(opt);
                    }
                    else
                    {
                        __CurrentScript = noctis;
                        args.Add(opt);
                        args.Add(script);
                        __ArgumentCount = args.Count;
                        loop(false);
                    }
                }
            }
            else if (c > 3)
            {
                string opt = v[1];

                if (StringHelper.IsScript(opt))
                {
                    for (int i = 2; i < c; i++)
                    {
                        string tmpStr = v[i];
                        args.Add(tmpStr);
                    }

                    __ArgumentCount = args.Count;

                    loadScript(opt);
                }
                else
                {
                    for (int i = 1; i < c; i++)
                    {
                        string tmpStr = v[i];
                        args.Add(tmpStr);
                    }

                    __ArgumentCount = args.Count;

                    __CurrentScript = noctis;
                    loop(false);
                }
            }
            else
            {
                loop(true);
            }

            gc.ClearAll();

            return (0);
        }
    }
}
