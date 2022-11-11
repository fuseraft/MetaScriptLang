namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;
    using MetaScriptLang.IO;

    public partial class Parser
    {
        #region Loop
        string GetReplPrompt()
        {
            string new_style = ("");
            int length = __PromptStyle.Length;
            char prevChar = 'a';

            for (int i = 0; i < length; i++)
            {
                switch (__PromptStyle[i])
                {
                    case 'u':
                        if (prevChar == '\\')
                            new_style += System.Environment.UserName;
                        else
                            new_style += ('u');
                        break;

                    case 'm':
                        if (prevChar == '\\')
                            new_style += System.Environment.MachineName;
                        else
                            new_style += ('m');
                        break;

                    case 'w':
                        if (prevChar == '\\')
                            new_style += System.Environment.CurrentDirectory;
                        else
                            new_style += ('w');
                        break;

                    case '\\':
                        break;

                    default:
                        new_style += __PromptStyle[i];
                        break;
                }

                prevChar = __PromptStyle[i];
            }

            return (new_style);
        }

        void StartReplLoop(bool skip)
        {
            bool active = true;

            if (!skip)
            {
                if (System.IO.File.Exists(__SavedVars))
                    LoadSavedVariables();
            }

            while (active)
            {
                if (__UseCustomPrompt)
                {
                    if (__PromptStyle == "bash")
                        ConsoleHelper.Output = System.Environment.UserName + "@" + System.Environment.MachineName + "(" + System.Environment.CurrentDirectory + ")" + "$ ";
                    else if (__PromptStyle == "empty")
                        engine.Idle();
                    else
                        ConsoleHelper.Output = GetReplPrompt();
                }
                else
                    ConsoleHelper.Output = "> ";

                string input = ConsoleHelper.GetLine();// getline(cin, s, '\n');

                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                if (input[0] == '\t')
                    input = input.Replace("\t", string.Empty);

                if (input == "exit")
                {
                    if (!__DefiningObject && !__DefiningMethod)
                    {
                        active = false;
                        gc.ClearAll();
                    }
                    else
                        ParseString(input);
                }
                else
                {
                    string c = input;
                    ParseString(StringHelper.LTrim(c));
                }
            }
        }
        #endregion

        #region Script Runner
        void RunScript()
        {
            for (int i = 0; i < engine.GetScriptSize(__CurrentScript); i++)
            {
                __CurrentLineNumber = i + 1;

                if (!engine.__GoToLabel)
                    ParseString(engine.GetLineFromScript(__CurrentLine, i));
                else
                {
                    bool startParsing = false;
                    __DefiningIfStatement = false;
                    __DefiningForLoop = false;
                    engine.__GoToLabel = false;

                    for (int z = 0; z < engine.GetScriptSize(__CurrentScript); z++)
                    {
                        if (StringHelper.StringEndsWith(engine.GetLineFromScript(__CurrentScript, z), "::"))
                        {
                            string s = engine.GetLineFromScript(__CurrentScript, z);
                            s = StringHelper.SubtractString(s, "::");

                            if (s == __GoTo)
                                startParsing = true;
                        }

                        if (startParsing)
                            ParseString(engine.GetLineFromScript(__CurrentScript, z));
                    }
                }
            }

            __CurrentScript = __PreviousScript;
        }

        void LoadScript(string script)
        {
            string s = string.Empty;
            __CurrentScript = script;

            Script newScript = new(script);

            string[] lines = System.IO.File.ReadAllLines(script);
            for (var i = 0; i < lines.Length; i++)
            {
                s = lines[i];
                if (s.Length > 0)
                {
                    if (s[0] == '\r' || s[0] == '\n')
                    {
                        engine.Idle();
                    }
                    else if (s[0] == '\t')
                    {
                        s = s.Trim();
                    }
                    else
                    {
                        newScript.AddLine(StringHelper.LTrim(s));
                    }
                }
                else
                    newScript.AddLine("");
            }

            scripts.Add(script, newScript);

            RunScript();
        }
        #endregion

        #region External Process
        int RunExternalProcess(string s, System.Collections.Generic.List<string> command)
        {
            /*string _cleaned;
	        _cleaned = cleanstring(s);
            for (int i = 0; i < methods.Count; i++)
            {
                if (command[0] == methods[i].name())
                {
                    if (command.Count - 1 == methods[i].getmethodvariables().Count)
                    {
                        // work
                    }
                }
            }*/
            //exec(cleanString(s));
            return 0;
        }
        #endregion
    }
}
