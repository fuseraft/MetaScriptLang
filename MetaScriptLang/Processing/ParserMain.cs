namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        public string getStdout(string command)
        {
            // execute process and return result of stdout
            return string.Empty;
        }
        public void doNothing() { }

        #region Capture
        public string getParsedOutput(string cmd)
        {
            __CaptureParse = true;
            parse(cmd);
            string ret = __ParsedOutput;
            __ParsedOutput = string.Empty;
            __CaptureParse = false;

            return ret.Length == 0 ? __LastValue : ret;
        }
        #endregion

        #region Parse
        /**
            The heart of it all. Parse a string and send for interpretation.
        **/
        public void parse(string s)
        {
            System.Collections.Generic.List<string> command = new(); // a tokenized command container
            int length = s.Length, //	length of the line
                count = 0, // command token counter
                size = 0; // final size of tokenized command container
            bool quoted = false, // flag: parsing string literals
                 broken = false, // flag: end of a command
                 uncomment = false, // flag: end a command
                 parenthesis = false; // flag: parsing contents within parentheses
            char prevChar = 'a'; // previous character in string

            StringContainer stringContainer = new(); // contains separate commands
            System.Text.StringBuilder bigString = new(); // a string to build upon

            __CurrentLine = s; // store a copy of the current line
                               // if (__Logging) app(__LogFile, s + "\r\n"); // if __Logging a session, log the line

            command.Add(string.Empty); // push back an empty string to begin.
                                       // iterate each char in the initial string
            for (int i = 0; i < length; i++)
            {
                switch (s[i])
                {
                    case ' ':
                        if (!__IsCommented)
                        {
                            if ((!parenthesis && quoted) || (parenthesis && quoted))
                            {
                                command[count] += (' ');
                            }
                            else if (parenthesis && !quoted)
                            {
                                doNothing();
                            }
                            else
                            {
                                if (prevChar != ' ')
                                {
                                    command.Add(string.Empty);
                                    count++;
                                }
                            }
                        }

                        bigString.Append(' ');
                        break;

                    case '\"':
                        quoted = !quoted;
                        if (parenthesis)
                        {
                            command[count] += ('\"');
                        }
                        bigString.Append('\"');
                        break;

                    case '(':
                        if (!parenthesis)
                            parenthesis = true;

                        command[count] += ('(');

                        bigString.Append('(');
                        break;

                    case ')':
                        if (parenthesis)
                            parenthesis = false;

                        command[count] += (')');
                        bigString.Append(')');
                        break;

                    case '\\':
                        if (quoted || parenthesis)
                        {
                            if (!__IsCommented)
                                command[count] += ('\\');
                        }

                        bigString.Append('\\');
                        break;

                    case '\'':
                        if (quoted || parenthesis)
                        {
                            if (prevChar == '\\')
                                command[count] += ('\'');
                            else
                                command[count] += ('\"');

                            bigString.Append('\'');
                        }
                        break;

                    case '#':
                        if (quoted || parenthesis)
                            command[count] += ('#');
                        else if (prevChar == '#' && __MultilineComment == false)
                        {
                            __MultilineComment = true;
                            __IsCommented = true;
                            uncomment = false;
                        }
                        else if (prevChar == '#' && __MultilineComment == true)
                            uncomment = true;
                        else if (prevChar != '#' && __MultilineComment == false)
                        {
                            __IsCommented = true;
                            uncomment = true;
                        }

                        bigString.Append('#');
                        break;

                    case '~':
                        if (!__IsCommented)
                        {
                            if (prevChar == '\\')
                                command[count] += ('~');
                            else
                            {
                                // TODO
                                /*if (__GuessedOS == OS_NIX)
                                    command[count] += (System.Environment.GetEnvironmentVariable("HOME"));
                                else
                                    command[count] += (System.Environment.GetEnvironmentVariable("HOMEPATH"));*/
                            }
                        }
                        bigString.Append('~');
                        break;

                    case ';':
                        if (!quoted)
                        {
                            if (!__IsCommented)
                            {
                                broken = true;
                                stringContainer.add(bigString.ToString());
                                bigString.Clear();
                                count = 0;
                                command.Clear();
                                command.Add(string.Empty);
                            }
                        }
                        else
                        {
                            bigString.Append(';');
                            command[count] += (';');
                        }
                        break;

                    default:
                        if (!__IsCommented)
                            command[count] += (s[i]);
                        bigString.Append(s[i]);
                        break;
                }

                prevChar = s[i];
            }

            // for (unsigned int x = 0; x < command.size(); x++) {
            // cout = x << ":\t__ " << command[x) << " __" + System.Environment.NewLine;
            // }

            size = command.Count;

            if (command[size - 1] == "{" && size != 1)
                command.RemoveAt(size - 1);

            size = command.Count;

            if (!__IsCommented)
            {
                if (!broken)
                {
                    for (int i = 0; i < size; i++)
                    {
                        // handle arguments
                        // args[0], args[1], ..., args[n-1]
                        if (command[i].Contains("args") && command[i] != "args.size")
                        {
                            System.Collections.Generic.List<string> parameters = StringHelper.GetBracketRange(command[i]);

                            if (StringHelper.IsNumeric(parameters[0]))
                            {
                                if (args.Count - 1 >= Convert.ToInt32(parameters[0]) && Convert.ToInt32(parameters[0]) >= 0)
                                {
                                    if (parameters[0] == "0")
                                        command[i] = __CurrentScript;
                                    else
                                        command[i] = args[Convert.ToInt32(parameters[0])];
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, command[i], false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, command[i], false);
                        }
                    }

                    if (__DefiningSwitchBlock)
                    {
                        if (s == "{")
                        {
                            doNothing();
                        }
                        else if (StringHelper.StringStartsWith(s, "case"))
                            mainSwitch.CreateSwitchCase(command[1]);
                        else if (s == "default")
                            __InDefaultCase = true;
                        else if (s == "end" || s == "}")
                        {
                            string switch_value = string.Empty;

                            if (IsStringVariable(__SwitchVarName))
                                switch_value = GetVariableString(__SwitchVarName);
                            else if (IsNumberVariable(__SwitchVarName))
                                switch_value = StringHelper.DtoS(GetVariableNumber(__SwitchVarName));
                            else
                                switch_value = string.Empty;

                            SwitchCase rightCase = mainSwitch.FindSwitchCase(switch_value);

                            __InDefaultCase = false;
                            __DefiningSwitchBlock = false;

                            for (int i = 0; i < rightCase.Count; i++)
                                parse(rightCase[i]);

                            mainSwitch.Clear();
                        }
                        else
                        {
                            if (__InDefaultCase)
                                mainSwitch.AddToDefaultSwitchCase(s);
                            else
                                mainSwitch.AddToCurrentSwitchCase(s);
                        }
                    }
                    else if (__DefiningModule)
                    {
                        if (s == ("[/" + __CurrentModule + "]"))
                        {
                            __DefiningModule = false;
                            __CurrentModule = string.Empty;
                        }
                        else
                            ModAddLine(__CurrentModule, s);
                    }
                    else if (__DefiningScript)
                    {
                        if (s == "__end__")
                        {
                            __CurrentScriptName = string.Empty;
                            __DefiningScript = false;
                        }
                        else
                            app(__CurrentScriptName, s + "\n");
                    }
                    else
                    {
                        if (__RaiseCatchBlock)
                        {
                            if (s == "catch")
                                __RaiseCatchBlock = false;
                        }
                        else if (__ExecutedTryBlock && s == "catch")
                            __SkipCatchBlock = true;
                        else if (__ExecutedTryBlock && __SkipCatchBlock)
                        {
                            if (s == "caught")
                            {
                                __SkipCatchBlock = false;
                                parse("caught");
                            }
                        }
                        else if (__DefiningMethod)
                        {
                            if (StringHelper.ContainsString(s, "while"))
                                __DefiningLocalWhileLoop = true;

                            if (StringHelper.ContainsString(s, "switch"))
                                __DefiningLocalSwitchBlock = true;

                            if (__DefiningParameterizedMethod)
                            {
                                if (s == "{")
                                {
                                    doNothing();
                                }
                                else if (s == "end" || s == "}")
                                {
                                    if (__DefiningLocalWhileLoop)
                                    {
                                        __DefiningLocalWhileLoop = false;

                                        if (__DefiningObject)
                                            AddLineToObjectMethod(__CurrentObject, s);
                                        else
                                            methods[CurrentMethodName].AddLine(s);
                                    }
                                    else if (__DefiningLocalSwitchBlock)
                                    {
                                        __DefiningLocalSwitchBlock = false;

                                        if (__DefiningObject)
                                            AddLineToObjectMethod(__CurrentObject, s);
                                        else
                                            methods[CurrentMethodName].AddLine(s);
                                    }
                                    else
                                    {
                                        __DefiningMethod = false;

                                        if (__DefiningObject)
                                        {
                                            __DefiningObjectMethod = false;
                                            objects[__CurrentObject].setCurrentMethod(string.Empty);
                                        }
                                    }
                                }
                                else
                                {
                                    int _len = s.Length;
                                    System.Collections.Generic.List<string> words = new();
                                    string word = string.Empty;

                                    for (int z = 0; z < _len; z++)
                                    {
                                        if (s[z] == ' ')
                                        {
                                            words.Add(word);
                                            word = string.Empty;
                                        }
                                        else
                                            word += (s[z]);
                                    }

                                    words.Add(word);

                                    string freshLine = string.Empty;

                                    for (int z = 0; z < words.Count; z++)
                                    {
                                        if (VariableExists(words[z]))
                                        {
                                            if (IsStringVariable(words[z]))
                                                freshLine += (GetVariableString(words[z]));
                                            else if (IsNumberVariable(words[z]))
                                                freshLine += (StringHelper.DtoS(GetVariableNumber(words[z])));
                                        }
                                        else
                                            freshLine += (words[z]);

                                        if (z != words.Count - 1)
                                            freshLine += (' ');
                                    }

                                    if (__DefiningObject)
                                    {
                                        AddLineToObjectMethod(__CurrentObject, freshLine);

                                        if (__DefiningPublicCode)
                                            SetObjectAsPublic(__CurrentObject);
                                        else if (__DefiningPrivateCode)
                                            SetObjectAsPrivate(__CurrentObject);
                                        else
                                            SetObjectAsPublic(__CurrentObject);
                                    }
                                    else
                                        methods[CurrentMethodName].AddLine(freshLine);
                                }
                            }
                            else
                            {
                                if (s == "{")
                                    doNothing();
                                else if (s == "end" || s == "}")
                                {
                                    if (__DefiningLocalWhileLoop)
                                    {
                                        __DefiningLocalWhileLoop = false;

                                        if (__DefiningObject)
                                            objects[__CurrentObject].addToCurrentMethod(s);
                                        else
                                            methods[CurrentMethodName].AddLine(s);
                                    }
                                    else if (__DefiningLocalSwitchBlock)
                                    {
                                        __DefiningLocalSwitchBlock = false;

                                        if (__DefiningObject)
                                            objects[__CurrentObject].addToCurrentMethod(s);
                                        else
                                            methods[CurrentMethodName].AddLine(s);
                                    }
                                    else
                                    {
                                        __DefiningMethod = false;

                                        if (__DefiningObject)
                                        {
                                            __DefiningObjectMethod = false;
                                            objects[__CurrentObject].setCurrentMethod(string.Empty);
                                        }
                                    }
                                }
                                else
                                {
                                    if (__DefiningObject)
                                    {
                                        objects[__CurrentObject].addToCurrentMethod(s);

                                        if (__DefiningPublicCode)
                                            objects[__CurrentObject].setPublic();
                                        else if (__DefiningPrivateCode)
                                            objects[__CurrentObject].setPrivate();
                                        else
                                            objects[__CurrentObject].setPublic();
                                    }
                                    else
                                    {
                                        if (__DefiningObjectMethod)
                                        {
                                            objects[__CurrentObject].addToCurrentMethod(s);

                                            if (__DefiningPublicCode)
                                                objects[__CurrentObject].setPublic();
                                            else if (__DefiningPrivateCode)
                                                objects[__CurrentObject].setPrivate();
                                            else
                                                objects[__CurrentObject].setPublic();
                                        }
                                        else
                                            methods[CurrentMethodName].AddLine(s);
                                    }
                                }
                            }
                        }
                        else if (__DefiningIfStatement)
                        {
                            if (__DefiningNest)
                            {
                                if (command[0] == "endif")
                                    executeNest(ifStatements[ifStatements.Count - 1].GetNest());
                                else
                                    ifStatements[(int)ifStatements.Count - 1].AddToNest(s);
                            }
                            else
                            {
                                if (command[0] == "if")
                                {
                                    __DefiningNest = true;

                                    if (size == 4)
                                        threeSpace("if", command[1], command[2], command[3], s, command);
                                    else
                                    {
                                        setFalseIf();
                                        __DefiningNest = false;
                                    }
                                }
                                else if (command[0] == "endif")
                                {
                                    __DefiningIfStatement = false;
                                    __ExecutedIfStatement = true;

                                    for (int i = 0; i < ifStatements.Count; i++)
                                    {
                                        if (ifStatements[i].IsIfStatement())
                                        {
                                            executeMethod(ifStatements[i]);

                                            if (__FailedIfStatement == false)
                                                break;
                                        }
                                    }

                                    __ExecutedIfStatement = false;

                                    ifStatements.Clear();

                                    __IfStatementCount = 0;
                                    __FailedIfStatement = false;
                                }
                                else if (command[0] == "elsif" || command[0] == "elif")
                                {
                                    if (size == 4)
                                        threeSpace("if", command[1], command[2], command[3], s, command);
                                    else
                                        setFalseIf();
                                }
                                else if (s == "else")
                                    threeSpace("if", "true", "==", "true", "if true == true", command);
                                else if (s == "failif")
                                {
                                    if (__FailedIfStatement == true)
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                    ifStatements[(int)ifStatements.Count - 1].AddLine(s);
                            }
                        }
                        else
                        {
                            if (__DefiningWhileLoop)
                            {
                                if (s == "{")
                                    doNothing();
                                else if (command[0] == "end" || command[0] == "}")
                                {
                                    __DefiningWhileLoop = false;

                                    string v1 = whileLoops[whileLoops.Count - 1].FirstValue(),
                                           v2 = whileLoops[whileLoops.Count - 1].SecondValue(),
                                           op = whileLoops[whileLoops.Count - 1].LogicalOperator();

                                    if (VariableExists(v1) && VariableExists(v2))
                                    {
                                        if (op == "==")
                                        {
                                            while (GetVariableNumber(v1) == GetVariableNumber(v2))
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                        else if (op == "<")
                                        {
                                            while (GetVariableNumber(v1) < GetVariableNumber(v2))
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                        else if (op == ">")
                                        {
                                            while (GetVariableNumber(v1) > GetVariableNumber(v2))
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                        else if (op == "<=")
                                        {
                                            while (GetVariableNumber(v1) <= GetVariableNumber(v2))
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                        else if (op == ">=")
                                        {
                                            while (GetVariableNumber(v1) >= GetVariableNumber(v2))
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                        else if (op == "!=")
                                        {
                                            while (GetVariableNumber(v1) != GetVariableNumber(v2))
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                    }
                                    else if (VariableExists(v1))
                                    {
                                        if (op == "==")
                                        {
                                            while (GetVariableNumber(v1) == StringHelper.StoI(v2))
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                        else if (op == "<")
                                        {
                                            while (GetVariableNumber(v1) < StringHelper.StoI(v2))
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                        else if (op == ">")
                                        {
                                            while (GetVariableNumber(v1) > StringHelper.StoI(v2))
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                        else if (op == "<=")
                                        {
                                            while (GetVariableNumber(v1) <= StringHelper.StoI(v2))
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                        else if (op == ">=")
                                        {
                                            while (GetVariableNumber(v1) >= StringHelper.StoI(v2))
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                        else if (op == "!=")
                                        {
                                            while (GetVariableNumber(v1) != StringHelper.StoI(v2))
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                    }
                                }
                                else
                                    whileLoops[whileLoops.Count - 1].AddLine(s);
                            }
                            else if (__DefiningForLoop)
                            {
                                if (command[0] == "next" || command[0] == "endfor")
                                {
                                    __DefiningForLoop = false;

                                    for (int i = 0; i < (int)forLoops.Count; i++)
                                        if (forLoops[i].IsForLoop())
                                            forLoop(forLoops[i]);

                                    forLoops.Clear();

                                    __ForLoopCount = 0;
                                }
                                else
                                {
                                    if (s == "{")
                                        doNothing();
                                    else
                                        forLoops[forLoops.Count - 1].AddLine(s);
                                }
                            }
                            else
                            {
                                if (size == 1)
                                {
                                    if (notStandardZeroSpace(command[0]))
                                    {
                                        string before = (StringHelper.BeforeDot(s)), after = (StringHelper.AfterDot(s));

                                        if (before.Length != 0 && after.Length != 0)
                                        {
                                            if (engine.ObjectExists(before) && after.Length != 0)
                                            {
                                                if (StringHelper.ContainsParameters(after))
                                                {
                                                    s = StringHelper.SubtractChars(s, "\"");

                                                    if (ObjectMethodExists(before, StringHelper.BeforeParameters(after)))
                                                        executeTemplate(GetObjectMethod(before, StringHelper.BeforeParameters(after)), StringHelper.GetParameters(after));
                                                    else
                                                        sysExec(s, command);
                                                }
                                                else if (ObjectMethodExists(before, after))
                                                    executeMethod(GetObjectMethod(before, after));
                                                else if (ObjectVariableExists(before, after))
                                                {
                                                    if (GetObjectVariableString(before, after) != __Null)
                                                        writeline(GetObjectVariableString(before, after));
                                                    else if (GetObjectVariableNumber(before, after) != __NullNum)
                                                        writeline(StringHelper.DtoS(GetObjectVariableNumber(before, after)));
                                                    else
                                                        ErrorLogger.Error(ErrorLogger.IS_NULL, string.Empty, false);
                                                }
                                                else if (after == "clear")
                                                    engine.ClearObject(before);
                                                else
                                                    ErrorLogger.Error(ErrorLogger.UNDEFINED, string.Empty, false);
                                            }
                                            else
                                            {
                                                if (before == "env")
                                                {
                                                    InternalGetEnv(string.Empty, after, 3);
                                                }
                                                else if (VariableExists(before))
                                                {
                                                    if (after == "clear")
                                                        parse(before + " = __Null");
                                                }
                                                else if (engine.ListExists(before))
                                                {
                                                    // REFACTOR HERE
                                                    if (after == "clear")
                                                        engine.ListClear(before);
                                                    else if (after == "sort")
                                                        engine.ListSort(before);
                                                    else if (after == "reverse")
                                                        engine.ListReverse(before);
                                                    else if (after == "revert")
                                                        engine.ListRevert(before);
                                                }
                                                else if (before == "self")
                                                {
                                                    if (__ExecutedMethod)
                                                        executeMethod(GetObjectMethod(__CurrentMethodObject, after));
                                                }
                                                else
                                                    sysExec(s, command);
                                            }
                                        }
                                        else if (StringHelper.StringEndsWith(s, "::"))
                                        {
                                            if (__CurrentScript != string.Empty)
                                            {
                                                string newMark = StringHelper.SubtractString(s, "::");
                                                SetSMark(__CurrentScript, newMark);
                                            }
                                        }
                                        else if (MethodExists(s))
                                            executeMethod(GetMethod(s));
                                        else if (StringHelper.StringStartsWith(s, "[") && StringHelper.StringEndsWith(s, "]"))
                                        {
                                            InternalCreateModule(s);
                                        }
                                        else
                                        {
                                            s = StringHelper.SubtractChars(s, "\"");

                                            if (MethodExists(StringHelper.BeforeParameters(s)))
                                                executeTemplate(GetMethod(StringHelper.BeforeParameters(s)), StringHelper.GetParameters(s));
                                            else
                                                sysExec(s, command);
                                        }
                                    }
                                    else
                                        zeroSpace(command[0], s, command);
                                }
                                else if (size == 2)
                                {
                                    if (notStandardOneSpace(command[0]))
                                        sysExec(s, command);
                                    else
                                    {
                                        oneSpace(command[0], command[1], s, command);
                                    }
                                }
                                else if (size == 3)
                                {
                                    if (notStandardTwoSpace(command[1]))
                                    {
                                        if (command[0] == "append")
                                            appendText(command[1], command[2], false);
                                        else if (command[0] == "appendl")
                                            appendText(command[1], command[2], true);
                                        else if ((command[0] == "fwrite"))
                                            __fwrite(command[1], command[2]);
                                        else if (command[0] == "redefine")
                                            MemRedefine(command[1], command[2]);
                                        else if (command[0] == "loop")
                                        {
                                            if (StringHelper.ContainsParameters(command[2]))
                                            {
                                                __DefaultLoopSymbol = command[2];
                                                __DefaultLoopSymbol = StringHelper.SubtractChars(__DefaultLoopSymbol, "(");
                                                __DefaultLoopSymbol = StringHelper.SubtractChars(__DefaultLoopSymbol, ")");

                                                oneSpace(command[0], command[1], StringHelper.SubtractString(s, command[2]), command);
                                                __DefaultLoopSymbol = "$";
                                            }
                                            else
                                                sysExec(s, command);
                                        }
                                        else
                                            sysExec(s, command);
                                    }
                                    else
                                        twoSpace(command[0], command[1], command[2], s, command);
                                }
                                else if (size == 4)
                                    threeSpace(command[0], command[1], command[2], command[3], s, command);
                                else if (size == 5)
                                {
                                    if (command[0] == "for")
                                    {
                                        if (StringHelper.ContainsParameters(command[4]))
                                        {
                                            __DefaultLoopSymbol = command[4];
                                            __DefaultLoopSymbol = StringHelper.SubtractChars(__DefaultLoopSymbol, "(");
                                            __DefaultLoopSymbol = StringHelper.SubtractChars(__DefaultLoopSymbol, ")");

                                            threeSpace(command[0], command[1], command[2], command[3], StringHelper.SubtractString(s, command[4]), command);
                                            __DefaultLoopSymbol = "$";
                                        }
                                        else
                                            sysExec(s, command);
                                    }
                                    else
                                        sysExec(s, command);
                                }
                                else
                                    sysExec(s, command);
                            }
                        }
                    }
                }
                else
                {
                    stringContainer.add(bigString.ToString());

                    for (int i = 0; i < (int)stringContainer.size(); i++)
                        parse(stringContainer.at(i));
                }
            }
            else
            {
                if (__MultilineComment)
                {
                    if (uncomment)
                    {
                        __IsCommented = false;
                        __MultilineComment = false;
                    }
                }
                else
                {
                    if (uncomment)
                    {
                        __IsCommented = false;
                        uncomment = false;

                        if (!broken)
                        {
                            string commentString = string.Empty;

                            bool commentFound = false;

                            for (int i = 0; i < (int)bigString.Length; i++)
                            {
                                if (bigString[i] == '#')
                                    commentFound = true;

                                if (!commentFound)
                                    commentString += (bigString[i]);
                            }

                            parse(StringHelper.LTrim(commentString));
                        }
                        else
                        {
                            string commentString = string.Empty;

                            bool commentFound = false;

                            for (int i = 0; i < (int)bigString.Length; i++)
                            {
                                if (bigString[i] == '#')
                                    commentFound = true;

                                if (!commentFound)
                                    commentString += (bigString[i]);
                            }

                            stringContainer.add(StringHelper.LTrim(commentString));

                            for (int i = 0; i < (int)stringContainer.size(); i++)
                                parse(stringContainer.at(i));
                        }
                    }
                }
            }
        }
        #endregion

        #region Zero Space
        public void zeroSpace(string arg0, string s, System.Collections.Generic.List<string> command)
        {
            if (arg0 == "pass")
            {
                return;
            }
            else if (arg0 == "caught")
            {
                string to_remove = "remove ";
                to_remove += __ErrorVarName;

                parse(to_remove);

                __ExecutedTryBlock = false;
                __RaiseCatchBlock = false;
                __LastError = string.Empty;
                __ErrorVarName = string.Empty;
            }
            else if (arg0 == "clear_methods!")
                gc.ClearMethods();
            else if (arg0 == "clear_objects!")
                gc.ClearObjects();
            else if (arg0 == "clear_variables!")
                gc.ClearVariables();
            else if (arg0 == "clear_lists!")
                gc.ClearLists();
            else if (arg0 == "clear_all!")
                gc.ClearAll();
            else if (arg0 == "clear_constants!")
                gc.ClearConstants();
            else if (arg0 == "exit")
            {
                gc.ClearAll();
                System.Environment.Exit(0);
            }
            else if (arg0 == "break" || arg0 == "leave!")
                __Breaking = true;
            else if (arg0 == "no_methods?")
            {
                if (noMethods())
                    __true();
                else
                    __false();
            }
            else if (arg0 == "no_objects?")
            {
                if (noObjects())
                    __true();
                else
                    __false();
            }
            else if (arg0 == "no_variables?")
            {
                if (noVariables())
                    __true();
                else
                    __false();
            }
            else if (arg0 == "no_lists?")
            {
                if (noLists())
                    __true();
                else
                    __false();
            }
            else if (arg0 == "end" || arg0 == "}")
            {
                __DefiningPrivateCode = false;
                __DefiningPublicCode = false;
                __DefiningObject = false;
                __DefiningObjectMethod = false;
                __CurrentObject = string.Empty;
            }
            else if (arg0 == "parser")
                loop(false);
            else if (arg0 == "private")
            {
                __DefiningPrivateCode = true;
                __DefiningPublicCode = false;
            }
            else if (arg0 == "public")
            {
                __DefiningPrivateCode = false;
                __DefiningPublicCode = true;
            }
            else if (arg0 == "try")
                __ExecutedTryBlock = true;
            else if (arg0 == "failif")
            {
                if (__FailedIfStatement == true)
                    setTrueIf();
                else
                    setFalseIf();
            }
            else
                sysExec(s, command);
        }
        #endregion

        #region One Space
        void oneSpace(string arg0, string arg1, string s, System.Collections.Generic.List<string> command)
        {
            string before = (StringHelper.BeforeDot(arg1)), after = (StringHelper.AfterDot(arg1));

            if (StringHelper.ContainsString(arg1, "self."))
            {
                arg1 = StringHelper.ReplaceString(arg1, "self", __CurrentMethodObject);
            }

            if (arg0 == "return")
            {
                if (!InternalReturn(arg0, arg1, before, after))
                    oneSpace("return", arg1, "return " + arg1, command);
            }
            else if (arg0 == "switch")
            {
                if (VariableExists(arg1))
                {
                    __DefiningSwitchBlock = true;
                    __SwitchVarName = arg1;
                }
                else
                    ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, arg1, false);
            }
            else if (arg0 == "goto")
            {
                if (__CurrentScript != string.Empty)
                {
                    if (GetSMarkExists(__CurrentScript, arg1))
                    {
                        __GoTo = arg1;
                        __GoToLabel = true;
                    }
                }
            }
            else if (arg0 == "if")
            {
                string tmpValue = string.Empty;
                // if arg1 is a variable
                if (VariableExists(arg1))
                {
                    // can we can assume that arg1 belongs to an object?
                    if (!StringHelper.ZeroDots(arg1))
                    {
                        string objName = StringHelper.BeforeDot(arg1), varName = StringHelper.AfterDot(arg1);
                        Variable tmpVar = GetObjectVariable(objName, varName);

                        if (IsStringVariable(tmpVar))
                        {
                            tmpValue = tmpVar.getString();
                        }
                        else if (IsNumberVariable(tmpVar))
                        {
                            tmpValue = StringHelper.DtoS(tmpVar.getNumber());
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, true);
                        }
                    }
                    else
                    {
                        if (IsStringVariable(arg1))
                        {
                            tmpValue = GetVariableString(arg1);
                        }
                        else if (IsNumberVariable(arg1))
                        {
                            tmpValue = Convert.ToString(GetVariableNumber(arg1));
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, true);
                        }
                    }
                }
                else
                {
                    if (StringHelper.IsNumeric(arg1) || StringHelper.IsTrueString(arg1) || StringHelper.IsFalseString(arg1))
                    {
                        tmpValue = arg1;
                    }
                    else
                    {
                        string tmpCode = string.Empty;

                        if (StringHelper.StringStartsWith(arg1, "(\"") && StringHelper.StringEndsWith(arg1, "\")"))
                        {
                            tmpCode = StringHelper.GetInnerString(arg1, 2, arg1.Length - 3);
                        }
                        else
                        {
                            tmpCode = arg1;
                        }

                        tmpValue = getParsedOutput(tmpCode);
                    }
                }

                if (StringHelper.IsTrueString(tmpValue))
                {
                    setTrueIf();
                }
                else if (StringHelper.IsFalseString(tmpValue))
                {
                    setFalseIf();
                }
                else
                {
                    ErrorLogger.Error(ErrorLogger.INVALID_OP, arg1, true);
                }
            }
            else if (arg0 == "prompt")
            {
                if (arg1 == "bash")
                {
                    __UseCustomPrompt = true;
                    __PromptStyle = "bash";
                }
                else if (arg1 == "!")
                {
                    if (__UseCustomPrompt == true)
                        __UseCustomPrompt = false;
                    else
                        __UseCustomPrompt = true;
                }
                else if (arg1 == "empty")
                {
                    __UseCustomPrompt = true;
                    __PromptStyle = "empty";
                }
                else
                {
                    __UseCustomPrompt = true;
                    __PromptStyle = arg1;
                }
            }
            else if (arg0 == "err" || arg0 == "error")
            {
                if (VariableExists(arg1))
                {
                    if (IsStringVariable(arg1))
                        cerr = GetVariableString(arg1) + System.Environment.NewLine;
                    else if (IsNumberVariable(arg1))
                        cerr = GetVariableNumber(arg1) + System.Environment.NewLine;
                    else
                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                }
                else
                    cerr = arg1 + System.Environment.NewLine;
            }
            else if (arg0 == "delay")
            {
                if (StringHelper.IsNumeric(arg1))
                    delay(StringHelper.StoI(arg1));
                else
                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg1, false);
            }
            else if (arg0 == "loop")
                threeSpace("for", "var", "in", arg1, "for var in " + arg1, command); // REFACTOR HERE
            else if (arg0 == "for" && arg1 == "infinity")
                successfulFor();
            else if (arg0 == "remove")
            {
                if (StringHelper.ContainsParameters(arg1))
                {
                    System.Collections.Generic.List<string> parameters = StringHelper.GetParameters(arg1);

                    for (int i = 0; i < parameters.Count; i++)
                    {
                        if (VariableExists(parameters[i]))
                            DeleteVariable(parameters[i]);
                        else if (engine.ListExists(parameters[i]))
                            engine.DeleteList(parameters[i]);
                        else if (engine.ObjectExists(parameters[i]))
                            engine.DeleteObject(parameters[i]);
                        else if (MethodExists(parameters[i]))
                            DeleteMethod(parameters[i]);
                        else
                            ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, parameters[i], false);
                    }
                }
                else if (VariableExists(arg1))
                    DeleteVariable(arg1);
                else if (engine.ListExists(arg1))
                    engine.DeleteList(arg1);
                else if (engine.ObjectExists(arg1))
                    engine.DeleteObject(arg1);
                else if (MethodExists(arg1))
                    DeleteMethod(arg1);
                else
                    ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
            }
            else if (arg0 == "see_string")
            {
                if (VariableExists(arg1))
                    write(GetVariableString(arg1));
                else
                    ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, arg1, false);
            }
            else if (arg0 == "see_number")
            {
                if (VariableExists(arg1))
                    write(StringHelper.DtoS(GetVariableNumber(arg1)));
                else
                    ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, arg1, false);
            }
            else if (arg0 == "__begin__")
            {
                if (VariableExists(arg1))
                {
                    if (IsStringVariable(arg1))
                    {
                        if (!System.IO.File.Exists(GetVariableString(arg1)))
                        {
                            createFile(GetVariableString(arg1));
                            __DefiningScript = true;
                            __CurrentScriptName = GetVariableString(arg1);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.FILE_EXISTS, GetVariableString(arg1), false);
                    }
                }
                else if (!System.IO.File.Exists(arg1))
                {
                    createFile(arg1);
                    __DefiningScript = true;
                    __CurrentScriptName = arg1;
                }
                else
                    ErrorLogger.Error(ErrorLogger.FILE_EXISTS, arg1, false);
            }
            else if (arg0 == "encrypt" || arg0 == "decrypt")
            {
                InternalEncryptDecrypt(arg0, arg1);
            }
            else if (arg0 == "globalize")
            {
                InternalGlobalize(arg0, arg1);
            }
            else if (arg0 == "remember" || arg0 == "save")
            {
                InternalRemember(arg0, arg1);
            }
            else if (arg0 == "forget" || arg0 == "lose")
            {
                InternalForget(arg0, arg1);
            }
            else if (arg0 == "load")
            {
                if (System.IO.File.Exists(arg1))
                {
                    if (StringHelper.IsScript(arg1))
                    {
                        __PreviousScript = __CurrentScript;
                        loadScript(arg1);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.BAD_LOAD, arg1, true);
                }
                else if (ModExists(arg1))
                {
                    System.Collections.Generic.List<string> lines = ModGetLines(arg1);

                    for (int i = 0; i < lines.Count; i++)
                        parse(lines[i]);
                }
                else
                    ErrorLogger.Error(ErrorLogger.BAD_LOAD, arg1, true);
            }
            else if (arg0 == "say" || arg0 == "stdout" || arg0 == "out" || arg0 == "print" || arg0 == "println")
            {
                InternalOutput(arg0, arg1);
            }
            else if (arg0 == "cd" || arg0 == "chdir")
            {
                if (VariableExists(arg1))
                {
                    if (IsStringVariable(arg1))
                    {
                        if (System.IO.Directory.Exists(GetVariableString(arg1)))
                            System.Environment.CurrentDirectory = (GetVariableString(arg1));
                        else
                            ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(arg1), false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.NULL_STRING, arg1, false);
                }
                else
                {
                    if (arg1 == "init_dir" || arg1 == "initial_directory")
                        System.Environment.CurrentDirectory = (__InitialDirectory);
                    else if (System.IO.Directory.Exists(arg1))
                        System.Environment.CurrentDirectory = (arg1);
                    else
                        System.Environment.CurrentDirectory = (arg1);
                }
            }
            else if (arg0 == "list")
            {
                if (engine.ListExists(arg1))
                    engine.ListClear(arg1);
                else
                {
                    List newList = new(arg1);

                    if (__ExecutedTemplate || __ExecutedMethod)
                        newList.collect();
                    else
                        newList.dontCollect();

                    lists.Add(arg1, newList);
                }
            }
            else if (arg0 == "!")
            {
                if (VariableExists(arg1))
                {
                    if (IsStringVariable(arg1))
                        parse(GetVariableString(arg1));
                    else
                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                }
                else
                    parse(arg1);
            }
            else if (arg0 == "?")
            {
                if (VariableExists(arg1))
                {
                    if (IsStringVariable(arg1))
                        sysExec(GetVariableString(arg1), command);
                    else
                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                }
                else
                    sysExec(arg1, command);
            }
            else if (arg0 == "init_dir" || arg0 == "initial_directory")
            {
                if (VariableExists(arg1))
                {
                    if (IsStringVariable(arg1))
                    {
                        if (System.IO.Directory.Exists(GetVariableString(arg1)))
                        {
                            __InitialDirectory = GetVariableString(arg1);
                            System.Environment.CurrentDirectory = (__InitialDirectory);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.READ_FAIL, __InitialDirectory, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.NULL_STRING, arg1, false);
                }
                else
                {
                    if (System.IO.Directory.Exists(arg1))
                    {
                        if (arg1 == ".")
                            __InitialDirectory = System.Environment.CurrentDirectory;
                        else if (arg1 == "..")
                            __InitialDirectory = System.Environment.CurrentDirectory + "\\..";
                        else
                            __InitialDirectory = arg1;

                        System.Environment.CurrentDirectory = (__InitialDirectory);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.READ_FAIL, __InitialDirectory, false);
                }
            }
            else if (arg0 == "method?")
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (ObjectMethodExists(before, after))
                        __true();
                    else
                        __false();
                }
                else
                {
                    if (MethodExists(arg1))
                        __true();
                    else
                        __false();
                }
            }
            else if (arg0 == "object?")
            {
                if (engine.ObjectExists(arg1))
                    __true();
                else
                    __false();
            }
            else if (arg0 == "variable?")
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (ObjectMethodExists(before, after))
                        __true();
                    else
                        __false();
                }
                else
                {
                    if (VariableExists(arg1))
                        __true();
                    else
                        __false();
                }
            }
            else if (arg0 == "list?")
            {
                if (engine.ListExists(arg1))
                    __true();
                else
                    __false();
            }
            else if (arg0 == "directory?")
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (ObjectMethodExists(before, after))
                    {
                        if (System.IO.Directory.Exists(GetObjectVariableString(before, after)))
                            __true();
                        else
                            __false();
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                {
                    if (VariableExists(arg1))
                    {
                        if (IsStringVariable(arg1))
                        {
                            if (System.IO.Directory.Exists(GetVariableString(arg1)))
                                __true();
                            else
                                __false();
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.NULL_STRING, arg1, false);
                    }
                    else
                    {
                        if (System.IO.Directory.Exists(arg1))
                            __true();
                        else
                            __false();
                    }
                }
            }
            else if (arg0 == "file?")
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (ObjectMethodExists(before, after))
                    {
                        if (System.IO.File.Exists(GetObjectVariableString(before, after)))
                            __true();
                        else
                            __false();
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                {
                    if (VariableExists(arg1))
                    {
                        if (IsStringVariable(arg1))
                        {
                            if (System.IO.File.Exists(GetVariableString(arg1)))
                                __true();
                            else
                                __false();
                        }
                        else
                            __false();
                    }
                    else
                    {
                        if (System.IO.File.Exists(arg1))
                            __true();
                        else
                            __false();
                    }
                }
            }
            else if (arg0 == "collect?")
            {
                if (VariableExists(arg1))
                {
                    if (GCCanCollectVariable(arg1))
                        __true();
                    else
                        __false();
                }
                else
                    cout = "under construction..." + System.Environment.NewLine;
            }
            else if (arg0 == "number?")
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (ObjectMethodExists(before, after))
                    {
                        if (GetObjectVariableNumber(before, after) != __NullNum)
                            __true();
                        else
                            __false();
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                {
                    if (VariableExists(arg1))
                    {
                        if (IsNumberVariable(arg1))
                            __true();
                        else
                            __false();
                    }
                    else
                    {
                        if (StringHelper.IsNumeric(arg1))
                            __true();
                        else
                            __false();
                    }
                }
            }
            else if (arg0 == "string?")
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (ObjectMethodExists(before, after))
                    {
                        if (GetObjectVariableString(before, after) != __Null)
                            __true();
                        else
                            __false();
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                {
                    if (VariableExists(arg1))
                    {
                        if (IsStringVariable(arg1))
                            __true();
                        else
                            __false();
                    }
                    else
                    {
                        if (StringHelper.IsNumeric(arg1))
                            __false();
                        else
                            __true();
                    }
                }
            }
            else if (arg0 == "uppercase?")
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (ObjectMethodExists(before, after))
                    {
                        if (StringHelper.IsUppercase(GetObjectVariableString(before, after)))
                            __true();
                        else
                            __false();
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                {
                    if (VariableExists(arg1))
                    {
                        if (IsStringVariable(arg1))
                        {
                            if (StringHelper.IsUppercase(GetVariableString(arg1)))
                                __true();
                            else
                                __false();
                        }
                        else
                            __false();
                    }
                    else
                    {
                        if (StringHelper.IsNumeric(arg1))
                            __false();
                        else
                        {
                            if (StringHelper.IsUppercase(arg1))
                                __true();
                            else
                                __false();
                        }
                    }
                }
            }
            else if (arg0 == "lowercase?")
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (ObjectMethodExists(before, after))
                    {
                        if (StringHelper.IsLowercase(GetObjectVariableString(before, after)))
                            __true();
                        else
                            __false();
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                {
                    if (VariableExists(arg1))
                    {
                        if (IsStringVariable(arg1))
                        {
                            if (StringHelper.IsLowercase(GetVariableString(arg1)))
                                __true();
                            else
                                __false();
                        }
                        else
                            __false();
                    }
                    else
                    {
                        if (StringHelper.IsNumeric(arg1))
                            __false();
                        else
                        {
                            if (StringHelper.IsLowercase(arg1))
                                __true();
                            else
                                __false();
                        }
                    }
                }
            }
            else if (arg0 == "see")
            {
                InternalInspect(arg0, arg1, before, after);
            }
            else if (arg0 == "template")
            {
                if (MethodExists(arg1))
                    ErrorLogger.Error(ErrorLogger.METHOD_DEFINED, arg1, false);
                else
                {
                    if (StringHelper.ContainsParameters(arg1))
                    {
                        System.Collections.Generic.List<string> parameters = StringHelper.GetParameters(arg1);
                        Method method = new(StringHelper.BeforeParameters(arg1), true);

                        method.SetTemplateSize(parameters.Count);

                        methods.Add(method.GetName(), method);

                        __DefiningMethod = true;
                    }
                }
            }
            else if (arg0 == "lock")
            {
                if (VariableExists(arg1))
                    LockVariable(arg1);
                else if (MethodExists(arg1))
                    LockMethod(arg1);
            }
            else if (arg0 == "unlock")
            {
                if (VariableExists(arg1))
                    UnlockVariable(arg1);
                else if (MethodExists(arg1))
                    UnlockMethod(arg1);
            }
            else if (arg0 == "method" || arg0 == "[method]")
            {
                InternalCreateMethod(arg0, arg1);
            }
            else if (arg0 == "call_method")
            {
                InternalCallMethod(arg0, arg1, before, after);
            }
            else if (arg0 == "object")
            {
                InternalCreateObject(arg1);
            }
            else if (arg0 == "fpush")
            {
                if (VariableExists(arg1))
                {
                    if (IsStringVariable(arg1))
                    {
                        if (!System.IO.File.Exists(GetVariableString(arg1)))
                            createFile(GetVariableString(arg1));
                        else
                            ErrorLogger.Error(ErrorLogger.FILE_EXISTS, GetVariableString(arg1), false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.NULL_STRING, arg1, false);
                }
                else
                {
                    if (!System.IO.File.Exists(arg1))
                        createFile(arg1);
                    else
                        ErrorLogger.Error(ErrorLogger.FILE_EXISTS, arg1, false);
                }
            }
            else if (arg0 == "fpop")
            {
                if (VariableExists(arg1))
                {
                    if (IsStringVariable(arg1))
                    {
                        if (System.IO.File.Exists(GetVariableString(arg1)))
                            System.IO.File.Delete(GetVariableString(arg1));
                        else
                            ErrorLogger.Error(ErrorLogger.FILE_NOT_FOUND, GetVariableString(arg1), false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.NULL_STRING, arg1, false);
                }
                else
                {
                    if (System.IO.File.Exists(arg1))
                        System.IO.File.Delete(arg1);
                    else
                        ErrorLogger.Error(ErrorLogger.FILE_NOT_FOUND, arg1, false);
                }
            }
            else if (arg0 == "dpush")
            {
                if (VariableExists(arg1))
                {
                    if (IsStringVariable(arg1))
                    {
                        if (!System.IO.Directory.Exists(GetVariableString(arg1)))
                            System.IO.Directory.CreateDirectory(GetVariableString(arg1));
                        else
                            ErrorLogger.Error(ErrorLogger.DIR_EXISTS, GetVariableString(arg1), false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.NULL_STRING, arg1, false);
                }
                else
                {
                    if (!System.IO.Directory.Exists(arg1))
                        System.IO.Directory.CreateDirectory(arg1);
                    else
                        ErrorLogger.Error(ErrorLogger.DIR_EXISTS, arg1, false);
                }
            }
            else if (arg0 == "dpop")
            {
                if (VariableExists(arg1))
                {
                    if (IsStringVariable(arg1))
                    {
                        if (System.IO.Directory.Exists(GetVariableString(arg1)))
                            System.IO.Directory.Delete(GetVariableString(arg1));
                        else
                            ErrorLogger.Error(ErrorLogger.DIR_NOT_FOUND, GetVariableString(arg1), false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.NULL_STRING, arg1, false);
                }
                else
                {
                    if (System.IO.Directory.Exists(arg1))
                        System.IO.Directory.Delete(arg1);
                    else
                        ErrorLogger.Error(ErrorLogger.DIR_NOT_FOUND, arg1, false);
                }
            }
            else
                sysExec(s, command);
        }
        #endregion

        #region Two Space
        void twoSpace(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            string last_val = string.Empty;

            if (StringHelper.ContainsString(arg2, "self."))
                arg2 = StringHelper.ReplaceString(arg2, "self", __CurrentMethodObject);

            if (StringHelper.ContainsString(arg0, "self."))
                arg0 = StringHelper.ReplaceString(arg0, "self", __CurrentMethodObject);

            if (VariableExists(arg0))
            {
                initializeVariable(arg0, arg1, arg2, s, command);
            }
            else if (engine.ListExists(arg0) || engine.ListExists(StringHelper.BeforeBrackets(arg0)))
            {
                initializeListValues(arg0, arg1, arg2, s, command);
            }
            else
            {
                if (StringHelper.StringStartsWith(arg0, "@") && StringHelper.ZeroDots(arg0))
                {
                    createGlobalVariable(arg0, arg1, arg2, s, command);
                }
                else if (StringHelper.StringStartsWith(arg0, "@") && !StringHelper.ZeroDots(arg2))
                {
                    createObjectVariable(arg0, arg1, arg2, s, command);
                }
                else if (!engine.ObjectExists(arg0) && engine.ObjectExists(arg2))
                {
                    CopyObject(arg0, arg1, arg2, s, command);
                }
                else if (StringHelper.IsUppercaseConstant(arg0))
                {
                    createConstant(arg0, arg1, arg2, s, command);
                }
                else
                {
                    executeSimpleStatement(arg0, arg1, arg2, s, command);
                }
            }
        }
        #endregion

        #region Three Space
        void threeSpace(string arg0, string arg1, string arg2, string arg3, string s, System.Collections.Generic.List<string> command)
        {
            // isNumber(arg3)
            // isString(arg3)

            if (arg0 == "object")
            {
                if (engine.ObjectExists(arg1))
                {
                    __DefiningObject = true;
                    __CurrentObject = arg1;
                }
                else
                {
                    if (engine.ObjectExists(arg3))
                    {
                        if (arg2 == "=")
                        {
                            System.Collections.Generic.List<Method> objectMethods = GetObjectMethodList(arg3);
                            Object newObject = new(arg1);

                            for (int i = 0; i < objectMethods.Count; i++)
                            {
                                if (objectMethods[i].IsPublic())
                                    newObject.addMethod(objectMethods[i]);
                            }

                            objects.Add(arg1, newObject);
                            __CurrentObject = arg1;
                            __DefiningObject = true;

                            newObject.clear();
                            objectMethods.Clear();
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3, false);
                }
            }
            else if (arg0 == "unless")
            {
                if (engine.ListExists(arg3))
                {
                    if (arg2 == "in")
                    {
                        string testString = ("[none]");

                        if (VariableExists(arg1))
                        {
                            if (IsStringVariable(arg1))
                                testString = GetVariableString(arg1);
                            else if (IsNumberVariable(arg1))
                                testString = StringHelper.DtoS(GetVariableNumber(arg1));
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                        }
                        else
                            testString = arg1;

                        if (testString != "[none]")
                        {
                            bool elementFound = false;
                            for (int i = 0; i < engine.GetListSize(arg3); i++)
                            {
                                if (engine.GetListLine(arg3, i) == testString)
                                {
                                    elementFound = true;
                                    setFalseIf();
                                    __LastValue = StringHelper.ItoS(i);
                                    break;
                                }
                            }

                            if (!elementFound)
                                setTrueIf();
                        }
                        else
                            setTrueIf();
                    }
                }
                else if (VariableExists(arg1) && VariableExists(arg3))
                {
                    if (IsStringVariable(arg1) && IsStringVariable(arg3))
                    {
                        if (arg2 == "==")
                        {
                            if (GetVariableString(arg1) == GetVariableString(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (GetVariableString(arg1) != GetVariableString(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (GetVariableString(arg1).Length > GetVariableString(arg3).Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (GetVariableString(arg1).Length < GetVariableString(arg3).Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (GetVariableString(arg1).Length <= GetVariableString(arg3).Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (GetVariableString(arg1).Length >= GetVariableString(arg3).Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "contains")
                        {
                            if (StringHelper.ContainsString(GetVariableString(arg1), GetVariableString(arg3)))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "ends_with")
                        {
                            if (StringHelper.StringEndsWith(GetVariableString(arg1), GetVariableString(arg3)))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "begins_with")
                        {
                            if (StringHelper.StringStartsWith(GetVariableString(arg1), GetVariableString(arg3)))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                    else if (IsNumberVariable(arg1) && IsNumberVariable(arg3))
                    {
                        if (arg2 == "==")
                        {
                            if (GetVariableNumber(arg1) == GetVariableNumber(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (GetVariableNumber(arg1) != GetVariableNumber(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (GetVariableNumber(arg1) > GetVariableNumber(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (GetVariableNumber(arg1) >= GetVariableNumber(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (GetVariableNumber(arg1) < GetVariableNumber(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (GetVariableNumber(arg1) <= GetVariableNumber(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                    else
                    {
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                        setTrueIf();
                    }
                }
                else if ((VariableExists(arg1) && !VariableExists(arg3)) && !MethodExists(arg3) && notObjectMethod(arg3) && !StringHelper.ContainsParameters(arg3))
                {
                    if (IsNumberVariable(arg1))
                    {
                        if (StringHelper.IsNumeric(arg3))
                        {
                            if (arg2 == "==")
                            {
                                if (GetVariableNumber(arg1) == StringHelper.StoD(arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (GetVariableNumber(arg1) != StringHelper.StoD(arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (GetVariableNumber(arg1) > StringHelper.StoD(arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (GetVariableNumber(arg1) < StringHelper.StoD(arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (GetVariableNumber(arg1) >= StringHelper.StoD(arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (GetVariableNumber(arg1) <= StringHelper.StoD(arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                        else if (arg3 == "number?")
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            setTrueIf();
                        }
                    }
                    else
                    {
                        if (arg3 == "string?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (arg2 == "==")
                                    setFalseIf();
                                else if (arg2 == "!=")
                                    setTrueIf();
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!")
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                        }
                        else if (arg3 == "number?")
                        {
                            if (IsNumberVariable(arg1))
                            {
                                if (arg2 == "==")
                                    setFalseIf();
                                else if (arg2 == "!=")
                                    setTrueIf();
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                        }
                        else if (arg3 == "uppercase?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (StringHelper.IsUppercase(GetVariableString(arg1)))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (StringHelper.IsUppercase(GetVariableString(arg1)))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (StringHelper.IsUppercase(arg2))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                    setTrueIf();
                            }
                        }
                        else if (arg3 == "lowercase?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (StringHelper.IsLowercase(GetVariableString(arg1)))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (StringHelper.IsLowercase(GetVariableString(arg1)))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (StringHelper.IsLowercase(arg2))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                    setTrueIf();
                            }
                        }
                        else if (arg3 == "file?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (System.IO.File.Exists(GetVariableString(arg1)))
                                {
                                    if (arg2 == "==")
                                        setFalseIf();
                                    else if (arg2 == "!=")
                                        setTrueIf();
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "!=")
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                                setTrueIf();
                            }
                        }
                        else if (arg3 == "directory?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (System.IO.Directory.Exists(GetVariableString(arg1)))
                                {
                                    if (arg2 == "==")
                                        setFalseIf();
                                    else if (arg2 == "!=")
                                        setTrueIf();
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "!=")
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                            {
                                if (GetVariableString(arg1) == arg3)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (GetVariableString(arg1) != arg3)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (GetVariableString(arg1).Length > arg3.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (GetVariableString(arg1).Length < arg3.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (GetVariableString(arg1).Length >= arg3.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (GetVariableString(arg1).Length <= arg3.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "contains")
                            {
                                if (StringHelper.ContainsString(GetVariableString(arg1), arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "ends_with")
                            {
                                if (StringHelper.StringEndsWith(GetVariableString(arg1), arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "begins_with")
                            {
                                if (StringHelper.StringStartsWith(GetVariableString(arg1), arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                    }
                }
                else if ((VariableExists(arg1) && !VariableExists(arg3)) && !MethodExists(arg3) && notObjectMethod(arg3) && StringHelper.ContainsParameters(arg3))
                {
                    string stackValue = string.Empty;

                    if (isStringStack(arg3))
                        stackValue = getStringStack(arg3);
                    else if (stackReady(arg3))
                        stackValue = StringHelper.DtoS(getStack(arg3));
                    else
                        stackValue = arg3;

                    if (IsNumberVariable(arg1))
                    {
                        if (StringHelper.IsNumeric(stackValue))
                        {
                            if (arg2 == "==")
                            {
                                if (GetVariableNumber(arg1) == StringHelper.StoD(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (GetVariableNumber(arg1) != StringHelper.StoD(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (GetVariableNumber(arg1) > StringHelper.StoD(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (GetVariableNumber(arg1) < StringHelper.StoD(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (GetVariableNumber(arg1) >= StringHelper.StoD(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (GetVariableNumber(arg1) <= StringHelper.StoD(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                        else if (stackValue == "number?")
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            setTrueIf();
                        }
                    }
                    else
                    {
                        if (stackValue == "string?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (arg2 == "==")
                                    setFalseIf();
                                else if (arg2 == "!=")
                                    setTrueIf();
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                        }
                        else if (stackValue == "number?")
                        {
                            if (IsNumberVariable(arg1))
                            {
                                if (arg2 == "==")
                                    setFalseIf();
                                else if (arg2 == "!=")
                                    setTrueIf();
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                        }
                        else if (stackValue == "uppercase?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (StringHelper.IsUppercase(GetVariableString(arg1)))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (StringHelper.IsUppercase(GetVariableString(arg1)))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (StringHelper.IsUppercase(arg2))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                    setTrueIf();
                            }
                        }
                        else if (stackValue == "lowercase?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (StringHelper.IsLowercase(GetVariableString(arg1)))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (StringHelper.IsLowercase(GetVariableString(arg1)))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (StringHelper.IsLowercase(arg2))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                    setTrueIf();
                            }
                        }
                        else if (stackValue == "file?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (System.IO.File.Exists(GetVariableString(arg1)))
                                {
                                    if (arg2 == "==")
                                        setFalseIf();
                                    else if (arg2 == "!=")
                                        setTrueIf();
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "!=")
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                                setTrueIf();
                            }
                        }
                        else if (stackValue == "directory?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (System.IO.Directory.Exists(GetVariableString(arg1)))
                                {
                                    if (arg2 == "==")
                                        setFalseIf();
                                    else if (arg2 == "!=")
                                        setTrueIf();
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "!=")
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                            {
                                if (GetVariableString(arg1) == stackValue)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (GetVariableString(arg1) != stackValue)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (GetVariableString(arg1).Length > stackValue.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (GetVariableString(arg1).Length < stackValue.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (GetVariableString(arg1).Length >= stackValue.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (GetVariableString(arg1).Length <= stackValue.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "contains")
                            {
                                if (StringHelper.ContainsString(GetVariableString(arg1), stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "ends_with")
                            {
                                if (StringHelper.StringEndsWith(GetVariableString(arg1), stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "begins_with")
                            {
                                if (StringHelper.StringStartsWith(GetVariableString(arg1), stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                    }
                }
                else if ((!VariableExists(arg1) && VariableExists(arg3)) && !MethodExists(arg1) && notObjectMethod(arg1) && !StringHelper.ContainsParameters(arg1))
                {
                    if (IsNumberVariable(arg3))
                    {
                        if (StringHelper.IsNumeric(arg1))
                        {
                            if (arg2 == "==")
                            {
                                if (GetVariableNumber(arg3) == StringHelper.StoD(arg1))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (GetVariableNumber(arg3) != StringHelper.StoD(arg1))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (GetVariableNumber(arg3) > StringHelper.StoD(arg1))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (GetVariableNumber(arg3) < StringHelper.StoD(arg1))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (GetVariableNumber(arg3) >= StringHelper.StoD(arg1))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (GetVariableNumber(arg3) <= StringHelper.StoD(arg1))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            setTrueIf();
                        }
                    }
                    else
                    {
                        if (arg2 == "==")
                        {
                            if (GetVariableString(arg3) == arg1)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (GetVariableString(arg3) != arg1)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (GetVariableString(arg3).Length > arg1.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (GetVariableString(arg3).Length < arg1.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (GetVariableString(arg3).Length >= arg1.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (GetVariableString(arg3).Length <= arg1.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                }
                else if ((!VariableExists(arg1) && VariableExists(arg3)) && !MethodExists(arg1) && notObjectMethod(arg1) && StringHelper.ContainsParameters(arg1))
                {
                    string stackValue = string.Empty;

                    if (isStringStack(arg1))
                        stackValue = getStringStack(arg1);
                    else if (stackReady(arg1))
                        stackValue = StringHelper.DtoS(getStack(arg1));
                    else
                        stackValue = arg1;

                    if (IsNumberVariable(arg3))
                    {
                        if (StringHelper.IsNumeric(stackValue))
                        {
                            if (arg2 == "==")
                            {
                                if (GetVariableNumber(arg3) == StringHelper.StoD(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (GetVariableNumber(arg3) != StringHelper.StoD(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (GetVariableNumber(arg3) > StringHelper.StoD(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (GetVariableNumber(arg3) < StringHelper.StoD(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (GetVariableNumber(arg3) >= StringHelper.StoD(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (GetVariableNumber(arg3) <= StringHelper.StoD(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            setTrueIf();
                        }
                    }
                    else
                    {
                        if (arg2 == "==")
                        {
                            if (GetVariableString(arg3) == stackValue)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (GetVariableString(arg3) != stackValue)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (GetVariableString(arg3).Length > stackValue.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (GetVariableString(arg3).Length < stackValue.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (GetVariableString(arg3).Length >= stackValue.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (GetVariableString(arg3).Length <= stackValue.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                }
                else if (StringHelper.ContainsParameters(arg1) || StringHelper.ContainsParameters(arg3))
                {
                    if (StringHelper.ContainsParameters(arg1) && StringHelper.ContainsParameters(arg3))
                    {
                        if (!StringHelper.ZeroDots(arg1) && !StringHelper.ZeroDots(arg3))
                        {
                            string arg1before = (StringHelper.BeforeDot(arg1)), arg1after = (StringHelper.AfterDot(arg1)),
                           arg3before = (StringHelper.BeforeDot(arg3)), arg3after = (StringHelper.AfterDot(arg3));

                            string arg1Result = string.Empty, arg3Result = string.Empty;

                            if (engine.ObjectExists(arg1before) && engine.ObjectExists(arg3before))
                            {
                                if (ObjectMethodExists(arg1before, StringHelper.BeforeParameters(arg1after)))
                                    executeTemplate(GetObjectMethod(arg1before, StringHelper.BeforeParameters(arg1after)), StringHelper.GetParameters(arg1after));

                                arg1Result = __LastValue;
                                
                                if (ObjectMethodExists(arg3before, StringHelper.BeforeParameters(arg3after)))
                                    executeTemplate(GetObjectMethod(arg3before, StringHelper.BeforeParameters(arg3after)), StringHelper.GetParameters(arg3after));

                                arg3Result = __LastValue;

                                if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "==")
                                    {
                                        if (arg1Result == arg3Result)
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (arg1Result != arg3Result)
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                            }
                            else
                            {
                                if (!engine.ObjectExists(arg1before))
                                    ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1before, false);

                                if (!engine.ObjectExists(arg3before))
                                    ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3before, false);

                                setTrueIf();
                            }
                        }
                        else if (!StringHelper.ZeroDots(arg1) && StringHelper.ZeroDots(arg3))
                        {
                            string arg1before = (StringHelper.BeforeDot(arg1)), arg1after = (StringHelper.AfterDot(arg1));

                            string arg1Result = string.Empty, arg3Result = string.Empty;

                            if (engine.ObjectExists(arg1before))
                            {
                                if (ObjectMethodExists(arg1before, StringHelper.BeforeParameters(arg1after)))
                                    executeTemplate(GetObjectMethod(arg1before, StringHelper.BeforeParameters(arg1after)), StringHelper.GetParameters(arg1after));

                                arg1Result = __LastValue;

                                if (MethodExists(StringHelper.BeforeParameters(arg3)))
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg3)), StringHelper.GetParameters(arg3));

                                arg3Result = __LastValue;

                                if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "==")
                                    {
                                        if (arg1Result == arg3Result)
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (arg1Result != arg3Result)
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1before, false);
                                setTrueIf();
                            }
                        }
                        else if (StringHelper.ZeroDots(arg1) && !StringHelper.ZeroDots(arg3))
                        {
                            string arg3before = (StringHelper.BeforeDot(arg3)), arg3after = (StringHelper.AfterDot(arg3));

                            string arg1Result = string.Empty, arg3Result = string.Empty;

                            if (engine.ObjectExists(arg3before))
                            {
                                if (ObjectMethodExists(arg3before, StringHelper.BeforeParameters(arg3after)))
                                    executeTemplate(GetObjectMethod(arg3before, StringHelper.BeforeParameters(arg3after)), StringHelper.GetParameters(arg3after));

                                arg3Result = __LastValue;

                                if (MethodExists(StringHelper.BeforeParameters(arg1)))
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg1)), StringHelper.GetParameters(arg1));

                                arg1Result = __LastValue;

                                if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "==")
                                    {
                                        if (arg1Result == arg3Result)
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (arg1Result != arg3Result)
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3before, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            string arg1Result = string.Empty, arg3Result = string.Empty;

                            if (MethodExists(StringHelper.BeforeParameters(arg1)))
                                executeTemplate(GetMethod(StringHelper.BeforeParameters(arg1)), StringHelper.GetParameters(arg1));

                            arg1Result = __LastValue;

                            if (MethodExists(StringHelper.BeforeParameters(arg3)))
                                executeTemplate(GetMethod(StringHelper.BeforeParameters(arg3)), StringHelper.GetParameters(arg3));

                            arg3Result = __LastValue;

                            if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                            {
                                if (arg2 == "==")
                                {
                                    if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "<")
                                {
                                    if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == ">")
                                {
                                    if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "<=")
                                {
                                    if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == ">=")
                                {
                                    if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "==")
                                {
                                    if (arg1Result == arg3Result)
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (arg1Result != arg3Result)
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                        }
                    }
                    else if (StringHelper.ContainsParameters(arg1) && !StringHelper.ContainsParameters(arg3))
                    {
                        string arg1Result = string.Empty, arg3Result = string.Empty;

                        bool pass = true;

                        if (StringHelper.ZeroDots(arg1))
                        {
                            if (MethodExists(StringHelper.BeforeParameters(arg1)))
                            {
                                executeTemplate(GetMethod(StringHelper.BeforeParameters(arg1)), StringHelper.GetParameters(arg1));

                                arg1Result = __LastValue;

                                if (MethodExists(arg3))
                                {
                                    parse(arg3);
                                    arg3Result = __LastValue;
                                }
                                else if (VariableExists(arg3))
                                {
                                    if (IsStringVariable(arg3))
                                        arg3Result = GetVariableString(arg3);
                                    else if (IsNumberVariable(arg3))
                                        arg3Result = StringHelper.DtoS(GetVariableNumber(arg3));
                                    else
                                    {
                                        pass = false;
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg3, false);
                                        setTrueIf();
                                    }
                                }
                                else
                                    arg3Result = arg3;

                                if (pass)
                                {
                                    if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "<")
                                        {
                                            if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == ">")
                                        {
                                            if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "<=")
                                        {
                                            if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == ">=")
                                        {
                                            if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setTrueIf();
                                        }
                                    }
                                    else
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (arg1Result == arg3Result)
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (arg1Result != arg3Result)
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setTrueIf();
                                        }
                                    }
                                }
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.METHOD_UNDEFINED, StringHelper.BeforeParameters(arg1), false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            string arg1before = (StringHelper.BeforeDot(arg1)), arg1after = (StringHelper.AfterDot(arg1));

                            if (engine.ObjectExists(arg1before))
                            {
                                if (ObjectMethodExists(arg1before, StringHelper.BeforeParameters(arg1after)))
                                    executeTemplate(GetObjectMethod(arg1before, StringHelper.BeforeParameters(arg1after)), StringHelper.GetParameters(arg1after));

                                arg1Result = __LastValue;

                                if (VariableExists(arg3))
                                {
                                    if (IsStringVariable(arg3))
                                        arg3Result = GetVariableString(arg3);
                                    else if (IsNumberVariable(arg3))
                                        arg3Result = StringHelper.DtoS(GetVariableNumber(arg3));
                                    else
                                    {
                                        pass = false;
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg3, false);
                                        setTrueIf();
                                    }
                                }
                                else if (MethodExists(arg3))
                                {
                                    parse(arg3);

                                    arg3Result = __LastValue;
                                }
                                else
                                    arg3Result = arg3;

                                if (pass)
                                {
                                    if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "<")
                                        {
                                            if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == ">")
                                        {
                                            if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "<=")
                                        {
                                            if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == ">=")
                                        {
                                            if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setTrueIf();
                                        }
                                    }
                                    else
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (arg1Result == arg3Result)
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (arg1Result != arg3Result)
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setTrueIf();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1before, false);
                                setTrueIf();
                            }
                        }
                    }
                    else if (!StringHelper.ContainsParameters(arg1) && StringHelper.ContainsParameters(arg3))
                    {
                        string arg1Result = string.Empty, arg3Result = string.Empty;

                        bool pass = true;

                        if (StringHelper.ZeroDots(arg3))
                        {
                            if (MethodExists(StringHelper.BeforeParameters(arg3)))
                            {
                                executeTemplate(GetMethod(StringHelper.BeforeParameters(arg3)), StringHelper.GetParameters(arg3));

                                arg3Result = __LastValue;

                                if (MethodExists(arg1))
                                {
                                    parse(arg1);
                                    arg1Result = __LastValue;
                                }
                                else if (VariableExists(arg1))
                                {
                                    if (IsStringVariable(arg1))
                                        arg1Result = GetVariableString(arg1);
                                    else if (IsNumberVariable(arg1))
                                        arg1Result = StringHelper.DtoS(GetVariableNumber(arg1));
                                    else
                                    {
                                        pass = false;
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                                        setTrueIf();
                                    }
                                }
                                else
                                    arg1Result = arg1;

                                if (pass)
                                {
                                    if (StringHelper.IsNumeric(arg3Result) && StringHelper.IsNumeric(arg1Result))
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (StringHelper.StoD(arg3Result) == StringHelper.StoD(arg1Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (StringHelper.StoD(arg3Result) != StringHelper.StoD(arg1Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "<")
                                        {
                                            if (StringHelper.StoD(arg3Result) < StringHelper.StoD(arg1Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == ">")
                                        {
                                            if (StringHelper.StoD(arg3Result) > StringHelper.StoD(arg1Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "<=")
                                        {
                                            if (StringHelper.StoD(arg3Result) <= StringHelper.StoD(arg1Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == ">=")
                                        {
                                            if (StringHelper.StoD(arg3Result) >= StringHelper.StoD(arg1Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setTrueIf();
                                        }
                                    }
                                    else
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (arg3Result == arg1Result)
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (arg3Result != arg1Result)
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setTrueIf();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.METHOD_UNDEFINED, StringHelper.BeforeParameters(arg3), false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            string arg3before = (StringHelper.BeforeDot(arg3)), arg3after = (StringHelper.AfterDot(arg3));

                            if (engine.ObjectExists(arg3before))
                            {
                                if (ObjectMethodExists(arg3before, StringHelper.BeforeParameters(arg3after)))
                                    executeTemplate(GetObjectMethod(arg3before, StringHelper.BeforeParameters(arg3after)), StringHelper.GetParameters(arg3after));

                                arg3Result = __LastValue;

                                if (VariableExists(arg1))
                                {
                                    if (IsStringVariable(arg1))
                                        arg1Result = GetVariableString(arg1);
                                    else if (IsNumberVariable(arg3))
                                        arg1Result = StringHelper.DtoS(GetVariableNumber(arg1));
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                                        setTrueIf();
                                    }
                                }
                                else if (MethodExists(arg1))
                                {
                                    parse(arg1);

                                    arg1Result = __LastValue;
                                }
                                else
                                    arg1Result = arg1;

                                if (StringHelper.IsNumeric(arg3Result) && StringHelper.IsNumeric(arg1Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (StringHelper.StoD(arg3Result) == StringHelper.StoD(arg1Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (StringHelper.StoD(arg3Result) != StringHelper.StoD(arg1Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (StringHelper.StoD(arg3Result) < StringHelper.StoD(arg1Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (StringHelper.StoD(arg3Result) > StringHelper.StoD(arg1Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (StringHelper.StoD(arg3Result) <= StringHelper.StoD(arg1Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (StringHelper.StoD(arg3Result) >= StringHelper.StoD(arg1Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "==")
                                    {
                                        if (arg3Result == arg1Result)
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (arg3Result != arg1Result)
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3before, false);
                                setTrueIf();
                            }
                        }
                    }
                }
                else if ((MethodExists(arg1) && arg3 != "method?") || MethodExists(arg3))
                {
                    string arg1Result = string.Empty, arg3Result = string.Empty;

                    if (MethodExists(arg1))
                    {
                        parse(arg1);
                        arg1Result = __LastValue;
                    }
                    else if (VariableExists(arg1))
                    {
                        if (IsStringVariable(arg1))
                            arg1Result = GetVariableString(arg1);
                        else if (IsNumberVariable(arg1))
                            arg1Result = StringHelper.DtoS(GetVariableNumber(arg1));
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                            setTrueIf();
                        }
                    }
                    else
                        arg1Result = arg1;

                    if (MethodExists(arg3))
                    {
                        parse(arg3);
                        arg3Result = __LastValue;
                    }
                    else if (VariableExists(arg3))
                    {
                        if (IsStringVariable(arg3))
                            arg3Result = GetVariableString(arg3);
                        else if (IsNumberVariable(arg3))
                            arg3Result = StringHelper.DtoS(GetVariableNumber(arg3));
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.IS_NULL, arg3, false);
                            setTrueIf();
                        }
                    }
                    else
                        arg3Result = arg3;

                    if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                    {
                        if (arg2 == "==")
                        {
                            if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                    else
                    {
                        if (arg2 == "==")
                        {
                            if (arg1Result == arg3Result)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (arg1Result != arg3Result)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                }
                else
                {
                    if (arg3 == "object?")
                    {
                        if (engine.ObjectExists(arg1))
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                    }
                    else if (arg3 == "variable?")
                    {
                        if (VariableExists(arg1))
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "=")
                                setTrueIf();
                            else if (arg2 == "!")
                                setFalseIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                    }
                    else if (arg3 == "method?")
                    {
                        if (MethodExists(arg1))
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                    }
                    else if (arg3 == "list?")
                    {
                        if (engine.ListExists(arg1))
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                    }
                    else if (arg2 == "==")
                    {
                        if (arg1 == arg3)
                            setFalseIf();
                        else
                            setTrueIf();
                    }
                    else if (arg2 == "!=")
                    {
                        if (arg1 != arg3)
                            setFalseIf();
                        else
                            setTrueIf();
                    }
                    else if (arg2 == ">")
                    {
                        if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (StringHelper.StoD(arg1) > StringHelper.StoD(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            if (arg1.Length > arg3.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                    }
                    else if (arg2 == "<")
                    {
                        if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (StringHelper.StoD(arg1) < StringHelper.StoD(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            if (arg1.Length < arg3.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                    }
                    else if (arg2 == ">=")
                    {
                        if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (StringHelper.StoD(arg1) >= StringHelper.StoD(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                    else if (arg2 == "<=")
                    {
                        if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (StringHelper.StoD(arg1) <= StringHelper.StoD(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                    else if (arg2 == "begins_with")
                    {
                        if (StringHelper.StringStartsWith(arg1, arg3))
                            setFalseIf();
                        else
                            setTrueIf();
                    }
                    else if (arg2 == "ends_with")
                    {
                        if (StringHelper.StringEndsWith(arg1, arg3))
                            setFalseIf();
                        else
                            setTrueIf();
                    }
                    else if (arg2 == "contains")
                    {
                        if (StringHelper.ContainsString(arg1, arg3))
                            setFalseIf();
                        else
                            setTrueIf();
                    }
                    else
                    {
                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                        setTrueIf();
                    }
                }
            }
            else if (arg0 == "if")
            {
                if (engine.ListExists(arg3))
                {
                    if (arg2 == "in")
                    {
                        string testString = ("[none]");

                        if (VariableExists(arg1))
                        {
                            if (IsStringVariable(arg1))
                                testString = GetVariableString(arg1);
                            else if (IsNumberVariable(arg1))
                                testString = StringHelper.DtoS(GetVariableNumber(arg1));
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                        }
                        else
                            testString = arg1;

                        if (testString != "[none]")
                        {
                            bool elementFound = false;
                            for (int i = 0; i < engine.GetListSize(arg3); i++)
                            {
                                if (engine.GetListLine(arg3, i) == testString)
                                {
                                    elementFound = true;
                                    setTrueIf();
                                    __LastValue = StringHelper.ItoS(i);
                                    break;
                                }
                            }

                            if (!elementFound)
                                setFalseIf();
                        }
                        else
                            setFalseIf();
                    }
                }
                else if (engine.ListExists(arg1) && arg3 != "list?")
                {
                    if (arg2 == "contains")
                    {
                        string testString = ("[none]");

                        if (VariableExists(arg3))
                        {
                            if (IsStringVariable(arg3))
                                testString = GetVariableString(arg3);
                            else if (IsNumberVariable(arg3))
                                testString = StringHelper.DtoS(GetVariableNumber(arg3));
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg3, false);
                        }
                        else
                            testString = arg3;

                        if (testString != "[none]")
                        {
                            bool elementFound = false;
                            for (int i = 0; i < engine.GetListSize(arg1); i++)
                            {
                                if (engine.GetListLine(arg1, i) == testString)
                                {
                                    elementFound = true;
                                    setTrueIf();
                                    __LastValue = StringHelper.ItoS(i);
                                    break;
                                }
                            }

                            if (!elementFound)
                                setFalseIf();
                        }
                        else
                            setFalseIf();
                    }
                }
                else if (VariableExists(arg1) && VariableExists(arg3))
                {
                    if (IsStringVariable(arg1) && IsStringVariable(arg3))
                    {
                        if (arg2 == "==")
                        {
                            if (GetVariableString(arg1) == GetVariableString(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (GetVariableString(arg1) != GetVariableString(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (GetVariableString(arg1).Length > GetVariableString(arg3).Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (GetVariableString(arg1).Length < GetVariableString(arg3).Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (GetVariableString(arg1).Length <= GetVariableString(arg3).Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (GetVariableString(arg1).Length >= GetVariableString(arg3).Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "contains")
                        {
                            if (StringHelper.ContainsString(GetVariableString(arg1), GetVariableString(arg3)))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "ends_with")
                        {
                            if (StringHelper.StringEndsWith(GetVariableString(arg1), GetVariableString(arg3)))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "begins_with")
                        {
                            if (StringHelper.StringStartsWith(GetVariableString(arg1), GetVariableString(arg3)))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                    else if (IsNumberVariable(arg1) && IsNumberVariable(arg3))
                    {
                        if (arg2 == "==")
                        {
                            if (GetVariableNumber(arg1) == GetVariableNumber(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (GetVariableNumber(arg1) != GetVariableNumber(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (GetVariableNumber(arg1) > GetVariableNumber(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (GetVariableNumber(arg1) >= GetVariableNumber(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (GetVariableNumber(arg1) < GetVariableNumber(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (GetVariableNumber(arg1) <= GetVariableNumber(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                    else
                    {
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                        setFalseIf();
                    }
                }
                else if ((VariableExists(arg1) && !VariableExists(arg3)) && !MethodExists(arg3) && notObjectMethod(arg3) && !StringHelper.ContainsParameters(arg3))
                {
                    if (IsNumberVariable(arg1))
                    {
                        if (StringHelper.IsNumeric(arg3))
                        {
                            if (arg2 == "==")
                            {
                                if (GetVariableNumber(arg1) == StringHelper.StoD(arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (GetVariableNumber(arg1) != StringHelper.StoD(arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (GetVariableNumber(arg1) > StringHelper.StoD(arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (GetVariableNumber(arg1) < StringHelper.StoD(arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (GetVariableNumber(arg1) >= StringHelper.StoD(arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (GetVariableNumber(arg1) <= StringHelper.StoD(arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                        else if (arg3 == "number?")
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            setFalseIf();
                        }
                    }
                    else
                    {
                        if (arg3 == "string?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (arg2 == "==")
                                    setTrueIf();
                                else if (arg2 == "!=")
                                    setFalseIf();
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                        }
                        else if (arg3 == "number?")
                        {
                            if (IsNumberVariable(arg1))
                            {
                                if (arg2 == "==")
                                    setTrueIf();
                                else if (arg2 == "!=")
                                    setFalseIf();
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                        }
                        else if (arg3 == "uppercase?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (StringHelper.IsUppercase(GetVariableString(arg1)))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (StringHelper.IsUppercase(GetVariableString(arg1)))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (StringHelper.IsUppercase(arg2))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                    setFalseIf();
                            }
                        }
                        else if (arg3 == "lowercase?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (StringHelper.IsLowercase(GetVariableString(arg1)))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (StringHelper.IsLowercase(GetVariableString(arg1)))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (StringHelper.IsLowercase(arg2))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                    setFalseIf();
                            }
                        }
                        else if (arg3 == "file?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (System.IO.File.Exists(GetVariableString(arg1)))
                                {
                                    if (arg2 == "==")
                                        setTrueIf();
                                    else if (arg2 == "!=")
                                        setFalseIf();
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "!=")
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                                setFalseIf();
                            }
                        }
                        else if (arg3 == "dir?" || arg3 == "directory?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (System.IO.Directory.Exists(GetVariableString(arg1)))
                                {
                                    if (arg2 == "==")
                                        setTrueIf();
                                    else if (arg2 == "!=")
                                        setFalseIf();
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "!=")
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                            {
                                if (GetVariableString(arg1) == arg3)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (GetVariableString(arg1) != arg3)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (GetVariableString(arg1).Length > arg3.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (GetVariableString(arg1).Length < arg3.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (GetVariableString(arg1).Length >= arg3.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (GetVariableString(arg1).Length <= arg3.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "contains")
                            {
                                if (StringHelper.ContainsString(GetVariableString(arg1), arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "ends_with")
                            {
                                if (StringHelper.StringEndsWith(GetVariableString(arg1), arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "begins_with")
                            {
                                if (StringHelper.StringStartsWith(GetVariableString(arg1), arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                    }
                }
                else if ((VariableExists(arg1) && !VariableExists(arg3)) && !MethodExists(arg3) && notObjectMethod(arg3) && StringHelper.ContainsParameters(arg3))
                {
                    string stackValue = string.Empty;

                    if (isStringStack(arg3))
                        stackValue = getStringStack(arg3);
                    else if (stackReady(arg3))
                        stackValue = StringHelper.DtoS(getStack(arg3));
                    else
                        stackValue = arg3;

                    if (IsNumberVariable(arg1))
                    {
                        if (StringHelper.IsNumeric(stackValue))
                        {
                            if (arg2 == "==")
                            {
                                if (GetVariableNumber(arg1) == StringHelper.StoD(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (GetVariableNumber(arg1) != StringHelper.StoD(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (GetVariableNumber(arg1) > StringHelper.StoD(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (GetVariableNumber(arg1) < StringHelper.StoD(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (GetVariableNumber(arg1) >= StringHelper.StoD(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (GetVariableNumber(arg1) <= StringHelper.StoD(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                        else if (stackValue == "number?")
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            setFalseIf();
                        }
                    }
                    else
                    {
                        if (stackValue == "string?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (arg2 == "==")
                                    setTrueIf();
                                else if (arg2 == "!=")
                                    setFalseIf();
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                        }
                        else if (stackValue == "number?")
                        {
                            if (IsNumberVariable(arg1))
                            {
                                if (arg2 == "==")
                                    setTrueIf();
                                else if (arg2 == "!=")
                                    setFalseIf();
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                        }
                        else if (stackValue == "uppercase?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (StringHelper.IsUppercase(GetVariableString(arg1)))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (StringHelper.IsUppercase(GetVariableString(arg1)))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (StringHelper.IsUppercase(arg2))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                    setFalseIf();
                            }
                        }
                        else if (stackValue == "lower?" || stackValue == "lowercase?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (StringHelper.IsLowercase(GetVariableString(arg1)))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (StringHelper.IsLowercase(GetVariableString(arg1)))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (StringHelper.IsLowercase(arg2))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                    setFalseIf();
                            }
                        }
                        else if (stackValue == "file?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (System.IO.File.Exists(GetVariableString(arg1)))
                                {
                                    if (arg2 == "==")
                                        setTrueIf();
                                    else if (arg2 == "!=")
                                        setFalseIf();
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "!=")
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                                setFalseIf();
                            }
                        }
                        else if (stackValue == "directory?")
                        {
                            if (IsStringVariable(arg1))
                            {
                                if (System.IO.Directory.Exists(GetVariableString(arg1)))
                                {
                                    if (arg2 == "==")
                                        setTrueIf();
                                    else if (arg2 == "!=")
                                        setFalseIf();
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "!=")
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                            {
                                if (GetVariableString(arg1) == stackValue)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (GetVariableString(arg1) != stackValue)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (GetVariableString(arg1).Length > stackValue.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (GetVariableString(arg1).Length < stackValue.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (GetVariableString(arg1).Length >= stackValue.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (GetVariableString(arg1).Length <= stackValue.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "contains")
                            {
                                if (StringHelper.ContainsString(GetVariableString(arg1), stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "ends_with")
                            {
                                if (StringHelper.StringEndsWith(GetVariableString(arg1), stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "begins_with")
                            {
                                if (StringHelper.StringStartsWith(GetVariableString(arg1), stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                    }
                }
                else if ((!VariableExists(arg1) && VariableExists(arg3)) && !MethodExists(arg1) && notObjectMethod(arg1) && !StringHelper.ContainsParameters(arg1))
                {
                    if (IsNumberVariable(arg3))
                    {
                        if (StringHelper.IsNumeric(arg1))
                        {
                            if (arg2 == "==")
                            {
                                if (GetVariableNumber(arg3) == StringHelper.StoD(arg1))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (GetVariableNumber(arg3) != StringHelper.StoD(arg1))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (GetVariableNumber(arg3) > StringHelper.StoD(arg1))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (GetVariableNumber(arg3) < StringHelper.StoD(arg1))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (GetVariableNumber(arg3) >= StringHelper.StoD(arg1))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (GetVariableNumber(arg3) <= StringHelper.StoD(arg1))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            setFalseIf();
                        }
                    }
                    else
                    {
                        if (arg2 == "==")
                        {
                            if (GetVariableString(arg3) == arg1)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (GetVariableString(arg3) != arg1)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (GetVariableString(arg3).Length > arg1.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (GetVariableString(arg3).Length < arg1.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (GetVariableString(arg3).Length >= arg1.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (GetVariableString(arg3).Length <= arg1.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                }
                else if ((!VariableExists(arg1) && VariableExists(arg3)) && !MethodExists(arg1) && notObjectMethod(arg1) && StringHelper.ContainsParameters(arg1))
                {
                    string stackValue = string.Empty;

                    if (isStringStack(arg1))
                        stackValue = getStringStack(arg1);
                    else if (stackReady(arg1))
                        stackValue = StringHelper.DtoS(getStack(arg1));
                    else
                        stackValue = arg1;

                    if (IsNumberVariable(arg3))
                    {
                        if (StringHelper.IsNumeric(stackValue))
                        {
                            if (arg2 == "==")
                            {
                                if (GetVariableNumber(arg3) == StringHelper.StoD(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (GetVariableNumber(arg3) != StringHelper.StoD(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (GetVariableNumber(arg3) > StringHelper.StoD(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (GetVariableNumber(arg3) < StringHelper.StoD(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (GetVariableNumber(arg3) >= StringHelper.StoD(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (GetVariableNumber(arg3) <= StringHelper.StoD(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            setFalseIf();
                        }
                    }
                    else
                    {
                        if (arg2 == "==")
                        {
                            if (GetVariableString(arg3) == stackValue)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (GetVariableString(arg3) != stackValue)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (GetVariableString(arg3).Length > stackValue.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (GetVariableString(arg3).Length < stackValue.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (GetVariableString(arg3).Length >= stackValue.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (GetVariableString(arg3).Length <= stackValue.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                }
                else if (StringHelper.ContainsParameters(arg1) || StringHelper.ContainsParameters(arg3))
                {
                    if (StringHelper.ContainsParameters(arg1) && StringHelper.ContainsParameters(arg3))
                    {
                        if (!StringHelper.ZeroDots(arg1) && !StringHelper.ZeroDots(arg3))
                        {
                            string arg1before = (StringHelper.BeforeDot(arg1)), arg1after = (StringHelper.AfterDot(arg1)),
                                arg3before = (StringHelper.BeforeDot(arg3)), arg3after = (StringHelper.AfterDot(arg3));

                            string arg1Result = string.Empty, arg3Result = string.Empty;

                            if (engine.ObjectExists(arg1before) && engine.ObjectExists(arg3before))
                            {
                                if (ObjectMethodExists(arg1before, StringHelper.BeforeParameters(arg1after)))
                                    executeTemplate(GetObjectMethod(arg1before, StringHelper.BeforeParameters(arg1after)), StringHelper.GetParameters(arg1after));

                                arg1Result = __LastValue;

                                if (ObjectMethodExists(arg3before, StringHelper.BeforeParameters(arg3after)))
                                    executeTemplate(GetObjectMethod(arg3before, StringHelper.BeforeParameters(arg3after)), StringHelper.GetParameters(arg3after));

                                arg3Result = __LastValue;

                                if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "==")
                                    {
                                        if (arg1Result == arg3Result)
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (arg1Result != arg3Result)
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                            }
                            else
                            {
                                if (!engine.ObjectExists(arg1before))
                                    ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1before, false);

                                if (!engine.ObjectExists(arg3before))
                                    ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3before, false);

                                setFalseIf();
                            }
                        }
                        else if (!StringHelper.ZeroDots(arg1) && StringHelper.ZeroDots(arg3))
                        {
                            string arg1before = (StringHelper.BeforeDot(arg1)), arg1after = (StringHelper.AfterDot(arg1));

                            string arg1Result = string.Empty, arg3Result = string.Empty;

                            if (engine.ObjectExists(arg1before))
                            {
                                if (ObjectMethodExists(arg1before, StringHelper.BeforeParameters(arg1after)))
                                    executeTemplate(GetObjectMethod(arg1before, StringHelper.BeforeParameters(arg1after)), StringHelper.GetParameters(arg1after));

                                arg1Result = __LastValue;

                                if (MethodExists(StringHelper.BeforeParameters(arg3)))
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg3)), StringHelper.GetParameters(arg3));

                                arg3Result = __LastValue;

                                if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "==")
                                    {
                                        if (arg1Result == arg3Result)
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (arg1Result != arg3Result)
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1before, false);
                                setFalseIf();
                            }
                        }
                        else if (StringHelper.ZeroDots(arg1) && !StringHelper.ZeroDots(arg3))
                        {
                            string arg3before = (StringHelper.BeforeDot(arg3)), arg3after = (StringHelper.AfterDot(arg3));

                            string arg1Result = string.Empty, arg3Result = string.Empty;

                            if (engine.ObjectExists(arg3before))
                            {
                                if (ObjectMethodExists(arg3before, StringHelper.BeforeParameters(arg3after)))
                                    executeTemplate(GetObjectMethod(arg3before, StringHelper.BeforeParameters(arg3after)), StringHelper.GetParameters(arg3after));

                                arg3Result = __LastValue;

                                if (MethodExists(StringHelper.BeforeParameters(arg1)))
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg1)), StringHelper.GetParameters(arg1));

                                arg1Result = __LastValue;

                                if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "==")
                                    {
                                        if (arg1Result == arg3Result)
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (arg1Result != arg3Result)
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3before, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            string arg1Result = string.Empty, arg3Result = string.Empty;

                            if (MethodExists(StringHelper.BeforeParameters(arg1)))
                                executeTemplate(GetMethod(StringHelper.BeforeParameters(arg1)), StringHelper.GetParameters(arg1));

                            arg1Result = __LastValue;

                            if (MethodExists(StringHelper.BeforeParameters(arg3)))
                                executeTemplate(GetMethod(StringHelper.BeforeParameters(arg3)), StringHelper.GetParameters(arg3));

                            arg3Result = __LastValue;

                            if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                            {
                                if (arg2 == "==")
                                {
                                    if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "<")
                                {
                                    if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == ">")
                                {
                                    if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "<=")
                                {
                                    if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == ">=")
                                {
                                    if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "==")
                                {
                                    if (arg1Result == arg3Result)
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (arg1Result != arg3Result)
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                        }
                    }
                    else if (StringHelper.ContainsParameters(arg1) && !StringHelper.ContainsParameters(arg3))
                    {
                        string arg1Result = string.Empty, arg3Result = string.Empty;

                        bool pass = true;

                        if (StringHelper.ZeroDots(arg1))
                        {
                            if (MethodExists(StringHelper.BeforeParameters(arg1)))
                            {
                                executeTemplate(GetMethod(StringHelper.BeforeParameters(arg1)), StringHelper.GetParameters(arg1));

                                arg1Result = __LastValue;

                                if (MethodExists(arg3))
                                {
                                    parse(arg3);
                                    arg3Result = __LastValue;
                                }
                                else if (VariableExists(arg3))
                                {
                                    if (IsStringVariable(arg3))
                                        arg3Result = GetVariableString(arg3);
                                    else if (IsNumberVariable(arg3))
                                        arg3Result = StringHelper.DtoS(GetVariableNumber(arg3));
                                    else
                                    {
                                        pass = false;
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg3, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                    arg3Result = arg3;

                                if (pass)
                                {
                                    if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "<")
                                        {
                                            if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == ">")
                                        {
                                            if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "<=")
                                        {
                                            if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == ">=")
                                        {
                                            if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setFalseIf();
                                        }
                                    }
                                    else
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (arg1Result == arg3Result)
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (arg1Result != arg3Result)
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setFalseIf();
                                        }
                                    }
                                }
                                else
                                    setFalseIf();
                            }
                            else if (stackReady(arg1))
                            {
                                string stackValue = string.Empty;

                                if (isStringStack(arg1))
                                    stackValue = getStringStack(arg1);
                                else
                                    stackValue = StringHelper.DtoS(getStack(arg1));

                                string comp = string.Empty;

                                if (VariableExists(arg3))
                                {
                                    if (IsStringVariable(arg3))
                                        comp = GetVariableString(arg3);
                                    else if (IsNumberVariable(arg3))
                                        comp = StringHelper.DtoS(GetVariableNumber(arg3));
                                }
                                else if (MethodExists(arg3))
                                {
                                    parse(arg3);

                                    comp = __LastValue;
                                }
                                else if (StringHelper.ContainsParameters(arg3))
                                {
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg3)), StringHelper.GetParameters(arg3));

                                    comp = __LastValue;
                                }
                                else
                                    comp = arg3;

                                if (StringHelper.IsNumeric(stackValue) && StringHelper.IsNumeric(comp))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (StringHelper.StoD(stackValue) == StringHelper.StoD(comp))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (StringHelper.StoD(stackValue) != StringHelper.StoD(comp))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (StringHelper.StoD(stackValue) < StringHelper.StoD(comp))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (StringHelper.StoD(stackValue) > StringHelper.StoD(comp))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (StringHelper.StoD(stackValue) <= StringHelper.StoD(comp))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (StringHelper.StoD(stackValue) >= StringHelper.StoD(comp))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "==")
                                    {
                                        if (stackValue == comp)
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (stackValue != comp)
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.METHOD_UNDEFINED, StringHelper.BeforeParameters(arg1), false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            string arg1before = (StringHelper.BeforeDot(arg1)), arg1after = (StringHelper.AfterDot(arg1));

                            if (engine.ObjectExists(arg1before))
                            {
                                if (ObjectMethodExists(arg1before, StringHelper.BeforeParameters(arg1after)))
                                    executeTemplate(GetObjectMethod(arg1before, StringHelper.BeforeParameters(arg1after)), StringHelper.GetParameters(arg1after));

                                arg1Result = __LastValue;

                                if (VariableExists(arg3))
                                {
                                    if (IsStringVariable(arg3))
                                        arg3Result = GetVariableString(arg3);
                                    else if (IsNumberVariable(arg3))
                                        arg3Result = StringHelper.DtoS(GetVariableNumber(arg3));
                                    else
                                    {
                                        pass = false;
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg3, false);
                                        setFalseIf();
                                    }
                                }
                                else if (MethodExists(arg3))
                                {
                                    parse(arg3);

                                    arg3Result = __LastValue;
                                }
                                else
                                    arg3Result = arg3;

                                if (pass)
                                {
                                    if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "<")
                                        {
                                            if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == ">")
                                        {
                                            if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "<=")
                                        {
                                            if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == ">=")
                                        {
                                            if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setFalseIf();
                                        }
                                    }
                                    else
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (arg1Result == arg3Result)
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (arg1Result != arg3Result)
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setFalseIf();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1before, false);
                                setFalseIf();
                            }
                        }
                    }
                    else if (!StringHelper.ContainsParameters(arg1) && StringHelper.ContainsParameters(arg3))
                    {
                        string arg1Result = string.Empty, arg3Result = string.Empty;

                        bool pass = true;

                        if (StringHelper.ZeroDots(arg3))
                        {
                            if (MethodExists(StringHelper.BeforeParameters(arg3)))
                            {
                                executeTemplate(GetMethod(StringHelper.BeforeParameters(arg3)), StringHelper.GetParameters(arg3));

                                arg3Result = __LastValue;

                                if (MethodExists(arg1))
                                {
                                    parse(arg1);
                                    arg1Result = __LastValue;
                                }
                                else if (VariableExists(arg1))
                                {
                                    if (IsStringVariable(arg1))
                                        arg1Result = GetVariableString(arg1);
                                    else if (IsNumberVariable(arg1))
                                        arg1Result = StringHelper.DtoS(GetVariableNumber(arg1));
                                    else
                                    {
                                        pass = false;
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                    arg1Result = arg1;

                                if (pass)
                                {
                                    if (StringHelper.IsNumeric(arg3Result) && StringHelper.IsNumeric(arg1Result))
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (StringHelper.StoD(arg3Result) == StringHelper.StoD(arg1Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (StringHelper.StoD(arg3Result) != StringHelper.StoD(arg1Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "<")
                                        {
                                            if (StringHelper.StoD(arg3Result) < StringHelper.StoD(arg1Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == ">")
                                        {
                                            if (StringHelper.StoD(arg3Result) > StringHelper.StoD(arg1Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "<=")
                                        {
                                            if (StringHelper.StoD(arg3Result) <= StringHelper.StoD(arg1Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == ">=")
                                        {
                                            if (StringHelper.StoD(arg3Result) >= StringHelper.StoD(arg1Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setFalseIf();
                                        }
                                    }
                                    else
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (arg3Result == arg1Result)
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (arg3Result != arg1Result)
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setFalseIf();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.METHOD_UNDEFINED, StringHelper.BeforeParameters(arg3), false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            string arg3before = (StringHelper.BeforeDot(arg3)), arg3after = (StringHelper.AfterDot(arg3));

                            if (engine.ObjectExists(arg3before))
                            {
                                if (ObjectMethodExists(arg3before, StringHelper.BeforeParameters(arg3after)))
                                    executeTemplate(GetObjectMethod(arg3before, StringHelper.BeforeParameters(arg3after)), StringHelper.GetParameters(arg3after));

                                arg3Result = __LastValue;

                                if (VariableExists(arg1))
                                {
                                    if (IsStringVariable(arg1))
                                        arg1Result = GetVariableString(arg1);
                                    else if (IsNumberVariable(arg3))
                                        arg1Result = StringHelper.DtoS(GetVariableNumber(arg1));
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                                        setFalseIf();
                                    }
                                }
                                else if (MethodExists(arg1))
                                {
                                    parse(arg1);

                                    arg1Result = __LastValue;
                                }
                                else
                                    arg1Result = arg1;

                                if (StringHelper.IsNumeric(arg3Result) && StringHelper.IsNumeric(arg1Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (StringHelper.StoD(arg3Result) == StringHelper.StoD(arg1Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (StringHelper.StoD(arg3Result) != StringHelper.StoD(arg1Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (StringHelper.StoD(arg3Result) < StringHelper.StoD(arg1Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (StringHelper.StoD(arg3Result) > StringHelper.StoD(arg1Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (StringHelper.StoD(arg3Result) <= StringHelper.StoD(arg1Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (StringHelper.StoD(arg3Result) >= StringHelper.StoD(arg1Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                {
                                    if (arg2 == "==")
                                    {
                                        if (arg3Result == arg1Result)
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (arg3Result != arg1Result)
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3before, false);
                                setFalseIf();
                            }
                        }
                    }
                }
                else if ((MethodExists(arg1) && arg3 != "method?") || MethodExists(arg3))
                {
                    string arg1Result = string.Empty, arg3Result = string.Empty;

                    if (MethodExists(arg1))
                    {
                        parse(arg1);
                        arg1Result = __LastValue;
                    }
                    else if (VariableExists(arg1))
                    {
                        if (IsStringVariable(arg1))
                            arg1Result = GetVariableString(arg1);
                        else if (IsNumberVariable(arg1))
                            arg1Result = StringHelper.DtoS(GetVariableNumber(arg1));
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                            setFalseIf();
                        }
                    }
                    else
                        arg1Result = arg1;

                    if (MethodExists(arg3))
                    {
                        parse(arg3);
                        arg3Result = __LastValue;
                    }
                    else if (VariableExists(arg3))
                    {
                        if (IsStringVariable(arg3))
                            arg3Result = GetVariableString(arg3);
                        else if (IsNumberVariable(arg3))
                            arg3Result = StringHelper.DtoS(GetVariableNumber(arg3));
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.IS_NULL, arg3, false);
                            setFalseIf();
                        }
                    }
                    else
                        arg3Result = arg3;

                    if (StringHelper.IsNumeric(arg1Result) && StringHelper.IsNumeric(arg3Result))
                    {
                        if (arg2 == "==")
                        {
                            if (StringHelper.StoD(arg1Result) == StringHelper.StoD(arg3Result))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (StringHelper.StoD(arg1Result) != StringHelper.StoD(arg3Result))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (StringHelper.StoD(arg1Result) < StringHelper.StoD(arg3Result))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (StringHelper.StoD(arg1Result) > StringHelper.StoD(arg3Result))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (StringHelper.StoD(arg1Result) <= StringHelper.StoD(arg3Result))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (StringHelper.StoD(arg1Result) >= StringHelper.StoD(arg3Result))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                    else
                    {
                        if (arg2 == "==")
                        {
                            if (arg1Result == arg3Result)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (arg1Result != arg3Result)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                }
                else
                {
                    if (arg3 == "object?")
                    {
                        if (engine.ObjectExists(arg1))
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                    }
                    else if (arg3 == "variable?")
                    {
                        if (VariableExists(arg1))
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                    }
                    else if (arg3 == "method?")
                    {
                        if (MethodExists(arg1))
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                    }
                    else if (arg3 == "list?")
                    {
                        if (engine.ListExists(arg1))
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                    }
                    else if (arg2 == "==")
                    {
                        if (arg1 == arg3)
                            setTrueIf();
                        else
                            setFalseIf();
                    }
                    else if (arg2 == "!=")
                    {
                        if (arg1 != arg3)
                            setTrueIf();
                        else
                            setFalseIf();
                    }
                    else if (arg2 == ">")
                    {
                        if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (StringHelper.StoD(arg1) > StringHelper.StoD(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            if (arg1.Length > arg3.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                    }
                    else if (arg2 == "<")
                    {
                        if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (StringHelper.StoD(arg1) < StringHelper.StoD(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            if (arg1.Length < arg3.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                    }
                    else if (arg2 == ">=")
                    {
                        if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (StringHelper.StoD(arg1) >= StringHelper.StoD(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                    else if (arg2 == "<=")
                    {
                        if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (StringHelper.StoD(arg1) <= StringHelper.StoD(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                    else if (arg2 == "begins_with")
                    {
                        if (StringHelper.StringStartsWith(arg1, arg3))
                            setTrueIf();
                        else
                            setFalseIf();
                    }
                    else if (arg2 == "ends_with")
                    {
                        if (StringHelper.StringEndsWith(arg1, arg3))
                            setTrueIf();
                        else
                            setFalseIf();
                    }
                    else if (arg2 == "contains")
                    {
                        if (StringHelper.ContainsString(arg1, arg3))
                            setTrueIf();
                        else
                            setFalseIf();
                    }
                    else
                    {
                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                        setFalseIf();
                    }
                }
            }
            else if (arg0 == "for")
            {
                if (arg2 == "<")
                {
                    if (VariableExists(arg1) && VariableExists(arg3))
                    {
                        if (IsNumberVariable(arg1) && IsNumberVariable(arg3))
                        {
                            if (GetVariableNumber(arg1) < GetVariableNumber(arg3))
                                successfulFor(GetVariableNumber(arg1), GetVariableNumber(arg3), "<");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (VariableExists(arg1) && !VariableExists(arg3))
                    {
                        if (IsNumberVariable(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (GetVariableNumber(arg1) < StringHelper.StoD(arg3))
                                successfulFor(GetVariableNumber(arg1), StringHelper.StoD(arg3), "<");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (!VariableExists(arg1) && VariableExists(arg3))
                    {
                        if (StringHelper.IsNumeric(arg1) && IsNumberVariable(arg3))
                        {
                            if (StringHelper.StoD(arg1) < GetVariableNumber(arg3))
                                successfulFor(StringHelper.StoD(arg1), GetVariableNumber(arg3), "<");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else
                    {
                        if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (StringHelper.StoD(arg1) < StringHelper.StoD(arg3))
                                successfulFor(StringHelper.StoD(arg1), StringHelper.StoD(arg3), "<");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                }
                else if (arg2 == ">")
                {
                    if (VariableExists(arg1) && VariableExists(arg3))
                    {
                        if (IsNumberVariable(arg1) && IsNumberVariable(arg3))
                        {
                            if (GetVariableNumber(arg1) > GetVariableNumber(arg3))
                                successfulFor(GetVariableNumber(arg1), GetVariableNumber(arg3), ">");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (VariableExists(arg1) && !VariableExists(arg3))
                    {
                        if (IsNumberVariable(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (GetVariableNumber(arg1) > StringHelper.StoD(arg3))
                                successfulFor(GetVariableNumber(arg1), StringHelper.StoD(arg3), ">");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (!VariableExists(arg1) && VariableExists(arg3))
                    {
                        if (StringHelper.IsNumeric(arg1) && IsNumberVariable(arg3))
                        {
                            if (StringHelper.StoD(arg1) > GetVariableNumber(arg3))
                                successfulFor(StringHelper.StoD(arg1), GetVariableNumber(arg3), ">");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else
                    {
                        if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (StringHelper.StoD(arg1) > StringHelper.StoD(arg3))
                                successfulFor(StringHelper.StoD(arg1), StringHelper.StoD(arg3), ">");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                }
                else if (arg2 == "<=")
                {
                    if (VariableExists(arg1) && VariableExists(arg3))
                    {
                        if (IsNumberVariable(arg1) && IsNumberVariable(arg3))
                        {
                            if (GetVariableNumber(arg1) <= GetVariableNumber(arg3))
                                successfulFor(GetVariableNumber(arg1), GetVariableNumber(arg3), "<=");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (VariableExists(arg1) && !VariableExists(arg3))
                    {
                        if (IsNumberVariable(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (GetVariableNumber(arg1) <= StringHelper.StoD(arg3))
                                successfulFor(GetVariableNumber(arg1), StringHelper.StoD(arg3), "<=");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (!VariableExists(arg1) && VariableExists(arg3))
                    {
                        if (StringHelper.IsNumeric(arg1) && IsNumberVariable(arg3))
                        {
                            if (StringHelper.StoD(arg1) <= GetVariableNumber(arg3))
                                successfulFor(StringHelper.StoD(arg1), GetVariableNumber(arg3), "<=");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else
                    {
                        if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (StringHelper.StoD(arg1) <= StringHelper.StoD(arg3))
                                successfulFor(StringHelper.StoD(arg1), StringHelper.StoD(arg3), "<=");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                }
                else if (arg2 == ">=")
                {
                    if (VariableExists(arg1) && VariableExists(arg3))
                    {
                        if (IsNumberVariable(arg1) && IsNumberVariable(arg3))
                        {
                            if (GetVariableNumber(arg1) >= GetVariableNumber(arg3))
                                successfulFor(GetVariableNumber(arg1), GetVariableNumber(arg3), ">=");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (VariableExists(arg1) && !VariableExists(arg3))
                    {
                        if (IsNumberVariable(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (GetVariableNumber(arg1) >= StringHelper.StoD(arg3))
                                successfulFor(GetVariableNumber(arg1), StringHelper.StoD(arg3), ">=");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (!VariableExists(arg1) && VariableExists(arg3))
                    {
                        if (StringHelper.IsNumeric(arg1) && IsNumberVariable(arg3))
                        {
                            if (StringHelper.StoD(arg1) >= GetVariableNumber(arg3))
                                successfulFor(StringHelper.StoD(arg1), GetVariableNumber(arg3), ">=");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else
                    {
                        if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                        {
                            if (StringHelper.StoD(arg1) >= StringHelper.StoD(arg3))
                                successfulFor(StringHelper.StoD(arg1), StringHelper.StoD(arg3), ">=");
                            else
                                failedFor();
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                }
                else if (arg2 == "in")
                {
                    if (arg1 == "var")
                    {
                        string before = (StringHelper.BeforeDot(arg3)), after = (StringHelper.AfterDot(arg3));

                        if (before == "args" && after == "size")
                        {
                            List newList = new();

                            for (int i = 0; i < args.Count; i++)
                                newList.add(args[i]);

                            successfulFor(newList);
                        }
                        else if (engine.ObjectExists(before) && after == "get_methods")
                        {
                            List newList = new();

                            System.Collections.Generic.List<Method> objMethods = GetObjectMethodList(before);

                            for (int i = 0; i < objMethods.Count; i++)
                                newList.add(objMethods[i].GetName());

                            successfulFor(newList);
                        }
                        else if (engine.ObjectExists(before) && after == "get_variables")
                        {
                            List newList = new();

                            System.Collections.Generic.List<Variable> objVars = GetObjectVariableList(before);

                            for (int i = 0; i < objVars.Count; i++)
                                newList.add(objVars[i].name());

                            successfulFor(newList);
                        }
                        else if (VariableExists(before) && after == "length")
                        {
                            if (IsStringVariable(before))
                            {
                                List newList = new();
                                string tempVarStr = GetVariableString(before);
                                int len = tempVarStr.Length;

                                for (int i = 0; i < len; i++)
                                {
                                    string tempStr = string.Empty;
                                    tempStr += (tempVarStr[i]);
                                    newList.add(tempStr);
                                }

                                successfulFor(newList);
                            }
                        }
                        else
                        {
                            if (before.Length != 0 && after.Length != 0)
                            {
                                if (VariableExists(before))
                                {
                                    if (after == "get_dirs")
                                    {
                                        if (System.IO.Directory.Exists(GetVariableString(before)))
                                            successfulFor(getDirectoryList(before, false));
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(before), false);
                                            failedFor();
                                        }
                                    }
                                    else if (after == "get_files")
                                    {
                                        if (System.IO.Directory.Exists(GetVariableString(before)))
                                            successfulFor(getDirectoryList(before, true));
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(before), false);
                                            failedFor();
                                        }
                                    }
                                    else if (after == "read")
                                    {
                                        if (System.IO.File.Exists(GetVariableString(before)))
                                        {
                                            List newList = new();
                                            foreach (var line in System.IO.File.ReadAllLines(GetVariableString(before)))
                                            {
                                                newList.add(line);
                                            }
                                            successfulFor(newList);
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(before), false);
                                            failedFor();
                                        }
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.METHOD_UNDEFINED, after, false);
                                        failedFor();
                                    }
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                                    failedFor();
                                }
                            }
                            else
                            {
                                if (engine.ListExists(arg3))
                                    successfulFor(engine.GetList(arg3));
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.LIST_UNDEFINED, arg3, false);
                                    failedFor();
                                }
                            }
                        }
                    }
                    else if (StringHelper.ContainsParameters(arg3))
                    {
                        System.Collections.Generic.List<string> rangeSpecifiers = new();

                        rangeSpecifiers = StringHelper.GetRange(arg3);

                        if (rangeSpecifiers.Count == 2)
                        {
                            string firstRangeSpecifier = (rangeSpecifiers[0]), lastRangeSpecifier = (rangeSpecifiers[1]);

                            if (VariableExists(firstRangeSpecifier))
                            {
                                if (IsNumberVariable(firstRangeSpecifier))
                                    firstRangeSpecifier = StringHelper.DtoS(GetVariableNumber(firstRangeSpecifier));
                                else
                                    failedFor();
                            }

                            if (VariableExists(lastRangeSpecifier))
                            {
                                if (IsNumberVariable(lastRangeSpecifier))
                                    lastRangeSpecifier = StringHelper.DtoS(GetVariableNumber(lastRangeSpecifier));
                                else
                                    failedFor();
                            }

                            if (StringHelper.IsNumeric(firstRangeSpecifier) && StringHelper.IsNumeric(lastRangeSpecifier))
                            {
                                __DefaultLoopSymbol = arg1;

                                int ifrs = StringHelper.StoI(firstRangeSpecifier), ilrs = (StringHelper.StoI(lastRangeSpecifier));

                                if (ifrs < ilrs)
                                    successfulFor(StringHelper.StoD(firstRangeSpecifier), StringHelper.StoD(lastRangeSpecifier), "<=");
                                else if (ifrs > ilrs)
                                    successfulFor(StringHelper.StoD(firstRangeSpecifier), StringHelper.StoD(lastRangeSpecifier), ">=");
                                else
                                    failedFor();
                            }
                            else
                                failedFor();
                        }
                    }
                    else if (StringHelper.ContainsBrackets(arg3))
                    {
                        string before = (StringHelper.BeforeBrackets(arg3));

                        if (VariableExists(before))
                        {
                            if (IsStringVariable(before))
                            {
                                string tempVarString = (GetVariableString(before));

                                System.Collections.Generic.List<string> range = StringHelper.GetBracketRange(arg3);

                                if (range.Count == 2)
                                {
                                    string rangeBegin = (range[0]), rangeEnd = (range[1]);

                                    if (rangeBegin.Length != 0 && rangeEnd.Length != 0)
                                    {
                                        if (StringHelper.IsNumeric(rangeBegin) && StringHelper.IsNumeric(rangeEnd))
                                        {
                                            if (StringHelper.StoI(rangeBegin) < StringHelper.StoI(rangeEnd))
                                            {
                                                if ((int)tempVarString.Length >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0)
                                                {
                                                    List newList = new("&l&i&s&t&");

                                                    for (int i = StringHelper.StoI(rangeBegin); i <= StringHelper.StoI(rangeEnd); i++)
                                                    {
                                                        string tempString = string.Empty;
                                                        tempString += (tempVarString[i]);
                                                        newList.add(tempString);
                                                    }

                                                    __DefaultLoopSymbol = arg1;

                                                    successfulFor(newList);

                                                    engine.DeleteList("&l&i&s&t&");
                                                }
                                                else
                                                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                            }
                                            else if (StringHelper.StoI(rangeBegin) > StringHelper.StoI(rangeEnd))
                                            {
                                                if ((int)tempVarString.Length >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0)
                                                {
                                                    List newList = new("&l&i&s&t&");

                                                    for (int i = StringHelper.StoI(rangeBegin); i >= StringHelper.StoI(rangeEnd); i--)
                                                    {
                                                        string tempString = string.Empty;
                                                        tempString += (tempVarString[i]);
                                                        newList.add(tempString);
                                                    }

                                                    __DefaultLoopSymbol = arg1;

                                                    successfulFor(newList);

                                                    engine.DeleteList("&l&i&s&t&");
                                                }
                                                else
                                                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                            }
                                            else
                                                ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                        }
                                        else
                                            ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, arg3, false);
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.NULL_STRING, before, false);
                                failedFor();
                            }
                        }
                    }
                    else if (engine.ListExists(arg3))
                    {
                        __DefaultLoopSymbol = arg1;
                        successfulFor(engine.GetList(arg3));
                    }
                    else if (!StringHelper.ZeroDots(arg3))
                    {
                        string _b = (StringHelper.BeforeDot(arg3)), _a = (StringHelper.AfterDot(arg3));

                        if (_b == "args" && _a == "size")
                        {
                            List newList = new();

                            __DefaultLoopSymbol = arg1;

                            for (int i = 0; i < (int)args.Count; i++)
                                newList.add(args[i]);

                            successfulFor(newList);
                        }
                        else if (_b == "env" && _a == "get_variables")
                        {
                            List newList = new();

                            newList.add("cwd");
                            newList.add("noctis");
                            newList.add("os?");
                            newList.add("user");
                            newList.add("machine");
                            newList.add("init_dir");
                            newList.add("initial_directory");
                            newList.add("am_or_pm");
                            newList.add("now");
                            newList.add("day_of_this_week");
                            newList.add("day_of_this_month");
                            newList.add("day_of_this_year");
                            newList.add("month_of_this_year");
                            newList.add("this_second");
                            newList.add("this_minute");
                            newList.add("this_hour");
                            newList.add("this_month");
                            newList.add("this_year");
                            newList.add("empty_string");
                            newList.add("empty_number");
                            newList.add("last_error");
                            newList.add("last_value");
                            newList.add("get_members");
                            newList.add("members");

                            __DefaultLoopSymbol = arg1;
                            successfulFor(newList);
                        }
                        else if (engine.ObjectExists(_b) && _a == "get_methods")
                        {
                            List newList = new();

                            System.Collections.Generic.List<Method> objMethods = GetObjectMethodList(_b);

                            for (int i = 0; i < (int)objMethods.Count; i++)
                                newList.add(objMethods[i].GetName());

                            __DefaultLoopSymbol = arg1;
                            successfulFor(newList);
                        }
                        else if (engine.ObjectExists(_b) && _a == "get_variables")
                        {
                            List newList = new();

                            System.Collections.Generic.List<Variable> objVars = GetObjectVariableList(_b);

                            for (int i = 0; i < (int)objVars.Count; i++)
                                newList.add(objVars[i].name());

                            __DefaultLoopSymbol = arg1;
                            successfulFor(newList);
                        }
                        else if (VariableExists(_b) && _a == "length")
                        {
                            if (IsStringVariable(_b))
                            {
                                __DefaultLoopSymbol = arg1;
                                List newList = new();
                                string _t = GetVariableString(_b);
                                int _l = _t.Length;

                                for (int i = 0; i < _l; i++)
                                {
                                    string tmpStr = string.Empty;
                                    tmpStr += (_t[i]);
                                    newList.add(tmpStr);
                                }

                                successfulFor(newList);
                            }
                        }
                        else
                        {
                            if (_b.Length != 0 && _a.Length != 0)
                            {
                                if (VariableExists(_b))
                                {
                                    if (_a == "get_dirs")
                                    {
                                        if (System.IO.Directory.Exists(GetVariableString(_b)))
                                        {
                                            __DefaultLoopSymbol = arg1;
                                            successfulFor(getDirectoryList(_b, false));
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(_b), false);
                                            failedFor();
                                        }
                                    }
                                    else if (_a == "get_files")
                                    {
                                        if (System.IO.Directory.Exists(GetVariableString(_b)))
                                        {
                                            __DefaultLoopSymbol = arg1;
                                            successfulFor(getDirectoryList(_b, true));
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(_b), false);
                                            failedFor();
                                        }
                                    }
                                    else if (_a == "read")
                                    {
                                        if (System.IO.File.Exists(GetVariableString(_b)))
                                        {
                                            List newList = new();

                                            foreach (var line in System.IO.File.ReadAllLines(GetVariableString(_b)))
                                            {
                                                newList.add(line);
                                            }

                                            __DefaultLoopSymbol = arg1;
                                            successfulFor(newList);
                                        }
                                        else
                                        {
                                            ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(_b), false);
                                            failedFor();
                                        }
                                    }
                                    else
                                    {
                                        ErrorLogger.Error(ErrorLogger.METHOD_UNDEFINED, _a, false);
                                        failedFor();
                                    }
                                }
                                else
                                {
                                    ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, _b, false);
                                    failedFor();
                                }
                            }
                        }
                    }
                    else
                    {
                        ErrorLogger.Error(ErrorLogger.INVALID_OP, s, false);
                        failedFor();
                    }
                }
                else
                {
                    ErrorLogger.Error(ErrorLogger.INVALID_OP, s, false);
                    failedFor();
                }
            }
            else if (arg0 == "while")
            {
                if (VariableExists(arg1) && VariableExists(arg3))
                {
                    if (IsNumberVariable(arg1) && IsNumberVariable(arg3))
                    {
                        if (arg2 == "<" || arg2 == "<=" || arg2 == ">=" || arg2 == ">" || arg2 == "==" || arg2 == "!=")
                            successfullWhile(arg1, arg2, arg3);
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OP, s, false);
                            failedWhile();
                        }
                    }
                    else
                    {
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg1 + arg2 + arg3, false);
                        failedWhile();
                    }
                }
                else if (StringHelper.IsNumeric(arg3) && VariableExists(arg1))
                {
                    if (IsNumberVariable(arg1))
                    {
                        if (arg2 == "<" || arg2 == "<=" || arg2 == ">=" || arg2 == ">" || arg2 == "==" || arg2 == "!=")
                            successfullWhile(arg1, arg2, arg3);
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.INVALID_OP, s, false);
                            failedWhile();
                        }
                    }
                    else
                    {
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg1 + arg2 + arg3, false);
                        failedWhile();
                    }
                }
                else if (StringHelper.IsNumeric(arg1) && StringHelper.IsNumeric(arg3))
                {
                    if (arg2 == "<" || arg2 == "<=" || arg2 == ">=" || arg2 == ">" || arg2 == "==" || arg2 == "!=")
                        successfullWhile(arg1, arg2, arg3);
                    else
                    {
                        ErrorLogger.Error(ErrorLogger.INVALID_OP, s, false);
                        failedWhile();
                    }
                }
                else
                {
                    ErrorLogger.Error(ErrorLogger.INVALID_OP, s, false);
                    failedWhile();
                }
            }
            else
                sysExec(s, command);
        }
        #endregion
    }
}
