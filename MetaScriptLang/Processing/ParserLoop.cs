﻿namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;

    public partial class Parser
    {
        #region Loop
        string getPrompt()
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
                            new_style += (System.Environment.UserName);
                        else
                            new_style += ('u');
                        break;

                    case 'm':
                        if (prevChar == '\\')
                            new_style += (System.Environment.MachineName);
                        else
                            new_style += ('m');
                        break;

                    case 'w':
                        if (prevChar == '\\')
                            new_style += (System.Environment.CurrentDirectory);
                        else
                            new_style += ('w');
                        break;

                    case '\\':
                        break;

                    default:
                        new_style += (__PromptStyle[i]);
                        break;
                }

                prevChar = __PromptStyle[i];
            }

            return (new_style);
        }

        void loop(bool skip)
        {
            string s = string.Empty;
            bool active = true;

            if (!skip)
            {
                Crypt c = new();
                string bigStr = string.Empty;

                if (System.IO.File.Exists(__SavedVars))
                    loadSavedVars(c, bigStr);
            }

            while (active)
            {
                s = string.Empty;

                if (__UseCustomPrompt)
                {
                    if (__PromptStyle == "bash")
                        cout = System.Environment.UserName + "@" + System.Environment.MachineName + "(" + System.Environment.CurrentDirectory + ")" + "$ ";
                    else if (__PromptStyle == "empty")
                        doNothing();
                    else
                        cout = getPrompt();
                }
                else
                    cout = "> ";

                s = Console.ReadLine();// getline(cin, s, '\n');

                if (string.IsNullOrEmpty(s))
                {
                    continue;
                }

                if (s[0] == '\t')
                    s = s.Replace("\t", string.Empty);

                if (s == "exit")
                {
                    if (!__DefiningObject && !__DefiningMethod)
                    {
                        active = false;
                        gc.ClearAll();
                    }
                    else
                        parse(s);
                }
                else
                {
                    string c = s;
                    parse(StringHelper.LTrim(c));
                }
            }
        }
        #endregion

        #region Script Runner
        void runScript()
        {
            for (int i = 0; i < GetSSize(__CurrentScript); i++)
            {
                __CurrentLineNumber = i + 1;

                if (!__GoToLabel)
                    parse(GetSLine(__CurrentLine, i));
                else
                {
                    bool startParsing = false;
                    __DefiningIfStatement = false;
                    __DefiningForLoop = false;
                    __GoToLabel = false;

                    for (int z = 0; z < GetSSize(__CurrentScript); z++)
                    {
                        if (StringHelper.StringEndsWith(GetSLine(__CurrentScript, z), "::"))
                        {
                            string s = GetSLine(__CurrentScript, z);
                            s = StringHelper.SubtractString(s, "::");

                            if (s == __GoTo)
                                startParsing = true;
                        }

                        if (startParsing)
                            parse(GetSLine(__CurrentScript, z));
                    }
                }
            }

            __CurrentScript = __PreviousScript;
        }

        void loadScript(string script)
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
                        doNothing();
                    }
                    else if (s[0] == '\t')
                    {
                        s = s.Trim();
                    }
                    else
                    {
                        newScript.add(StringHelper.LTrim(s));
                    }
                }
                else
                    newScript.add("");
            }

            scripts.Add(script, newScript);

            runScript();
        }
        #endregion

        #region External Process
        int sysExec(string s, System.Collections.Generic.List<string> command)
        {
            /*string _cleaned;
	        _cleaned = cleanstring(s);
            for (int i = 0; i < (int)methods.Count; i++)
            {
                if (command[0] == methods[i].name())
                {
                    if ((int)command.Count - 1 == (int)methods[i].getmethodvariables().Count)
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
