namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        void write(string st)
        {
            if (__CaptureParse)
                __ParsedOutput += (cleanString(st));
            else
                cout = cleanString(st);

            setLastValue(st);
        }

        void writeline(string st)
        {
            write(st);
            cout = System.Environment.NewLine;
        }

        void displayVersion()
        {
            cout = "\r\nnoctis v0.0.1 by <scstauf@gmail.com>\r\n" + System.Environment.NewLine;
        }

        void help(string app)
        {
            cout = "\r\nnoctis by <scstauf@gmail.com>" + System.Environment.NewLine + System.Environment.NewLine
                 + "usage:\t" + app + "\t\t\t// start the shell" + System.Environment.NewLine
                 + "\t" + app + " {args}\t\t// start the shell, with parameters" + System.Environment.NewLine
                 + "\t" + app + " {script}\t\t// interpret a script" + System.Environment.NewLine
                 + "\t" + app + " {script} {args}\t// interpret a script, with parameters" + System.Environment.NewLine
                 + "\t" + app + " -n, --negligence\t// do not terminate on parse errors" + System.Environment.NewLine
                 + "\t" + app + " -sl, --skipload\t// start the shell, skip loading saved vars" + System.Environment.NewLine
                 + "\t" + app + " -u, --uninstall\t// remove $HOME/.savedVarsPath" + System.Environment.NewLine
                 + "\t" + app + " -v, --version\t// display current version" + System.Environment.NewLine
                 + "\t" + app + " -p, --parse\t\t// parse a command" + System.Environment.NewLine
                 + "\t" + app + " -h, --help\t\t// display this message" + System.Environment.NewLine + System.Environment.NewLine;
        }

    }
}
