namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
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
                            System.Collections.Generic.List<string> parameters = getBracketRange(command[i]);

                            if (isNumeric(parameters[0]))
                            {
                                if (args.Count - 1 >= Convert.ToInt32(parameters[0]) && Convert.ToInt32(parameters[0]) >= 0)
                                {
                                    if (parameters[0] == "0")
                                        command[i] = __CurrentScript;
                                    else
                                        command[i] = args[Convert.ToInt32(parameters[0])];
                                }
                                else
                                    error(ErrorLogger.OUT_OF_BOUNDS, command[i], false);
                            }
                            else
                                error(ErrorLogger.OUT_OF_BOUNDS, command[i], false);
                        }
                    }

                    if (__DefiningSwitchBlock)
                    {
                        if (s == "{")
                        {
                            doNothing();
                        }
                        else if (startsWith(s, "case"))
                            mainSwitch.addCase(command[1]);
                        else if (s == "default")
                            __InDefaultCase = true;
                        else if (s == "end" || s == "}")
                        {
                            string switch_value = string.Empty;

                            if (isString(__SwitchVarName))
                                switch_value = variables[indexOfVariable(__SwitchVarName)].getString();
                            else if (isNumber(__SwitchVarName))
                                switch_value = dtos(variables[indexOfVariable(__SwitchVarName)].getNumber());
                            else
                                switch_value = string.Empty;

                            Container rightCase = mainSwitch.rightCase(switch_value);

                            __InDefaultCase = false;
                            __DefiningSwitchBlock = false;

                            for (int i = 0; i < (int)rightCase.size(); i++)
                                parse(rightCase.at(i));

                            mainSwitch.clear();
                        }
                        else
                        {
                            if (__InDefaultCase)
                                mainSwitch.addToDefault(s);
                            else
                                mainSwitch.addToCase(s);
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
                            modules[indexOfModule(__CurrentModule)].add(s);
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
                            if (contains(s, "while"))
                                __DefiningLocalWhileLoop = true;

                            if (contains(s, "switch"))
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
                                            objects[indexOfObject(__CurrentObject)].addToCurrentMethod(s);
                                        else
                                            methods[methods.Count - 1].add(s);
                                    }
                                    else if (__DefiningLocalSwitchBlock)
                                    {
                                        __DefiningLocalSwitchBlock = false;

                                        if (__DefiningObject)
                                            objects[indexOfObject(__CurrentObject)].addToCurrentMethod(s);
                                        else
                                            methods[methods.Count - 1].add(s);
                                    }
                                    else
                                    {
                                        __DefiningMethod = false;

                                        if (__DefiningObject)
                                        {
                                            __DefiningObjectMethod = false;
                                            objects[objects.Count - 1].setCurrentMethod(string.Empty);
                                        }
                                    }
                                }
                                else
                                {
                                    int _len = s.Length;
                                    System.Collections.Generic.List<string> words = new();
                                    string word = (string.Empty);

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

                                    string freshLine = (string.Empty);

                                    for (int z = 0; z < words.Count; z++)
                                    {
                                        if (variableExists(words[z]))
                                        {
                                            if (isString(words[z]))
                                                freshLine += (variables[indexOfVariable(words[z])].getString());
                                            else if (isNumber(words[z]))
                                                freshLine += (dtos(variables[indexOfVariable(words[z])].getNumber()));
                                        }
                                        else
                                            freshLine += (words[z]);

                                        if (z != words.Count - 1)
                                            freshLine += (' ');
                                    }

                                    if (__DefiningObject)
                                    {
                                        objects[indexOfObject(__CurrentObject)].addToCurrentMethod(freshLine);

                                        if (__DefiningPublicCode)
                                            objects[indexOfObject(__CurrentObject)].setPublic();
                                        else if (__DefiningPrivateCode)
                                            objects[indexOfObject(__CurrentObject)].setPrivate();
                                        else
                                            objects[indexOfObject(__CurrentObject)].setPublic();
                                    }
                                    else
                                        methods[methods.Count - 1].add(freshLine);
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
                                            objects[objects.Count - 1].addToCurrentMethod(s);
                                        else
                                            methods[methods.Count - 1].add(s);
                                    }
                                    else if (__DefiningLocalSwitchBlock)
                                    {
                                        __DefiningLocalSwitchBlock = false;

                                        if (__DefiningObject)
                                            objects[objects.Count - 1].addToCurrentMethod(s);
                                        else
                                            methods[methods.Count - 1].add(s);
                                    }
                                    else
                                    {
                                        __DefiningMethod = false;

                                        if (__DefiningObject)
                                        {
                                            __DefiningObjectMethod = false;
                                            objects[objects.Count - 1].setCurrentMethod(string.Empty);
                                        }
                                    }
                                }
                                else
                                {
                                    if (__DefiningObject)
                                    {
                                        objects[objects.Count - 1].addToCurrentMethod(s);

                                        if (__DefiningPublicCode)
                                            objects[objects.Count - 1].setPublic();
                                        else if (__DefiningPrivateCode)
                                            objects[objects.Count - 1].setPrivate();
                                        else
                                            objects[objects.Count - 1].setPublic();
                                    }
                                    else
                                    {
                                        if (__DefiningObjectMethod)
                                        {
                                            objects[objects.Count - 1].addToCurrentMethod(s);

                                            if (__DefiningPublicCode)
                                                objects[objects.Count - 1].setPublic();
                                            else if (__DefiningPrivateCode)
                                                objects[objects.Count - 1].setPrivate();
                                            else
                                                objects[objects.Count - 1].setPublic();
                                        }
                                        else
                                            methods[methods.Count - 1].add(s);
                                    }
                                }
                            }
                        }
                        else if (__DefiningIfStatement)
                        {
                            if (__DefiningNest)
                            {
                                if (command[0] == "endif")
                                    executeNest(ifStatements[ifStatements.Count - 1].getNest());
                                else
                                    ifStatements[(int)ifStatements.Count - 1].inNest(s);
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
                                        if (ifStatements[i].isIF())
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
                                    ifStatements[(int)ifStatements.Count - 1].add(s);
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

                                    string v1 = whileLoops[whileLoops.Count - 1].valueOne(),
                                           v2 = whileLoops[whileLoops.Count - 1].valueTwo(),
                                           op = whileLoops[whileLoops.Count - 1].logicOperator();

                                    if (variableExists(v1) && variableExists(v2))
                                    {
                                        if (op == "==")
                                        {
                                            while (variables[indexOfVariable(v1)].getNumber() == variables[indexOfVariable(v2)].getNumber())
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
                                            while (variables[indexOfVariable(v1)].getNumber() < variables[indexOfVariable(v2)].getNumber())
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
                                            while (variables[indexOfVariable(v1)].getNumber() > variables[indexOfVariable(v2)].getNumber())
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
                                            while (variables[indexOfVariable(v1)].getNumber() <= variables[indexOfVariable(v2)].getNumber())
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
                                            while (variables[indexOfVariable(v1)].getNumber() >= variables[indexOfVariable(v2)].getNumber())
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
                                            while (variables[indexOfVariable(v1)].getNumber() != variables[indexOfVariable(v2)].getNumber())
                                            {
                                                whileLoop(whileLoops[whileLoops.Count - 1]);

                                                if (__Breaking)
                                                    break;
                                            }

                                            whileLoops.Clear();

                                            __WhileLoopCount = 0;
                                        }
                                    }
                                    else if (variableExists(v1))
                                    {
                                        if (op == "==")
                                        {
                                            while (variables[indexOfVariable(v1)].getNumber() == stoi(v2))
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
                                            while (variables[indexOfVariable(v1)].getNumber() < stoi(v2))
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
                                            while (variables[indexOfVariable(v1)].getNumber() > stoi(v2))
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
                                            while (variables[indexOfVariable(v1)].getNumber() <= stoi(v2))
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
                                            while (variables[indexOfVariable(v1)].getNumber() >= stoi(v2))
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
                                            while (variables[indexOfVariable(v1)].getNumber() != stoi(v2))
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
                                    whileLoops[whileLoops.Count - 1].add(s);
                            }
                            else if (__DefiningForLoop)
                            {
                                if (command[0] == "next" || command[0] == "endfor")
                                {
                                    __DefiningForLoop = false;

                                    for (int i = 0; i < (int)forLoops.Count; i++)
                                        if (forLoops[i].isForLoop())
                                            forLoop(forLoops[i]);

                                    forLoops.Clear();

                                    __ForLoopCount = 0;
                                }
                                else
                                {
                                    if (s == "{")
                                        doNothing();
                                    else
                                        forLoops[forLoops.Count - 1].add(s);
                                }
                            }
                            else
                            {
                                if (size == 1)
                                {
                                    if (notStandardZeroSpace(command[0]))
                                    {
                                        string before = (beforeDot(s)), after = (afterDot(s));

                                        if (before.Length != 0 && after.Length != 0)
                                        {
                                            if (objectExists(before) && after.Length != 0)
                                            {
                                                if (containsParameters(after))
                                                {
                                                    s = subtractChar(s, "\"");

                                                    if (objects[indexOfObject(before)].methodExists(beforeParameters(after)))
                                                        executeTemplate(objects[indexOfObject(before)].getMethod(beforeParameters(after)), getParameters(after));
                                                    else
                                                        sysExec(s, command);
                                                }
                                                else if (objects[indexOfObject(before)].methodExists(after))
                                                    executeMethod(objects[indexOfObject(before)].getMethod(after));
                                                else if (objects[indexOfObject(before)].variableExists(after))
                                                {
                                                    if (objects[indexOfObject(before)].getVariable(after).getString() != __Null)
                                                        writeline(objects[indexOfObject(before)].getVariable(after).getString());
                                                    else if (objects[indexOfObject(before)].getVariable(after).getNumber() != __NullNum)
                                                        writeline(dtos(objects[indexOfObject(before)].getVariable(after).getNumber()));
                                                    else
                                                        error(ErrorLogger.IS_NULL, string.Empty, false);
                                                }
                                                else if (after == "clear")
                                                    objects[indexOfObject(before)].clear();
                                                else
                                                    error(ErrorLogger.UNDEFINED, string.Empty, false);
                                            }
                                            else
                                            {
                                                if (before == "env")
                                                {
                                                    InternalGetEnv(string.Empty, after, 3);
                                                }
                                                else if (variableExists(before))
                                                {
                                                    if (after == "clear")
                                                        parse(before + " = __Null");
                                                }
                                                else if (listExists(before))
                                                {
                                                    // REFACTOR HERE
                                                    if (after == "clear")
                                                        lists[indexOfList(before)].clear();
                                                    else if (after == "sort")
                                                        lists[indexOfList(before)].listSort();
                                                    else if (after == "reverse")
                                                        lists[indexOfList(before)].listReverse();
                                                    else if (after == "revert")
                                                        lists[indexOfList(before)].listRevert();
                                                }
                                                else if (before == "self")
                                                {
                                                    if (__ExecutedMethod)
                                                        executeMethod(objects[indexOfObject(__CurrentMethodObject)].getMethod(after));
                                                }
                                                else
                                                    sysExec(s, command);
                                            }
                                        }
                                        else if (endsWith(s, "::"))
                                        {
                                            if (__CurrentScript != string.Empty)
                                            {
                                                string newMark = (s);
                                                newMark = subtractString(s, "::");
                                                scripts[indexOfScript(__CurrentScript)].addMark(newMark);
                                            }
                                        }
                                        else if (methodExists(s))
                                            executeMethod(getMethod(s));
                                        else if (startsWith(s, "[") && endsWith(s, "]"))
                                        {
                                            InternalCreateModule(s);
                                        }
                                        else
                                        {
                                            s = subtractChar(s, "\"");

                                            if (methodExists(beforeParameters(s)))
                                                executeTemplate(getMethod(beforeParameters(s)), getParameters(s));
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
                                            redefine(command[1], command[2]);
                                        else if (command[0] == "loop")
                                        {
                                            if (containsParameters(command[2]))
                                            {
                                                __DefaultLoopSymbol = command[2];
                                                __DefaultLoopSymbol = subtractChar(__DefaultLoopSymbol, "(");
                                                __DefaultLoopSymbol = subtractChar(__DefaultLoopSymbol, ")");

                                                oneSpace(command[0], command[1], subtractString(s, command[2]), command);
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
                                        if (containsParameters(command[4]))
                                        {
                                            __DefaultLoopSymbol = command[4];
                                            __DefaultLoopSymbol = subtractChar(__DefaultLoopSymbol, "(");
                                            __DefaultLoopSymbol = subtractChar(__DefaultLoopSymbol, ")");

                                            threeSpace(command[0], command[1], command[2], command[3], subtractString(s, command[4]), command);
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
                            string commentString = (string.Empty);

                            bool commentFound = false;

                            for (int i = 0; i < (int)bigString.Length; i++)
                            {
                                if (bigString[i] == '#')
                                    commentFound = true;

                                if (!commentFound)
                                    commentString += (bigString[i]);
                            }

                            parse(trimLeadingWhitespace(commentString));
                        }
                        else
                        {
                            string commentString = (string.Empty);

                            bool commentFound = false;

                            for (int i = 0; i < (int)bigString.Length; i++)
                            {
                                if (bigString[i] == '#')
                                    commentFound = true;

                                if (!commentFound)
                                    commentString += (bigString[i]);
                            }

                            stringContainer.add(trimLeadingWhitespace(commentString));

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
                clearMethods();
            else if (arg0 == "clear_objects!")
                clearObjects();
            else if (arg0 == "clear_variables!")
                clearVariables();
            else if (arg0 == "clear_lists!")
                clearLists();
            else if (arg0 == "clear_all!")
                clearAll();
            else if (arg0 == "clear_constants!")
                clearConstants();
            else if (arg0 == "exit")
            {
                clearAll();
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
            string before = (beforeDot(arg1)), after = (afterDot(arg1));

            if (contains(arg1, "self."))
            {
                arg1 = replace(arg1, "self", __CurrentMethodObject);
            }

            if (arg0 == "return")
            {
                if (!InternalReturn(arg0, arg1, before, after))
                    oneSpace("return", arg1, "return " + arg1, command);
            }
            else if (arg0 == "switch")
            {
                if (variableExists(arg1))
                {
                    __DefiningSwitchBlock = true;
                    __SwitchVarName = arg1;
                }
                else
                    error(ErrorLogger.VAR_UNDEFINED, arg1, false);
            }
            else if (arg0 == "goto")
            {
                if (__CurrentScript != string.Empty)
                {
                    if (scripts[indexOfScript(__CurrentScript)].markExists(arg1))
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
                if (variableExists(arg1))
                {
                    // can we can assume that arg1 belongs to an object?
                    if (!zeroDots(arg1))
                    {
                        string objName = (beforeDot(arg1)), varName = (afterDot(arg1));
                        Variable tmpVar = getObject(objName).getVariable(varName);

                        if (isString(tmpVar))
                        {
                            tmpValue = tmpVar.getString();
                        }
                        else if (isNumber(tmpVar))
                        {
                            tmpValue = dtos(tmpVar.getNumber());
                        }
                        else
                        {
                            error(ErrorLogger.IS_NULL, arg1, true);
                        }
                    }
                    else
                    {
                        if (isString(arg1))
                        {
                            tmpValue = getVariable(arg1).getString();
                        }
                        else if (isNumber(arg1))
                        {
                            tmpValue = Convert.ToString(getVariable(arg1).getNumber());
                        }
                        else
                        {
                            error(ErrorLogger.IS_NULL, arg1, true);
                        }
                    }
                }
                else
                {
                    if (isNumeric(arg1) || isTrue(arg1) || isFalse(arg1))
                    {
                        tmpValue = arg1;
                    }
                    else
                    {
                        string tmpCode = (string.Empty);

                        if (startsWith(arg1, "(\"") && endsWith(arg1, "\")"))
                        {
                            tmpCode = getInner(arg1, 2, arg1.Length - 3);
                        }
                        else
                        {
                            tmpCode = arg1;
                        }
                        tmpValue = getParsedOutput(tmpCode);
                    }
                }

                if (isTrue(tmpValue))
                {
                    setTrueIf();
                }
                else if (isFalse(tmpValue))
                {
                    setFalseIf();
                }
                else
                {
                    error(ErrorLogger.INVALID_OP, arg1, true);
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
                if (variableExists(arg1))
                {
                    if (isString(arg1))
                        cerr = variables[indexOfVariable(arg1)].getString() + System.Environment.NewLine;
                    else if (isNumber(arg1))
                        cerr = variables[indexOfVariable(arg1)].getNumber() + System.Environment.NewLine;
                    else
                        error(ErrorLogger.IS_NULL, arg1, false);
                }
                else
                    cerr = arg1 + System.Environment.NewLine;
            }
            else if (arg0 == "delay")
            {
                if (isNumeric(arg1))
                    delay(stoi(arg1));
                else
                    error(ErrorLogger.CONV_ERR, arg1, false);
            }
            else if (arg0 == "loop")
                threeSpace("for", "var", "in", arg1, "for var in " + arg1, command); // REFACTOR HERE
            else if (arg0 == "for" && arg1 == "infinity")
                successfulFor();
            else if (arg0 == "remove")
            {
                if (containsParameters(arg1))
                {
                    System.Collections.Generic.List<string> parameters = getParameters(arg1);

                    for (int i = 0; i < parameters.Count; i++)
                    {
                        if (variableExists(parameters[i]))
                            variables = removeVariable(variables, parameters[i]);
                        else if (listExists(parameters[i]))
                            lists = removeList(lists, parameters[i]);
                        else if (objectExists(parameters[i]))
                            objects = removeObject(objects, parameters[i]);
                        else if (methodExists(parameters[i]))
                            methods = removeMethod(methods, parameters[i]);
                        else
                            error(ErrorLogger.TARGET_UNDEFINED, parameters[i], false);
                    }
                }
                else if (variableExists(arg1))
                    variables = removeVariable(variables, arg1);
                else if (listExists(arg1))
                    lists = removeList(lists, arg1);
                else if (objectExists(arg1))
                    objects = removeObject(objects, arg1);
                else if (methodExists(arg1))
                    methods = removeMethod(methods, arg1);
                else
                    error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
            }
            else if (arg0 == "see_string")
            {
                if (variableExists(arg1))
                    write(variables[indexOfVariable(arg1)].getString());
                else
                    error(ErrorLogger.VAR_UNDEFINED, arg1, false);
            }
            else if (arg0 == "see_number")
            {
                if (variableExists(arg1))
                    write(dtos(variables[indexOfVariable(arg1)].getNumber()));
                else
                    error(ErrorLogger.VAR_UNDEFINED, arg1, false);
            }
            else if (arg0 == "__begin__")
            {
                if (variableExists(arg1))
                {
                    if (isString(arg1))
                    {
                        if (!System.IO.File.Exists(variables[indexOfVariable(arg1)].getString()))
                        {
                            createFile(variables[indexOfVariable(arg1)].getString());
                            __DefiningScript = true;
                            __CurrentScriptName = variables[indexOfVariable(arg1)].getString();
                        }
                        else
                            error(ErrorLogger.FILE_EXISTS, variables[indexOfVariable(arg1)].getString(), false);
                    }
                }
                else if (!System.IO.File.Exists(arg1))
                {
                    createFile(arg1);
                    __DefiningScript = true;
                    __CurrentScriptName = arg1;
                }
                else
                    error(ErrorLogger.FILE_EXISTS, arg1, false);
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
                    if (isScript(arg1))
                    {
                        __PreviousScript = __CurrentScript;
                        loadScript(arg1);
                    }
                    else
                        error(ErrorLogger.BAD_LOAD, arg1, true);
                }
                else if (moduleExists(arg1))
                {
                    System.Collections.Generic.List<string> lines = modules[indexOfModule(arg1)].get();

                    for (int i = 0; i < lines.Count; i++)
                        parse(lines[i]);
                }
                else
                    error(ErrorLogger.BAD_LOAD, arg1, true);
            }
            else if (arg0 == "say" || arg0 == "stdout" || arg0 == "out" || arg0 == "print" || arg0 == "println")
            {
                InternalOutput(arg0, arg1);
            }
            else if (arg0 == "cd" || arg0 == "chdir")
            {
                if (variableExists(arg1))
                {
                    if (isString(arg1))
                    {
                        if (System.IO.Directory.Exists(variables[indexOfVariable(arg1)].getString()))
                            System.Environment.CurrentDirectory = (variables[indexOfVariable(arg1)].getString());
                        else
                            error(ErrorLogger.READ_FAIL, variables[indexOfVariable(arg1)].getString(), false);
                    }
                    else
                        error(ErrorLogger.NULL_STRING, arg1, false);
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
                if (listExists(arg1))
                    lists[indexOfList(arg1)].clear();
                else
                {
                    List newList = new(arg1);

                    if (__ExecutedTemplate || __ExecutedMethod)
                        newList.collect();
                    else
                        newList.dontCollect();

                    lists.Add(newList);
                }
            }
            else if (arg0 == "!")
            {
                if (variableExists(arg1))
                {
                    if (isString(arg1))
                        parse(variables[indexOfVariable(arg1)].getString());
                    else
                        error(ErrorLogger.IS_NULL, arg1, false);
                }
                else
                    parse(arg1);
            }
            else if (arg0 == "?")
            {
                if (variableExists(arg1))
                {
                    if (isString(arg1))
                        sysExec(variables[indexOfVariable(arg1)].getString(), command);
                    else
                        error(ErrorLogger.IS_NULL, arg1, false);
                }
                else
                    sysExec(arg1, command);
            }
            else if (arg0 == "init_dir" || arg0 == "initial_directory")
            {
                if (variableExists(arg1))
                {
                    if (isString(arg1))
                    {
                        if (System.IO.Directory.Exists(variables[indexOfVariable(arg1)].getString()))
                        {
                            __InitialDirectory = variables[indexOfVariable(arg1)].getString();
                            System.Environment.CurrentDirectory = (__InitialDirectory);
                        }
                        else
                            error(ErrorLogger.READ_FAIL, __InitialDirectory, false);
                    }
                    else
                        error(ErrorLogger.NULL_STRING, arg1, false);
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
                        error(ErrorLogger.READ_FAIL, __InitialDirectory, false);
                }
            }
            else if (arg0 == "method?")
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (objects[indexOfObject(before)].methodExists(after))
                        __true();
                    else
                        __false();
                }
                else
                {
                    if (methodExists(arg1))
                        __true();
                    else
                        __false();
                }
            }
            else if (arg0 == "object?")
            {
                if (objectExists(arg1))
                    __true();
                else
                    __false();
            }
            else if (arg0 == "variable?")
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (objects[indexOfObject(before)].variableExists(after))
                        __true();
                    else
                        __false();
                }
                else
                {
                    if (variableExists(arg1))
                        __true();
                    else
                        __false();
                }
            }
            else if (arg0 == "list?")
            {
                if (listExists(arg1))
                    __true();
                else
                    __false();
            }
            else if (arg0 == "directory?")
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (objects[indexOfObject(before)].variableExists(after))
                    {
                        if (System.IO.Directory.Exists(objects[indexOfObject(before)].getVariable(after).getString()))
                            __true();
                        else
                            __false();
                    }
                    else
                        error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                {
                    if (variableExists(arg1))
                    {
                        if (isString(arg1))
                        {
                            if (System.IO.Directory.Exists(variables[indexOfVariable(arg1)].getString()))
                                __true();
                            else
                                __false();
                        }
                        else
                            error(ErrorLogger.NULL_STRING, arg1, false);
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
                    if (objects[indexOfObject(before)].variableExists(after))
                    {
                        if (System.IO.File.Exists(objects[indexOfObject(before)].getVariable(after).getString()))
                            __true();
                        else
                            __false();
                    }
                    else
                        error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                {
                    if (variableExists(arg1))
                    {
                        if (isString(arg1))
                        {
                            if (System.IO.File.Exists(variables[indexOfVariable(arg1)].getString()))
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
                if (variableExists(arg1))
                {
                    if (variables[indexOfVariable(arg1)].garbage())
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
                    if (objects[indexOfObject(before)].variableExists(after))
                    {
                        if (objects[indexOfObject(before)].getVariable(after).getNumber() != __NullNum)
                            __true();
                        else
                            __false();
                    }
                    else
                        error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                {
                    if (variableExists(arg1))
                    {
                        if (isNumber(arg1))
                            __true();
                        else
                            __false();
                    }
                    else
                    {
                        if (isNumeric(arg1))
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
                    if (objects[indexOfObject(before)].variableExists(after))
                    {
                        if (objects[indexOfObject(before)].getVariable(after).getString() != __Null)
                            __true();
                        else
                            __false();
                    }
                    else
                        error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                {
                    if (variableExists(arg1))
                    {
                        if (isString(arg1))
                            __true();
                        else
                            __false();
                    }
                    else
                    {
                        if (isNumeric(arg1))
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
                    if (objects[indexOfObject(before)].variableExists(after))
                    {
                        if (isUpper(objects[indexOfObject(before)].getVariable(after).getString()))
                            __true();
                        else
                            __false();
                    }
                    else
                        error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                {
                    if (variableExists(arg1))
                    {
                        if (isString(arg1))
                        {
                            if (isUpper(variables[indexOfVariable(arg1)].getString()))
                                __true();
                            else
                                __false();
                        }
                        else
                            __false();
                    }
                    else
                    {
                        if (isNumeric(arg1))
                            __false();
                        else
                        {
                            if (isUpper(arg1))
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
                    if (objects[indexOfObject(before)].variableExists(after))
                    {
                        if (isLower(objects[indexOfObject(before)].getVariable(after).getString()))
                            __true();
                        else
                            __false();
                    }
                    else
                        error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                {
                    if (variableExists(arg1))
                    {
                        if (isString(arg1))
                        {
                            if (isLower(variables[indexOfVariable(arg1)].getString()))
                                __true();
                            else
                                __false();
                        }
                        else
                            __false();
                    }
                    else
                    {
                        if (isNumeric(arg1))
                            __false();
                        else
                        {
                            if (isLower(arg1))
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
                if (methodExists(arg1))
                    error(ErrorLogger.METHOD_DEFINED, arg1, false);
                else
                {
                    if (containsParameters(arg1))
                    {
                        System.Collections.Generic.List<string> parameters = getParameters(arg1);
                        Method method = new(beforeParameters(arg1), true);

                        method.setTemplateSize(parameters.Count);

                        methods.Add(method);

                        __DefiningMethod = true;
                    }
                }
            }
            else if (arg0 == "lock")
            {
                if (variableExists(arg1))
                    variables[indexOfVariable(arg1)].setIndestructible();
                else if (methodExists(arg1))
                    methods[indexOfMethod(arg1)].setIndestructible();
            }
            else if (arg0 == "unlock")
            {
                if (variableExists(arg1))
                    variables[indexOfVariable(arg1)].setDestructible();
                else if (methodExists(arg1))
                    methods[indexOfMethod(arg1)].setDestructible();
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
                if (variableExists(arg1))
                {
                    if (isString(arg1))
                    {
                        if (!System.IO.File.Exists(variables[indexOfVariable(arg1)].getString()))
                            createFile(variables[indexOfVariable(arg1)].getString());
                        else
                            error(ErrorLogger.FILE_EXISTS, variables[indexOfVariable(arg1)].getString(), false);
                    }
                    else
                        error(ErrorLogger.NULL_STRING, arg1, false);
                }
                else
                {
                    if (!System.IO.File.Exists(arg1))
                        createFile(arg1);
                    else
                        error(ErrorLogger.FILE_EXISTS, arg1, false);
                }
            }
            else if (arg0 == "fpop")
            {
                if (variableExists(arg1))
                {
                    if (isString(arg1))
                    {
                        if (System.IO.File.Exists(variables[indexOfVariable(arg1)].getString()))
                            System.IO.File.Delete(variables[indexOfVariable(arg1)].getString());
                        else
                            error(ErrorLogger.FILE_NOT_FOUND, variables[indexOfVariable(arg1)].getString(), false);
                    }
                    else
                        error(ErrorLogger.NULL_STRING, arg1, false);
                }
                else
                {
                    if (System.IO.File.Exists(arg1))
                        System.IO.File.Delete(arg1);
                    else
                        error(ErrorLogger.FILE_NOT_FOUND, arg1, false);
                }
            }
            else if (arg0 == "dpush")
            {
                if (variableExists(arg1))
                {
                    if (isString(arg1))
                    {
                        if (!System.IO.Directory.Exists(variables[indexOfVariable(arg1)].getString()))
                            System.IO.Directory.CreateDirectory(variables[indexOfVariable(arg1)].getString());
                        else
                            error(ErrorLogger.DIR_EXISTS, variables[indexOfVariable(arg1)].getString(), false);
                    }
                    else
                        error(ErrorLogger.NULL_STRING, arg1, false);
                }
                else
                {
                    if (!System.IO.Directory.Exists(arg1))
                        System.IO.Directory.CreateDirectory(arg1);
                    else
                        error(ErrorLogger.DIR_EXISTS, arg1, false);
                }
            }
            else if (arg0 == "dpop")
            {
                if (variableExists(arg1))
                {
                    if (isString(arg1))
                    {
                        if (System.IO.Directory.Exists(variables[indexOfVariable(arg1)].getString()))
                            System.IO.Directory.Delete(variables[indexOfVariable(arg1)].getString());
                        else
                            error(ErrorLogger.DIR_NOT_FOUND, variables[indexOfVariable(arg1)].getString(), false);
                    }
                    else
                        error(ErrorLogger.NULL_STRING, arg1, false);
                }
                else
                {
                    if (System.IO.Directory.Exists(arg1))
                        System.IO.Directory.Delete(arg1);
                    else
                        error(ErrorLogger.DIR_NOT_FOUND, arg1, false);
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

            if (contains(arg2, "self."))
                arg2 = replace(arg2, "self", __CurrentMethodObject);

            if (contains(arg0, "self."))
                arg0 = replace(arg0, "self", __CurrentMethodObject);

            if (variableExists(arg0))
            {
                initializeVariable(arg0, arg1, arg2, s, command);
            }
            else if (listExists(arg0) || listExists(beforeBrackets(arg0)))
            {
                initializeListValues(arg0, arg1, arg2, s, command);
            }
            else
            {
                if (startsWith(arg0, "@") && zeroDots(arg0))
                {
                    createGlobalVariable(arg0, arg1, arg2, s, command);
                }
                else if (startsWith(arg0, "@") && !zeroDots(arg2))
                {
                    createObjectVariable(arg0, arg1, arg2, s, command);
                }
                else if (!objectExists(arg0) && objectExists(arg2))
                {
                    copyObject(arg0, arg1, arg2, s, command);
                }
                else if (isUpperConstant(arg0))
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
                if (objectExists(arg1))
                {
                    __DefiningObject = true;
                    __CurrentObject = arg1;
                }
                else
                {
                    if (objectExists(arg3))
                    {
                        if (arg2 == "=")
                        {
                            System.Collections.Generic.List<Method> objectMethods = objects[indexOfObject(arg3)].getMethods();
                            Object newObject = new(arg1);

                            for (int i = 0; i < objectMethods.Count; i++)
                            {
                                if (objectMethods[i].isPublic())
                                    newObject.addMethod(objectMethods[i]);
                            }

                            objects.Add(newObject);
                            __CurrentObject = arg1;
                            __DefiningObject = true;

                            newObject.clear();
                            objectMethods.Clear();
                        }
                        else
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                    }
                    else
                        error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3, false);
                }
            }
            else if (arg0 == "unless")
            {
                if (listExists(arg3))
                {
                    if (arg2 == "in")
                    {
                        string testString = ("[none]");

                        if (variableExists(arg1))
                        {
                            if (isString(arg1))
                                testString = variables[indexOfVariable(arg1)].getString();
                            else if (isNumber(arg1))
                                testString = dtos(variables[indexOfVariable(arg1)].getNumber());
                            else
                                error(ErrorLogger.IS_NULL, arg1, false);
                        }
                        else
                            testString = arg1;

                        if (testString != "[none]")
                        {
                            bool elementFound = false;
                            for (int i = 0; i < (int)lists[indexOfList(arg3)].size(); i++)
                            {
                                if (lists[indexOfList(arg3)].at(i) == testString)
                                {
                                    elementFound = true;
                                    setFalseIf();
                                    __LastValue = itos(i);
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
                else if (variableExists(arg1) && variableExists(arg3))
                {
                    if (isString(arg1) && isString(arg3))
                    {
                        if (arg2 == "==")
                        {
                            if (variables[indexOfVariable(arg1)].getString() == variables[indexOfVariable(arg3)].getString())
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (variables[indexOfVariable(arg1)].getString() != variables[indexOfVariable(arg3)].getString())
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (variables[indexOfVariable(arg1)].getString().Length > variables[indexOfVariable(arg3)].getString().Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (variables[indexOfVariable(arg1)].getString().Length < variables[indexOfVariable(arg3)].getString().Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (variables[indexOfVariable(arg1)].getString().Length <= variables[indexOfVariable(arg3)].getString().Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (variables[indexOfVariable(arg1)].getString().Length >= variables[indexOfVariable(arg3)].getString().Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "contains")
                        {
                            if (contains(variables[indexOfVariable(arg1)].getString(), variables[indexOfVariable(arg3)].getString()))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "ends_with")
                        {
                            if (endsWith(variables[indexOfVariable(arg1)].getString(), variables[indexOfVariable(arg3)].getString()))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "begins_with")
                        {
                            if (startsWith(variables[indexOfVariable(arg1)].getString(), variables[indexOfVariable(arg3)].getString()))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                    else if (isNumber(arg1) && isNumber(arg3))
                    {
                        if (arg2 == "==")
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() == variables[indexOfVariable(arg3)].getNumber())
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() != variables[indexOfVariable(arg3)].getNumber())
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() > variables[indexOfVariable(arg3)].getNumber())
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() >= variables[indexOfVariable(arg3)].getNumber())
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() < variables[indexOfVariable(arg3)].getNumber())
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() <= variables[indexOfVariable(arg3)].getNumber())
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                    else
                    {
                        error(ErrorLogger.CONV_ERR, s, false);
                        setTrueIf();
                    }
                }
                else if ((variableExists(arg1) && !variableExists(arg3)) && !methodExists(arg3) && notObjectMethod(arg3) && !containsParameters(arg3))
                {
                    if (isNumber(arg1))
                    {
                        if (isNumeric(arg3))
                        {
                            if (arg2 == "==")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() == stod(arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() != stod(arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() > stod(arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() < stod(arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() >= stod(arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() <= stod(arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            setTrueIf();
                        }
                    }
                    else
                    {
                        if (arg3 == "string?")
                        {
                            if (isString(arg1))
                            {
                                if (arg2 == "==")
                                    setFalseIf();
                                else if (arg2 == "!=")
                                    setTrueIf();
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                            if (isNumber(arg1))
                            {
                                if (arg2 == "==")
                                    setFalseIf();
                                else if (arg2 == "!=")
                                    setTrueIf();
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                            if (isString(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (isUpper(variables[indexOfVariable(arg1)].getString()))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (isUpper(variables[indexOfVariable(arg1)].getString()))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (isUpper(arg2))
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
                            if (isString(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (isLower(variables[indexOfVariable(arg1)].getString()))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (isLower(variables[indexOfVariable(arg1)].getString()))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (isLower(arg2))
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
                            if (isString(arg1))
                            {
                                if (System.IO.File.Exists(variables[indexOfVariable(arg1)].getString()))
                                {
                                    if (arg2 == "==")
                                        setFalseIf();
                                    else if (arg2 == "!=")
                                        setTrueIf();
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.IS_NULL, arg1, false);
                                setTrueIf();
                            }
                        }
                        else if (arg3 == "directory?")
                        {
                            if (isString(arg1))
                            {
                                if (System.IO.Directory.Exists(variables[indexOfVariable(arg1)].getString()))
                                {
                                    if (arg2 == "==")
                                        setFalseIf();
                                    else if (arg2 == "!=")
                                        setTrueIf();
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.IS_NULL, arg1, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                            {
                                if (variables[indexOfVariable(arg1)].getString() == arg3)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (variables[indexOfVariable(arg1)].getString() != arg3)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length > arg3.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length < arg3.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length >= arg3.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length <= arg3.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "contains")
                            {
                                if (contains(variables[indexOfVariable(arg1)].getString(), arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "ends_with")
                            {
                                if (endsWith(variables[indexOfVariable(arg1)].getString(), arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "begins_with")
                            {
                                if (startsWith(variables[indexOfVariable(arg1)].getString(), arg3))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                    }
                }
                else if ((variableExists(arg1) && !variableExists(arg3)) && !methodExists(arg3) && notObjectMethod(arg3) && containsParameters(arg3))
                {
                    string stackValue = (string.Empty);

                    if (isStringStack(arg3))
                        stackValue = getStringStack(arg3);
                    else if (stackReady(arg3))
                        stackValue = dtos(getStack(arg3));
                    else
                        stackValue = arg3;

                    if (isNumber(arg1))
                    {
                        if (isNumeric(stackValue))
                        {
                            if (arg2 == "==")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() == stod(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() != stod(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() > stod(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() < stod(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() >= stod(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() <= stod(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            setTrueIf();
                        }
                    }
                    else
                    {
                        if (stackValue == "string?")
                        {
                            if (isString(arg1))
                            {
                                if (arg2 == "==")
                                    setFalseIf();
                                else if (arg2 == "!=")
                                    setTrueIf();
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                            if (isNumber(arg1))
                            {
                                if (arg2 == "==")
                                    setFalseIf();
                                else if (arg2 == "!=")
                                    setTrueIf();
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                            if (isString(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (isUpper(variables[indexOfVariable(arg1)].getString()))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (isUpper(variables[indexOfVariable(arg1)].getString()))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (isUpper(arg2))
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
                            if (isString(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (isLower(variables[indexOfVariable(arg1)].getString()))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (isLower(variables[indexOfVariable(arg1)].getString()))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (isLower(arg2))
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
                            if (isString(arg1))
                            {
                                if (System.IO.File.Exists(variables[indexOfVariable(arg1)].getString()))
                                {
                                    if (arg2 == "==")
                                        setFalseIf();
                                    else if (arg2 == "!=")
                                        setTrueIf();
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.IS_NULL, arg1, false);
                                setTrueIf();
                            }
                        }
                        else if (stackValue == "directory?")
                        {
                            if (isString(arg1))
                            {
                                if (System.IO.Directory.Exists(variables[indexOfVariable(arg1)].getString()))
                                {
                                    if (arg2 == "==")
                                        setFalseIf();
                                    else if (arg2 == "!=")
                                        setTrueIf();
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.IS_NULL, arg1, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                            {
                                if (variables[indexOfVariable(arg1)].getString() == stackValue)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (variables[indexOfVariable(arg1)].getString() != stackValue)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length > stackValue.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length < stackValue.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length >= stackValue.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length <= stackValue.Length)
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "contains")
                            {
                                if (contains(variables[indexOfVariable(arg1)].getString(), stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "ends_with")
                            {
                                if (endsWith(variables[indexOfVariable(arg1)].getString(), stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "begins_with")
                            {
                                if (startsWith(variables[indexOfVariable(arg1)].getString(), stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                    }
                }
                else if ((!variableExists(arg1) && variableExists(arg3)) && !methodExists(arg1) && notObjectMethod(arg1) && !containsParameters(arg1))
                {
                    if (isNumber(arg3))
                    {
                        if (isNumeric(arg1))
                        {
                            if (arg2 == "==")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() == stod(arg1))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() != stod(arg1))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() > stod(arg1))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() < stod(arg1))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() >= stod(arg1))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() <= stod(arg1))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            setTrueIf();
                        }
                    }
                    else
                    {
                        if (arg2 == "==")
                        {
                            if (variables[indexOfVariable(arg3)].getString() == arg1)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (variables[indexOfVariable(arg3)].getString() != arg1)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length > arg1.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length < arg1.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length >= arg1.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length <= arg1.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                }
                else if ((!variableExists(arg1) && variableExists(arg3)) && !methodExists(arg1) && notObjectMethod(arg1) && containsParameters(arg1))
                {
                    string stackValue = (string.Empty);

                    if (isStringStack(arg1))
                        stackValue = getStringStack(arg1);
                    else if (stackReady(arg1))
                        stackValue = dtos(getStack(arg1));
                    else
                        stackValue = arg1;

                    if (isNumber(arg3))
                    {
                        if (isNumeric(stackValue))
                        {
                            if (arg2 == "==")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() == stod(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() != stod(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() > stod(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() < stod(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() >= stod(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() <= stod(stackValue))
                                    setFalseIf();
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            setTrueIf();
                        }
                    }
                    else
                    {
                        if (arg2 == "==")
                        {
                            if (variables[indexOfVariable(arg3)].getString() == stackValue)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (variables[indexOfVariable(arg3)].getString() != stackValue)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length > stackValue.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length < stackValue.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length >= stackValue.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length <= stackValue.Length)
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                }
                else if (containsParameters(arg1) || containsParameters(arg3))
                {
                    if (containsParameters(arg1) && containsParameters(arg3))
                    {
                        if (!zeroDots(arg1) && !zeroDots(arg3))
                        {
                            string arg1before = (beforeDot(arg1)), arg1after = (afterDot(arg1)),
                           arg3before = (beforeDot(arg3)), arg3after = (afterDot(arg3));

                            string arg1Result = (string.Empty), arg3Result = (string.Empty);

                            if (objectExists(arg1before) && objectExists(arg3before))
                            {
                                if (objects[indexOfObject(arg1before)].methodExists(beforeParameters(arg1after)))
                                    executeTemplate(objects[indexOfObject(arg1before)].getMethod(beforeParameters(arg1after)), getParameters(arg1after));

                                arg1Result = __LastValue;

                                if (objects[indexOfObject(arg3before)].methodExists(beforeParameters(arg3after)))
                                    executeTemplate(objects[indexOfObject(arg3before)].getMethod(beforeParameters(arg3after)), getParameters(arg3after));

                                arg3Result = __LastValue;

                                if (isNumeric(arg1Result) && isNumeric(arg3Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (stod(arg1Result) == stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (stod(arg1Result) != stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (stod(arg1Result) < stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (stod(arg1Result) > stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (stod(arg1Result) <= stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (stod(arg1Result) >= stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                            }
                            else
                            {
                                if (!objectExists(arg1before))
                                    error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1before, false);

                                if (!objectExists(arg3before))
                                    error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3before, false);

                                setTrueIf();
                            }
                        }
                        else if (!zeroDots(arg1) && zeroDots(arg3))
                        {
                            string arg1before = (beforeDot(arg1)), arg1after = (afterDot(arg1));

                            string arg1Result = (string.Empty), arg3Result = (string.Empty);

                            if (objectExists(arg1before))
                            {
                                if (objects[indexOfObject(arg1before)].methodExists(beforeParameters(arg1after)))
                                    executeTemplate(objects[indexOfObject(arg1before)].getMethod(beforeParameters(arg1after)), getParameters(arg1after));

                                arg1Result = __LastValue;

                                if (methodExists(beforeParameters(arg3)))
                                    executeTemplate(methods[indexOfMethod(beforeParameters(arg3))], getParameters(arg3));

                                arg3Result = __LastValue;

                                if (isNumeric(arg1Result) && isNumeric(arg3Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (stod(arg1Result) == stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (stod(arg1Result) != stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (stod(arg1Result) < stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (stod(arg1Result) > stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (stod(arg1Result) <= stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (stod(arg1Result) >= stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                            }
                            else
                            {
                                error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1before, false);
                                setTrueIf();
                            }
                        }
                        else if (zeroDots(arg1) && !zeroDots(arg3))
                        {
                            string arg3before = (beforeDot(arg3)), arg3after = (afterDot(arg3));

                            string arg1Result = (string.Empty), arg3Result = (string.Empty);

                            if (objectExists(arg3before))
                            {
                                if (objects[indexOfObject(arg3before)].methodExists(beforeParameters(arg3after)))
                                    executeTemplate(objects[indexOfObject(arg3before)].getMethod(beforeParameters(arg3after)), getParameters(arg3after));

                                arg3Result = __LastValue;

                                if (methodExists(beforeParameters(arg1)))
                                    executeTemplate(methods[indexOfMethod(beforeParameters(arg1))], getParameters(arg1));

                                arg1Result = __LastValue;

                                if (isNumeric(arg1Result) && isNumeric(arg3Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (stod(arg1Result) == stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (stod(arg1Result) != stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (stod(arg1Result) < stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (stod(arg1Result) > stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (stod(arg1Result) <= stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (stod(arg1Result) >= stod(arg3Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                            }
                            else
                            {
                                error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3before, false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            string arg1Result = (string.Empty), arg3Result = (string.Empty);

                            if (methodExists(beforeParameters(arg1)))
                                executeTemplate(methods[indexOfMethod(beforeParameters(arg1))], getParameters(arg1));

                            arg1Result = __LastValue;

                            if (methodExists(beforeParameters(arg3)))
                                executeTemplate(methods[indexOfMethod(beforeParameters(arg3))], getParameters(arg3));

                            arg3Result = __LastValue;

                            if (isNumeric(arg1Result) && isNumeric(arg3Result))
                            {
                                if (arg2 == "==")
                                {
                                    if (stod(arg1Result) == stod(arg3Result))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (stod(arg1Result) != stod(arg3Result))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "<")
                                {
                                    if (stod(arg1Result) < stod(arg3Result))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == ">")
                                {
                                    if (stod(arg1Result) > stod(arg3Result))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == "<=")
                                {
                                    if (stod(arg1Result) <= stod(arg3Result))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else if (arg2 == ">=")
                                {
                                    if (stod(arg1Result) >= stod(arg3Result))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setTrueIf();
                                }
                            }
                        }
                    }
                    else if (containsParameters(arg1) && !containsParameters(arg3))
                    {
                        string arg1Result = (string.Empty), arg3Result = (string.Empty);

                        bool pass = true;

                        if (zeroDots(arg1))
                        {
                            if (methodExists(beforeParameters(arg1)))
                            {
                                executeTemplate(methods[indexOfMethod(beforeParameters(arg1))], getParameters(arg1));

                                arg1Result = __LastValue;

                                if (methodExists(arg3))
                                {
                                    parse(arg3);
                                    arg3Result = __LastValue;
                                }
                                else if (variableExists(arg3))
                                {
                                    if (isString(arg3))
                                        arg3Result = variables[indexOfVariable(arg3)].getString();
                                    else if (isNumber(arg3))
                                        arg3Result = dtos(variables[indexOfVariable(arg3)].getNumber());
                                    else
                                    {
                                        pass = false;
                                        error(ErrorLogger.IS_NULL, arg3, false);
                                        setTrueIf();
                                    }
                                }
                                else
                                    arg3Result = arg3;

                                if (pass)
                                {
                                    if (isNumeric(arg1Result) && isNumeric(arg3Result))
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (stod(arg1Result) == stod(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (stod(arg1Result) != stod(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "<")
                                        {
                                            if (stod(arg1Result) < stod(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == ">")
                                        {
                                            if (stod(arg1Result) > stod(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "<=")
                                        {
                                            if (stod(arg1Result) <= stod(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == ">=")
                                        {
                                            if (stod(arg1Result) >= stod(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else
                                        {
                                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setTrueIf();
                                        }
                                    }
                                }
                                else
                                    setTrueIf();
                            }
                            else
                            {
                                error(ErrorLogger.METHOD_UNDEFINED, beforeParameters(arg1), false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            string arg1before = (beforeDot(arg1)), arg1after = (afterDot(arg1));

                            if (objectExists(arg1before))
                            {
                                if (objects[indexOfObject(arg1before)].methodExists(beforeParameters(arg1after)))
                                    executeTemplate(objects[indexOfObject(arg1before)].getMethod(beforeParameters(arg1after)), getParameters(arg1after));

                                arg1Result = __LastValue;

                                if (variableExists(arg3))
                                {
                                    if (isString(arg3))
                                        arg3Result = variables[indexOfVariable(arg3)].getString();
                                    else if (isNumber(arg3))
                                        arg3Result = dtos(variables[indexOfVariable(arg3)].getNumber());
                                    else
                                    {
                                        pass = false;
                                        error(ErrorLogger.IS_NULL, arg3, false);
                                        setTrueIf();
                                    }
                                }
                                else if (methodExists(arg3))
                                {
                                    parse(arg3);

                                    arg3Result = __LastValue;
                                }
                                else
                                    arg3Result = arg3;

                                if (pass)
                                {
                                    if (isNumeric(arg1Result) && isNumeric(arg3Result))
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (stod(arg1Result) == stod(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (stod(arg1Result) != stod(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "<")
                                        {
                                            if (stod(arg1Result) < stod(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == ">")
                                        {
                                            if (stod(arg1Result) > stod(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "<=")
                                        {
                                            if (stod(arg1Result) <= stod(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == ">=")
                                        {
                                            if (stod(arg1Result) >= stod(arg3Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else
                                        {
                                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setTrueIf();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1before, false);
                                setTrueIf();
                            }
                        }
                    }
                    else if (!containsParameters(arg1) && containsParameters(arg3))
                    {
                        string arg1Result = (string.Empty), arg3Result = (string.Empty);

                        bool pass = true;

                        if (zeroDots(arg3))
                        {
                            if (methodExists(beforeParameters(arg3)))
                            {
                                executeTemplate(methods[indexOfMethod(beforeParameters(arg3))], getParameters(arg3));

                                arg3Result = __LastValue;

                                if (methodExists(arg1))
                                {
                                    parse(arg1);
                                    arg1Result = __LastValue;
                                }
                                else if (variableExists(arg1))
                                {
                                    if (isString(arg1))
                                        arg1Result = variables[indexOfVariable(arg1)].getString();
                                    else if (isNumber(arg1))
                                        arg1Result = dtos(variables[indexOfVariable(arg1)].getNumber());
                                    else
                                    {
                                        pass = false;
                                        error(ErrorLogger.IS_NULL, arg1, false);
                                        setTrueIf();
                                    }
                                }
                                else
                                    arg1Result = arg1;

                                if (pass)
                                {
                                    if (isNumeric(arg3Result) && isNumeric(arg1Result))
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (stod(arg3Result) == stod(arg1Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (stod(arg3Result) != stod(arg1Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "<")
                                        {
                                            if (stod(arg3Result) < stod(arg1Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == ">")
                                        {
                                            if (stod(arg3Result) > stod(arg1Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == "<=")
                                        {
                                            if (stod(arg3Result) <= stod(arg1Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else if (arg2 == ">=")
                                        {
                                            if (stod(arg3Result) >= stod(arg1Result))
                                                setFalseIf();
                                            else
                                                setTrueIf();
                                        }
                                        else
                                        {
                                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setTrueIf();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                error(ErrorLogger.METHOD_UNDEFINED, beforeParameters(arg3), false);
                                setTrueIf();
                            }
                        }
                        else
                        {
                            string arg3before = (beforeDot(arg3)), arg3after = (afterDot(arg3));

                            if (objectExists(arg3before))
                            {
                                if (objects[indexOfObject(arg3before)].methodExists(beforeParameters(arg3after)))
                                    executeTemplate(objects[indexOfObject(arg3before)].getMethod(beforeParameters(arg3after)), getParameters(arg3after));

                                arg3Result = __LastValue;

                                if (variableExists(arg1))
                                {
                                    if (isString(arg1))
                                        arg1Result = variables[indexOfVariable(arg1)].getString();
                                    else if (isNumber(arg3))
                                        arg1Result = dtos(variables[indexOfVariable(arg1)].getNumber());
                                    else
                                    {
                                        error(ErrorLogger.IS_NULL, arg1, false);
                                        setTrueIf();
                                    }
                                }
                                else if (methodExists(arg1))
                                {
                                    parse(arg1);

                                    arg1Result = __LastValue;
                                }
                                else
                                    arg1Result = arg1;

                                if (isNumeric(arg3Result) && isNumeric(arg1Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (stod(arg3Result) == stod(arg1Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (stod(arg3Result) != stod(arg1Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (stod(arg3Result) < stod(arg1Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (stod(arg3Result) > stod(arg1Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (stod(arg3Result) <= stod(arg1Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (stod(arg3Result) >= stod(arg1Result))
                                            setFalseIf();
                                        else
                                            setTrueIf();
                                    }
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setTrueIf();
                                    }
                                }
                            }
                            else
                            {
                                error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3before, false);
                                setTrueIf();
                            }
                        }
                    }
                }
                else if ((methodExists(arg1) && arg3 != "method?") || methodExists(arg3))
                {
                    string arg1Result = (string.Empty), arg3Result = (string.Empty);

                    if (methodExists(arg1))
                    {
                        parse(arg1);
                        arg1Result = __LastValue;
                    }
                    else if (variableExists(arg1))
                    {
                        if (isString(arg1))
                            arg1Result = variables[indexOfVariable(arg1)].getString();
                        else if (isNumber(arg1))
                            arg1Result = dtos(variables[indexOfVariable(arg1)].getNumber());
                        else
                        {
                            error(ErrorLogger.IS_NULL, arg1, false);
                            setTrueIf();
                        }
                    }
                    else
                        arg1Result = arg1;

                    if (methodExists(arg3))
                    {
                        parse(arg3);
                        arg3Result = __LastValue;
                    }
                    else if (variableExists(arg3))
                    {
                        if (isString(arg3))
                            arg3Result = variables[indexOfVariable(arg3)].getString();
                        else if (isNumber(arg3))
                            arg3Result = dtos(variables[indexOfVariable(arg3)].getNumber());
                        else
                        {
                            error(ErrorLogger.IS_NULL, arg3, false);
                            setTrueIf();
                        }
                    }
                    else
                        arg3Result = arg3;

                    if (isNumeric(arg1Result) && isNumeric(arg3Result))
                    {
                        if (arg2 == "==")
                        {
                            if (stod(arg1Result) == stod(arg3Result))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (stod(arg1Result) != stod(arg3Result))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (stod(arg1Result) < stod(arg3Result))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (stod(arg1Result) > stod(arg3Result))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (stod(arg1Result) <= stod(arg3Result))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (stod(arg1Result) >= stod(arg3Result))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                }
                else
                {
                    if (arg3 == "object?")
                    {
                        if (objectExists(arg1))
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                    }
                    else if (arg3 == "variable?")
                    {
                        if (variableExists(arg1))
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                    }
                    else if (arg3 == "method?")
                    {
                        if (methodExists(arg1))
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setTrueIf();
                            }
                        }
                    }
                    else if (arg3 == "list?")
                    {
                        if (listExists(arg1))
                        {
                            if (arg2 == "==")
                                setFalseIf();
                            else if (arg2 == "!=")
                                setTrueIf();
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                        if (isNumeric(arg1) && isNumeric(arg3))
                        {
                            if (stod(arg1) > stod(arg3))
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
                        if (isNumeric(arg1) && isNumeric(arg3))
                        {
                            if (stod(arg1) < stod(arg3))
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
                        if (isNumeric(arg1) && isNumeric(arg3))
                        {
                            if (stod(arg1) >= stod(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setTrueIf();
                        }
                    }
                    else if (arg2 == "<=")
                    {
                        if (isNumeric(arg1) && isNumeric(arg3))
                        {
                            if (stod(arg1) <= stod(arg3))
                                setFalseIf();
                            else
                                setTrueIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                    else if (arg2 == "begins_with")
                    {
                        if (startsWith(arg1, arg3))
                            setFalseIf();
                        else
                            setTrueIf();
                    }
                    else if (arg2 == "ends_with")
                    {
                        if (endsWith(arg1, arg3))
                            setFalseIf();
                        else
                            setTrueIf();
                    }
                    else if (arg2 == "contains")
                    {
                        if (contains(arg1, arg3))
                            setFalseIf();
                        else
                            setTrueIf();
                    }
                    else
                    {
                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                        setTrueIf();
                    }
                }
            }
            else if (arg0 == "if")
            {
                if (listExists(arg3))
                {
                    if (arg2 == "in")
                    {
                        string testString = ("[none]");

                        if (variableExists(arg1))
                        {
                            if (isString(arg1))
                                testString = variables[indexOfVariable(arg1)].getString();
                            else if (isNumber(arg1))
                                testString = dtos(variables[indexOfVariable(arg1)].getNumber());
                            else
                                error(ErrorLogger.IS_NULL, arg1, false);
                        }
                        else
                            testString = arg1;

                        if (testString != "[none]")
                        {
                            bool elementFound = false;
                            for (int i = 0; i < (int)lists[indexOfList(arg3)].size(); i++)
                            {
                                if (lists[indexOfList(arg3)].at(i) == testString)
                                {
                                    elementFound = true;
                                    setTrueIf();
                                    __LastValue = itos(i);
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
                else if (listExists(arg1) && arg3 != "list?")
                {
                    if (arg2 == "contains")
                    {
                        string testString = ("[none]");

                        if (variableExists(arg3))
                        {
                            if (isString(arg3))
                                testString = variables[indexOfVariable(arg3)].getString();
                            else if (isNumber(arg3))
                                testString = dtos(variables[indexOfVariable(arg3)].getNumber());
                            else
                                error(ErrorLogger.IS_NULL, arg3, false);
                        }
                        else
                            testString = arg3;

                        if (testString != "[none]")
                        {
                            bool elementFound = false;
                            for (int i = 0; i < (int)lists[indexOfList(arg1)].size(); i++)
                            {
                                if (lists[indexOfList(arg1)].at(i) == testString)
                                {
                                    elementFound = true;
                                    setTrueIf();
                                    __LastValue = itos(i);
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
                else if (variableExists(arg1) && variableExists(arg3))
                {
                    if (isString(arg1) && isString(arg3))
                    {
                        if (arg2 == "==")
                        {
                            if (variables[indexOfVariable(arg1)].getString() == variables[indexOfVariable(arg3)].getString())
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (variables[indexOfVariable(arg1)].getString() != variables[indexOfVariable(arg3)].getString())
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (variables[indexOfVariable(arg1)].getString().Length > variables[indexOfVariable(arg3)].getString().Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (variables[indexOfVariable(arg1)].getString().Length < variables[indexOfVariable(arg3)].getString().Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (variables[indexOfVariable(arg1)].getString().Length <= variables[indexOfVariable(arg3)].getString().Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (variables[indexOfVariable(arg1)].getString().Length >= variables[indexOfVariable(arg3)].getString().Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "contains")
                        {
                            if (contains(variables[indexOfVariable(arg1)].getString(), variables[indexOfVariable(arg3)].getString()))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "ends_with")
                        {
                            if (endsWith(variables[indexOfVariable(arg1)].getString(), variables[indexOfVariable(arg3)].getString()))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "begins_with")
                        {
                            if (startsWith(variables[indexOfVariable(arg1)].getString(), variables[indexOfVariable(arg3)].getString()))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                    else if (isNumber(arg1) && isNumber(arg3))
                    {
                        if (arg2 == "==")
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() == variables[indexOfVariable(arg3)].getNumber())
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() != variables[indexOfVariable(arg3)].getNumber())
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() > variables[indexOfVariable(arg3)].getNumber())
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() >= variables[indexOfVariable(arg3)].getNumber())
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() < variables[indexOfVariable(arg3)].getNumber())
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() <= variables[indexOfVariable(arg3)].getNumber())
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                    else
                    {
                        error(ErrorLogger.CONV_ERR, s, false);
                        setFalseIf();
                    }
                }
                else if ((variableExists(arg1) && !variableExists(arg3)) && !methodExists(arg3) && notObjectMethod(arg3) && !containsParameters(arg3))
                {
                    if (isNumber(arg1))
                    {
                        if (isNumeric(arg3))
                        {
                            if (arg2 == "==")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() == stod(arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() != stod(arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() > stod(arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() < stod(arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() >= stod(arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() <= stod(arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            setFalseIf();
                        }
                    }
                    else
                    {
                        if (arg3 == "string?")
                        {
                            if (isString(arg1))
                            {
                                if (arg2 == "==")
                                    setTrueIf();
                                else if (arg2 == "!=")
                                    setFalseIf();
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                            if (isNumber(arg1))
                            {
                                if (arg2 == "==")
                                    setTrueIf();
                                else if (arg2 == "!=")
                                    setFalseIf();
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                            if (isString(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (isUpper(variables[indexOfVariable(arg1)].getString()))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (isUpper(variables[indexOfVariable(arg1)].getString()))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (isUpper(arg2))
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
                            if (isString(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (isLower(variables[indexOfVariable(arg1)].getString()))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (isLower(variables[indexOfVariable(arg1)].getString()))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (isLower(arg2))
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
                            if (isString(arg1))
                            {
                                if (System.IO.File.Exists(variables[indexOfVariable(arg1)].getString()))
                                {
                                    if (arg2 == "==")
                                        setTrueIf();
                                    else if (arg2 == "!=")
                                        setFalseIf();
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.IS_NULL, arg1, false);
                                setFalseIf();
                            }
                        }
                        else if (arg3 == "dir?" || arg3 == "directory?")
                        {
                            if (isString(arg1))
                            {
                                if (System.IO.Directory.Exists(variables[indexOfVariable(arg1)].getString()))
                                {
                                    if (arg2 == "==")
                                        setTrueIf();
                                    else if (arg2 == "!=")
                                        setFalseIf();
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.IS_NULL, arg1, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                            {
                                if (variables[indexOfVariable(arg1)].getString() == arg3)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (variables[indexOfVariable(arg1)].getString() != arg3)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length > arg3.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length < arg3.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length >= arg3.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length <= arg3.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "contains")
                            {
                                if (contains(variables[indexOfVariable(arg1)].getString(), arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "ends_with")
                            {
                                if (endsWith(variables[indexOfVariable(arg1)].getString(), arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "begins_with")
                            {
                                if (startsWith(variables[indexOfVariable(arg1)].getString(), arg3))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                    }
                }
                else if ((variableExists(arg1) && !variableExists(arg3)) && !methodExists(arg3) && notObjectMethod(arg3) && containsParameters(arg3))
                {
                    string stackValue = (string.Empty);

                    if (isStringStack(arg3))
                        stackValue = getStringStack(arg3);
                    else if (stackReady(arg3))
                        stackValue = dtos(getStack(arg3));
                    else
                        stackValue = arg3;

                    if (isNumber(arg1))
                    {
                        if (isNumeric(stackValue))
                        {
                            if (arg2 == "==")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() == stod(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() != stod(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() > stod(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() < stod(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() >= stod(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (variables[indexOfVariable(arg1)].getNumber() <= stod(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            setFalseIf();
                        }
                    }
                    else
                    {
                        if (stackValue == "string?")
                        {
                            if (isString(arg1))
                            {
                                if (arg2 == "==")
                                    setTrueIf();
                                else if (arg2 == "!=")
                                    setFalseIf();
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                            if (isNumber(arg1))
                            {
                                if (arg2 == "==")
                                    setTrueIf();
                                else if (arg2 == "!=")
                                    setFalseIf();
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                            if (isString(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (isUpper(variables[indexOfVariable(arg1)].getString()))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (isUpper(variables[indexOfVariable(arg1)].getString()))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (isUpper(arg2))
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
                            if (isString(arg1))
                            {
                                if (arg2 == "==")
                                {
                                    if (isLower(variables[indexOfVariable(arg1)].getString()))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (isLower(variables[indexOfVariable(arg1)].getString()))
                                        setFalseIf();
                                    else
                                        setTrueIf();
                                }
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                            else
                            {
                                if (arg2 == "!=")
                                {
                                    if (isLower(arg2))
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
                            if (isString(arg1))
                            {
                                if (System.IO.File.Exists(variables[indexOfVariable(arg1)].getString()))
                                {
                                    if (arg2 == "==")
                                        setTrueIf();
                                    else if (arg2 == "!=")
                                        setFalseIf();
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.IS_NULL, arg1, false);
                                setFalseIf();
                            }
                        }
                        else if (stackValue == "directory?")
                        {
                            if (isString(arg1))
                            {
                                if (System.IO.Directory.Exists(variables[indexOfVariable(arg1)].getString()))
                                {
                                    if (arg2 == "==")
                                        setTrueIf();
                                    else if (arg2 == "!=")
                                        setFalseIf();
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.IS_NULL, arg1, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            if (arg2 == "==")
                            {
                                if (variables[indexOfVariable(arg1)].getString() == stackValue)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (variables[indexOfVariable(arg1)].getString() != stackValue)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length > stackValue.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length < stackValue.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length >= stackValue.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (variables[indexOfVariable(arg1)].getString().Length <= stackValue.Length)
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "contains")
                            {
                                if (contains(variables[indexOfVariable(arg1)].getString(), stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "ends_with")
                            {
                                if (endsWith(variables[indexOfVariable(arg1)].getString(), stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "begins_with")
                            {
                                if (startsWith(variables[indexOfVariable(arg1)].getString(), stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                    }
                }
                else if ((!variableExists(arg1) && variableExists(arg3)) && !methodExists(arg1) && notObjectMethod(arg1) && !containsParameters(arg1))
                {
                    if (isNumber(arg3))
                    {
                        if (isNumeric(arg1))
                        {
                            if (arg2 == "==")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() == stod(arg1))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() != stod(arg1))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() > stod(arg1))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() < stod(arg1))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() >= stod(arg1))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() <= stod(arg1))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            setFalseIf();
                        }
                    }
                    else
                    {
                        if (arg2 == "==")
                        {
                            if (variables[indexOfVariable(arg3)].getString() == arg1)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (variables[indexOfVariable(arg3)].getString() != arg1)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length > arg1.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length < arg1.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length >= arg1.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length <= arg1.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                }
                else if ((!variableExists(arg1) && variableExists(arg3)) && !methodExists(arg1) && notObjectMethod(arg1) && containsParameters(arg1))
                {
                    string stackValue = (string.Empty);

                    if (isStringStack(arg1))
                        stackValue = getStringStack(arg1);
                    else if (stackReady(arg1))
                        stackValue = dtos(getStack(arg1));
                    else
                        stackValue = arg1;

                    if (isNumber(arg3))
                    {
                        if (isNumeric(stackValue))
                        {
                            if (arg2 == "==")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() == stod(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "!=")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() != stod(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() > stod(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() < stod(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == ">=")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() >= stod(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else if (arg2 == "<=")
                            {
                                if (variables[indexOfVariable(arg3)].getNumber() <= stod(stackValue))
                                    setTrueIf();
                                else
                                    setFalseIf();
                            }
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            setFalseIf();
                        }
                    }
                    else
                    {
                        if (arg2 == "==")
                        {
                            if (variables[indexOfVariable(arg3)].getString() == stackValue)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (variables[indexOfVariable(arg3)].getString() != stackValue)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length > stackValue.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length < stackValue.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length >= stackValue.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (variables[indexOfVariable(arg3)].getString().Length <= stackValue.Length)
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                }
                else if (containsParameters(arg1) || containsParameters(arg3))
                {
                    if (containsParameters(arg1) && containsParameters(arg3))
                    {
                        if (!zeroDots(arg1) && !zeroDots(arg3))
                        {
                            string arg1before = (beforeDot(arg1)), arg1after = (afterDot(arg1)),
                                arg3before = (beforeDot(arg3)), arg3after = (afterDot(arg3));

                            string arg1Result = (string.Empty), arg3Result = (string.Empty);

                            if (objectExists(arg1before) && objectExists(arg3before))
                            {
                                if (objects[indexOfObject(arg1before)].methodExists(beforeParameters(arg1after)))
                                    executeTemplate(objects[indexOfObject(arg1before)].getMethod(beforeParameters(arg1after)), getParameters(arg1after));

                                arg1Result = __LastValue;

                                if (objects[indexOfObject(arg3before)].methodExists(beforeParameters(arg3after)))
                                    executeTemplate(objects[indexOfObject(arg3before)].getMethod(beforeParameters(arg3after)), getParameters(arg3after));

                                arg3Result = __LastValue;

                                if (isNumeric(arg1Result) && isNumeric(arg3Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (stod(arg1Result) == stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (stod(arg1Result) != stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (stod(arg1Result) < stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (stod(arg1Result) > stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (stod(arg1Result) <= stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (stod(arg1Result) >= stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                            }
                            else
                            {
                                if (!objectExists(arg1before))
                                    error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1before, false);

                                if (!objectExists(arg3before))
                                    error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3before, false);

                                setFalseIf();
                            }
                        }
                        else if (!zeroDots(arg1) && zeroDots(arg3))
                        {
                            string arg1before = (beforeDot(arg1)), arg1after = (afterDot(arg1));

                            string arg1Result = (string.Empty), arg3Result = (string.Empty);

                            if (objectExists(arg1before))
                            {
                                if (objects[indexOfObject(arg1before)].methodExists(beforeParameters(arg1after)))
                                    executeTemplate(objects[indexOfObject(arg1before)].getMethod(beforeParameters(arg1after)), getParameters(arg1after));

                                arg1Result = __LastValue;

                                if (methodExists(beforeParameters(arg3)))
                                    executeTemplate(methods[indexOfMethod(beforeParameters(arg3))], getParameters(arg3));

                                arg3Result = __LastValue;

                                if (isNumeric(arg1Result) && isNumeric(arg3Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (stod(arg1Result) == stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (stod(arg1Result) != stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (stod(arg1Result) < stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (stod(arg1Result) > stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (stod(arg1Result) <= stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (stod(arg1Result) >= stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                            }
                            else
                            {
                                error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1before, false);
                                setFalseIf();
                            }
                        }
                        else if (zeroDots(arg1) && !zeroDots(arg3))
                        {
                            string arg3before = (beforeDot(arg3)), arg3after = (afterDot(arg3));

                            string arg1Result = (string.Empty), arg3Result = (string.Empty);

                            if (objectExists(arg3before))
                            {
                                if (objects[indexOfObject(arg3before)].methodExists(beforeParameters(arg3after)))
                                    executeTemplate(objects[indexOfObject(arg3before)].getMethod(beforeParameters(arg3after)), getParameters(arg3after));

                                arg3Result = __LastValue;

                                if (methodExists(beforeParameters(arg1)))
                                    executeTemplate(methods[indexOfMethod(beforeParameters(arg1))], getParameters(arg1));

                                arg1Result = __LastValue;

                                if (isNumeric(arg1Result) && isNumeric(arg3Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (stod(arg1Result) == stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (stod(arg1Result) != stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (stod(arg1Result) < stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (stod(arg1Result) > stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (stod(arg1Result) <= stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (stod(arg1Result) >= stod(arg3Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                            }
                            else
                            {
                                error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3before, false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            string arg1Result = (string.Empty), arg3Result = (string.Empty);

                            if (methodExists(beforeParameters(arg1)))
                                executeTemplate(methods[indexOfMethod(beforeParameters(arg1))], getParameters(arg1));

                            arg1Result = __LastValue;

                            if (methodExists(beforeParameters(arg3)))
                                executeTemplate(methods[indexOfMethod(beforeParameters(arg3))], getParameters(arg3));

                            arg3Result = __LastValue;

                            if (isNumeric(arg1Result) && isNumeric(arg3Result))
                            {
                                if (arg2 == "==")
                                {
                                    if (stod(arg1Result) == stod(arg3Result))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "!=")
                                {
                                    if (stod(arg1Result) != stod(arg3Result))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "<")
                                {
                                    if (stod(arg1Result) < stod(arg3Result))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == ">")
                                {
                                    if (stod(arg1Result) > stod(arg3Result))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == "<=")
                                {
                                    if (stod(arg1Result) <= stod(arg3Result))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else if (arg2 == ">=")
                                {
                                    if (stod(arg1Result) >= stod(arg3Result))
                                        setTrueIf();
                                    else
                                        setFalseIf();
                                }
                                else
                                {
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                    error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                    setFalseIf();
                                }
                            }
                        }
                    }
                    else if (containsParameters(arg1) && !containsParameters(arg3))
                    {
                        string arg1Result = (string.Empty), arg3Result = (string.Empty);

                        bool pass = true;

                        if (zeroDots(arg1))
                        {
                            if (methodExists(beforeParameters(arg1)))
                            {
                                executeTemplate(methods[indexOfMethod(beforeParameters(arg1))], getParameters(arg1));

                                arg1Result = __LastValue;

                                if (methodExists(arg3))
                                {
                                    parse(arg3);
                                    arg3Result = __LastValue;
                                }
                                else if (variableExists(arg3))
                                {
                                    if (isString(arg3))
                                        arg3Result = variables[indexOfVariable(arg3)].getString();
                                    else if (isNumber(arg3))
                                        arg3Result = dtos(variables[indexOfVariable(arg3)].getNumber());
                                    else
                                    {
                                        pass = false;
                                        error(ErrorLogger.IS_NULL, arg3, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                    arg3Result = arg3;

                                if (pass)
                                {
                                    if (isNumeric(arg1Result) && isNumeric(arg3Result))
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (stod(arg1Result) == stod(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (stod(arg1Result) != stod(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "<")
                                        {
                                            if (stod(arg1Result) < stod(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == ">")
                                        {
                                            if (stod(arg1Result) > stod(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "<=")
                                        {
                                            if (stod(arg1Result) <= stod(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == ">=")
                                        {
                                            if (stod(arg1Result) >= stod(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else
                                        {
                                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setFalseIf();
                                        }
                                    }
                                }
                                else
                                    setFalseIf();
                            }
                            else if (stackReady(arg1))
                            {
                                string stackValue = (string.Empty);

                                if (isStringStack(arg1))
                                    stackValue = getStringStack(arg1);
                                else
                                    stackValue = dtos(getStack(arg1));

                                string comp = (string.Empty);

                                if (variableExists(arg3))
                                {
                                    if (isString(arg3))
                                        comp = variables[indexOfVariable(arg3)].getString();
                                    else if (isNumber(arg3))
                                        comp = dtos(variables[indexOfVariable(arg3)].getNumber());
                                }
                                else if (methodExists(arg3))
                                {
                                    parse(arg3);

                                    comp = __LastValue;
                                }
                                else if (containsParameters(arg3))
                                {
                                    executeTemplate(getMethod(beforeParameters(arg3)), getParameters(arg3));

                                    comp = __LastValue;
                                }
                                else
                                    comp = arg3;

                                if (isNumeric(stackValue) && isNumeric(comp))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (stod(stackValue) == stod(comp))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (stod(stackValue) != stod(comp))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (stod(stackValue) < stod(comp))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (stod(stackValue) > stod(comp))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (stod(stackValue) <= stod(comp))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (stod(stackValue) >= stod(comp))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                            }
                            else
                            {
                                error(ErrorLogger.METHOD_UNDEFINED, beforeParameters(arg1), false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            string arg1before = (beforeDot(arg1)), arg1after = (afterDot(arg1));

                            if (objectExists(arg1before))
                            {
                                if (objects[indexOfObject(arg1before)].methodExists(beforeParameters(arg1after)))
                                    executeTemplate(objects[indexOfObject(arg1before)].getMethod(beforeParameters(arg1after)), getParameters(arg1after));

                                arg1Result = __LastValue;

                                if (variableExists(arg3))
                                {
                                    if (isString(arg3))
                                        arg3Result = variables[indexOfVariable(arg3)].getString();
                                    else if (isNumber(arg3))
                                        arg3Result = dtos(variables[indexOfVariable(arg3)].getNumber());
                                    else
                                    {
                                        pass = false;
                                        error(ErrorLogger.IS_NULL, arg3, false);
                                        setFalseIf();
                                    }
                                }
                                else if (methodExists(arg3))
                                {
                                    parse(arg3);

                                    arg3Result = __LastValue;
                                }
                                else
                                    arg3Result = arg3;

                                if (pass)
                                {
                                    if (isNumeric(arg1Result) && isNumeric(arg3Result))
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (stod(arg1Result) == stod(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (stod(arg1Result) != stod(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "<")
                                        {
                                            if (stod(arg1Result) < stod(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == ">")
                                        {
                                            if (stod(arg1Result) > stod(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "<=")
                                        {
                                            if (stod(arg1Result) <= stod(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == ">=")
                                        {
                                            if (stod(arg1Result) >= stod(arg3Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else
                                        {
                                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setFalseIf();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1before, false);
                                setFalseIf();
                            }
                        }
                    }
                    else if (!containsParameters(arg1) && containsParameters(arg3))
                    {
                        string arg1Result = (string.Empty), arg3Result = (string.Empty);

                        bool pass = true;

                        if (zeroDots(arg3))
                        {
                            if (methodExists(beforeParameters(arg3)))
                            {
                                executeTemplate(methods[indexOfMethod(beforeParameters(arg3))], getParameters(arg3));

                                arg3Result = __LastValue;

                                if (methodExists(arg1))
                                {
                                    parse(arg1);
                                    arg1Result = __LastValue;
                                }
                                else if (variableExists(arg1))
                                {
                                    if (isString(arg1))
                                        arg1Result = variables[indexOfVariable(arg1)].getString();
                                    else if (isNumber(arg1))
                                        arg1Result = dtos(variables[indexOfVariable(arg1)].getNumber());
                                    else
                                    {
                                        pass = false;
                                        error(ErrorLogger.IS_NULL, arg1, false);
                                        setFalseIf();
                                    }
                                }
                                else
                                    arg1Result = arg1;

                                if (pass)
                                {
                                    if (isNumeric(arg3Result) && isNumeric(arg1Result))
                                    {
                                        if (arg2 == "==")
                                        {
                                            if (stod(arg3Result) == stod(arg1Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "!=")
                                        {
                                            if (stod(arg3Result) != stod(arg1Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "<")
                                        {
                                            if (stod(arg3Result) < stod(arg1Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == ">")
                                        {
                                            if (stod(arg3Result) > stod(arg1Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == "<=")
                                        {
                                            if (stod(arg3Result) <= stod(arg1Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else if (arg2 == ">=")
                                        {
                                            if (stod(arg3Result) >= stod(arg1Result))
                                                setTrueIf();
                                            else
                                                setFalseIf();
                                        }
                                        else
                                        {
                                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                            setFalseIf();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                error(ErrorLogger.METHOD_UNDEFINED, beforeParameters(arg3), false);
                                setFalseIf();
                            }
                        }
                        else
                        {
                            string arg3before = (beforeDot(arg3)), arg3after = (afterDot(arg3));

                            if (objectExists(arg3before))
                            {
                                if (objects[indexOfObject(arg3before)].methodExists(beforeParameters(arg3after)))
                                    executeTemplate(objects[indexOfObject(arg3before)].getMethod(beforeParameters(arg3after)), getParameters(arg3after));

                                arg3Result = __LastValue;

                                if (variableExists(arg1))
                                {
                                    if (isString(arg1))
                                        arg1Result = variables[indexOfVariable(arg1)].getString();
                                    else if (isNumber(arg3))
                                        arg1Result = dtos(variables[indexOfVariable(arg1)].getNumber());
                                    else
                                    {
                                        error(ErrorLogger.IS_NULL, arg1, false);
                                        setFalseIf();
                                    }
                                }
                                else if (methodExists(arg1))
                                {
                                    parse(arg1);

                                    arg1Result = __LastValue;
                                }
                                else
                                    arg1Result = arg1;

                                if (isNumeric(arg3Result) && isNumeric(arg1Result))
                                {
                                    if (arg2 == "==")
                                    {
                                        if (stod(arg3Result) == stod(arg1Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "!=")
                                    {
                                        if (stod(arg3Result) != stod(arg1Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<")
                                    {
                                        if (stod(arg3Result) < stod(arg1Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">")
                                    {
                                        if (stod(arg3Result) > stod(arg1Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == "<=")
                                    {
                                        if (stod(arg3Result) <= stod(arg1Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else if (arg2 == ">=")
                                    {
                                        if (stod(arg3Result) >= stod(arg1Result))
                                            setTrueIf();
                                        else
                                            setFalseIf();
                                    }
                                    else
                                    {
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                        setFalseIf();
                                    }
                                }
                            }
                            else
                            {
                                error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg3before, false);
                                setFalseIf();
                            }
                        }
                    }
                }
                else if ((methodExists(arg1) && arg3 != "method?") || methodExists(arg3))
                {
                    string arg1Result = (string.Empty), arg3Result = (string.Empty);

                    if (methodExists(arg1))
                    {
                        parse(arg1);
                        arg1Result = __LastValue;
                    }
                    else if (variableExists(arg1))
                    {
                        if (isString(arg1))
                            arg1Result = variables[indexOfVariable(arg1)].getString();
                        else if (isNumber(arg1))
                            arg1Result = dtos(variables[indexOfVariable(arg1)].getNumber());
                        else
                        {
                            error(ErrorLogger.IS_NULL, arg1, false);
                            setFalseIf();
                        }
                    }
                    else
                        arg1Result = arg1;

                    if (methodExists(arg3))
                    {
                        parse(arg3);
                        arg3Result = __LastValue;
                    }
                    else if (variableExists(arg3))
                    {
                        if (isString(arg3))
                            arg3Result = variables[indexOfVariable(arg3)].getString();
                        else if (isNumber(arg3))
                            arg3Result = dtos(variables[indexOfVariable(arg3)].getNumber());
                        else
                        {
                            error(ErrorLogger.IS_NULL, arg3, false);
                            setFalseIf();
                        }
                    }
                    else
                        arg3Result = arg3;

                    if (isNumeric(arg1Result) && isNumeric(arg3Result))
                    {
                        if (arg2 == "==")
                        {
                            if (stod(arg1Result) == stod(arg3Result))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "!=")
                        {
                            if (stod(arg1Result) != stod(arg3Result))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<")
                        {
                            if (stod(arg1Result) < stod(arg3Result))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">")
                        {
                            if (stod(arg1Result) > stod(arg3Result))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == "<=")
                        {
                            if (stod(arg1Result) <= stod(arg3Result))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else if (arg2 == ">=")
                        {
                            if (stod(arg1Result) >= stod(arg3Result))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                }
                else
                {
                    if (arg3 == "object?")
                    {
                        if (objectExists(arg1))
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                    }
                    else if (arg3 == "variable?")
                    {
                        if (variableExists(arg1))
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                    }
                    else if (arg3 == "method?")
                    {
                        if (methodExists(arg1))
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                                setFalseIf();
                            }
                        }
                    }
                    else if (arg3 == "list?")
                    {
                        if (listExists(arg1))
                        {
                            if (arg2 == "==")
                                setTrueIf();
                            else if (arg2 == "!=")
                                setFalseIf();
                            else
                            {
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
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
                        if (isNumeric(arg1) && isNumeric(arg3))
                        {
                            if (stod(arg1) > stod(arg3))
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
                        if (isNumeric(arg1) && isNumeric(arg3))
                        {
                            if (stod(arg1) < stod(arg3))
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
                        if (isNumeric(arg1) && isNumeric(arg3))
                        {
                            if (stod(arg1) >= stod(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                    else if (arg2 == "<=")
                    {
                        if (isNumeric(arg1) && isNumeric(arg3))
                        {
                            if (stod(arg1) <= stod(arg3))
                                setTrueIf();
                            else
                                setFalseIf();
                        }
                        else
                        {
                            error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                            setFalseIf();
                        }
                    }
                    else if (arg2 == "begins_with")
                    {
                        if (startsWith(arg1, arg3))
                            setTrueIf();
                        else
                            setFalseIf();
                    }
                    else if (arg2 == "ends_with")
                    {
                        if (endsWith(arg1, arg3))
                            setTrueIf();
                        else
                            setFalseIf();
                    }
                    else if (arg2 == "contains")
                    {
                        if (contains(arg1, arg3))
                            setTrueIf();
                        else
                            setFalseIf();
                    }
                    else
                    {
                        error(ErrorLogger.INVALID_OPERATOR, arg2, false);
                        setFalseIf();
                    }
                }
            }
            else if (arg0 == "for")
            {
                if (arg2 == "<")
                {
                    if (variableExists(arg1) && variableExists(arg3))
                    {
                        if (isNumber(arg1) && isNumber(arg3))
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() < variables[indexOfVariable(arg3)].getNumber())
                                successfulFor(variables[indexOfVariable(arg1)].getNumber(), variables[indexOfVariable(arg3)].getNumber(), "<");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (variableExists(arg1) && !variableExists(arg3))
                    {
                        if (isNumber(arg1) && isNumeric(arg3))
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() < stod(arg3))
                                successfulFor(variables[indexOfVariable(arg1)].getNumber(), stod(arg3), "<");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (!variableExists(arg1) && variableExists(arg3))
                    {
                        if (isNumeric(arg1) && isNumber(arg3))
                        {
                            if (stod(arg1) < variables[indexOfVariable(arg3)].getNumber())
                                successfulFor(stod(arg1), variables[indexOfVariable(arg3)].getNumber(), "<");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else
                    {
                        if (isNumeric(arg1) && isNumeric(arg3))
                        {
                            if (stod(arg1) < stod(arg3))
                                successfulFor(stod(arg1), stod(arg3), "<");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                }
                else if (arg2 == ">")
                {
                    if (variableExists(arg1) && variableExists(arg3))
                    {
                        if (isNumber(arg1) && isNumber(arg3))
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() > variables[indexOfVariable(arg3)].getNumber())
                                successfulFor(variables[indexOfVariable(arg1)].getNumber(), variables[indexOfVariable(arg3)].getNumber(), ">");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (variableExists(arg1) && !variableExists(arg3))
                    {
                        if (isNumber(arg1) && isNumeric(arg3))
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() > stod(arg3))
                                successfulFor(variables[indexOfVariable(arg1)].getNumber(), stod(arg3), ">");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (!variableExists(arg1) && variableExists(arg3))
                    {
                        if (isNumeric(arg1) && isNumber(arg3))
                        {
                            if (stod(arg1) > variables[indexOfVariable(arg3)].getNumber())
                                successfulFor(stod(arg1), variables[indexOfVariable(arg3)].getNumber(), ">");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else
                    {
                        if (isNumeric(arg1) && isNumeric(arg3))
                        {
                            if (stod(arg1) > stod(arg3))
                                successfulFor(stod(arg1), stod(arg3), ">");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                }
                else if (arg2 == "<=")
                {
                    if (variableExists(arg1) && variableExists(arg3))
                    {
                        if (isNumber(arg1) && isNumber(arg3))
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() <= variables[indexOfVariable(arg3)].getNumber())
                                successfulFor(variables[indexOfVariable(arg1)].getNumber(), variables[indexOfVariable(arg3)].getNumber(), "<=");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (variableExists(arg1) && !variableExists(arg3))
                    {
                        if (isNumber(arg1) && isNumeric(arg3))
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() <= stod(arg3))
                                successfulFor(variables[indexOfVariable(arg1)].getNumber(), stod(arg3), "<=");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (!variableExists(arg1) && variableExists(arg3))
                    {
                        if (isNumeric(arg1) && isNumber(arg3))
                        {
                            if (stod(arg1) <= variables[indexOfVariable(arg3)].getNumber())
                                successfulFor(stod(arg1), variables[indexOfVariable(arg3)].getNumber(), "<=");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else
                    {
                        if (isNumeric(arg1) && isNumeric(arg3))
                        {
                            if (stod(arg1) <= stod(arg3))
                                successfulFor(stod(arg1), stod(arg3), "<=");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                }
                else if (arg2 == ">=")
                {
                    if (variableExists(arg1) && variableExists(arg3))
                    {
                        if (isNumber(arg1) && isNumber(arg3))
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() >= variables[indexOfVariable(arg3)].getNumber())
                                successfulFor(variables[indexOfVariable(arg1)].getNumber(), variables[indexOfVariable(arg3)].getNumber(), ">=");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (variableExists(arg1) && !variableExists(arg3))
                    {
                        if (isNumber(arg1) && isNumeric(arg3))
                        {
                            if (variables[indexOfVariable(arg1)].getNumber() >= stod(arg3))
                                successfulFor(variables[indexOfVariable(arg1)].getNumber(), stod(arg3), ">=");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else if (!variableExists(arg1) && variableExists(arg3))
                    {
                        if (isNumeric(arg1) && isNumber(arg3))
                        {
                            if (stod(arg1) >= variables[indexOfVariable(arg3)].getNumber())
                                successfulFor(stod(arg1), variables[indexOfVariable(arg3)].getNumber(), ">=");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                    else
                    {
                        if (isNumeric(arg1) && isNumeric(arg3))
                        {
                            if (stod(arg1) >= stod(arg3))
                                successfulFor(stod(arg1), stod(arg3), ">=");
                            else
                                failedFor();
                        }
                        else
                        {
                            error(ErrorLogger.CONV_ERR, s, false);
                            failedFor();
                        }
                    }
                }
                else if (arg2 == "in")
                {
                    if (arg1 == "var")
                    {
                        string before = (beforeDot(arg3)), after = (afterDot(arg3));

                        if (before == "args" && after == "size")
                        {
                            List newList = new();

                            for (int i = 0; i < args.Count; i++)
                                newList.add(args[i]);

                            successfulFor(newList);
                        }
                        else if (objectExists(before) && after == "get_methods")
                        {
                            List newList = new();

                            System.Collections.Generic.List<Method> objMethods = objects[indexOfObject(before)].getMethods();

                            for (int i = 0; i < objMethods.Count; i++)
                                newList.add(objMethods[i].name());

                            successfulFor(newList);
                        }
                        else if (objectExists(before) && after == "get_variables")
                        {
                            List newList = new();

                            System.Collections.Generic.List<Variable> objVars = objects[indexOfObject(before)].getVariables();

                            for (int i = 0; i < objVars.Count; i++)
                                newList.add(objVars[i].name());

                            successfulFor(newList);
                        }
                        else if (variableExists(before) && after == "length")
                        {
                            if (isString(before))
                            {
                                List newList = new();
                                string tempVarStr = variables[indexOfVariable(before)].getString();
                                int len = tempVarStr.Length;

                                for (int i = 0; i < len; i++)
                                {
                                    string tempStr = (string.Empty);
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
                                if (variableExists(before))
                                {
                                    if (after == "get_dirs")
                                    {
                                        if (System.IO.Directory.Exists(variables[indexOfVariable(before)].getString()))
                                            successfulFor(getDirectoryList(before, false));
                                        else
                                        {
                                            error(ErrorLogger.READ_FAIL, variables[indexOfVariable(before)].getString(), false);
                                            failedFor();
                                        }
                                    }
                                    else if (after == "get_files")
                                    {
                                        if (System.IO.Directory.Exists(variables[indexOfVariable(before)].getString()))
                                            successfulFor(getDirectoryList(before, true));
                                        else
                                        {
                                            error(ErrorLogger.READ_FAIL, variables[indexOfVariable(before)].getString(), false);
                                            failedFor();
                                        }
                                    }
                                    else if (after == "read")
                                    {
                                        if (System.IO.File.Exists(variables[indexOfVariable(before)].getString()))
                                        {
                                            List newList = new();
                                            foreach (var line in System.IO.File.ReadAllLines(variables[indexOfVariable(before)].getString()))
                                            {
                                                newList.add(line);
                                            }
                                            successfulFor(newList);
                                        }
                                        else
                                        {
                                            error(ErrorLogger.READ_FAIL, variables[indexOfVariable(before)].getString(), false);
                                            failedFor();
                                        }
                                    }
                                    else
                                    {
                                        error(ErrorLogger.METHOD_UNDEFINED, after, false);
                                        failedFor();
                                    }
                                }
                                else
                                {
                                    error(ErrorLogger.VAR_UNDEFINED, before, false);
                                    failedFor();
                                }
                            }
                            else
                            {
                                if (listExists(arg3))
                                    successfulFor(lists[indexOfList(arg3)]);
                                else
                                {
                                    error(ErrorLogger.LIST_UNDEFINED, arg3, false);
                                    failedFor();
                                }
                            }
                        }
                    }
                    else if (containsParameters(arg3))
                    {
                        System.Collections.Generic.List<string> rangeSpecifiers = new();

                        rangeSpecifiers = getRange(arg3);

                        if (rangeSpecifiers.Count == 2)
                        {
                            string firstRangeSpecifier = (rangeSpecifiers[0]), lastRangeSpecifier = (rangeSpecifiers[1]);

                            if (variableExists(firstRangeSpecifier))
                            {
                                if (isNumber(firstRangeSpecifier))
                                    firstRangeSpecifier = dtos(variables[indexOfVariable(firstRangeSpecifier)].getNumber());
                                else
                                    failedFor();
                            }

                            if (variableExists(lastRangeSpecifier))
                            {
                                if (isNumber(lastRangeSpecifier))
                                    lastRangeSpecifier = dtos(variables[indexOfVariable(lastRangeSpecifier)].getNumber());
                                else
                                    failedFor();
                            }

                            if (isNumeric(firstRangeSpecifier) && isNumeric(lastRangeSpecifier))
                            {
                                __DefaultLoopSymbol = arg1;

                                int ifrs = stoi(firstRangeSpecifier), ilrs = (stoi(lastRangeSpecifier));

                                if (ifrs < ilrs)
                                    successfulFor(stod(firstRangeSpecifier), stod(lastRangeSpecifier), "<=");
                                else if (ifrs > ilrs)
                                    successfulFor(stod(firstRangeSpecifier), stod(lastRangeSpecifier), ">=");
                                else
                                    failedFor();
                            }
                            else
                                failedFor();
                        }
                    }
                    else if (containsBrackets(arg3))
                    {
                        string before = (beforeBrackets(arg3));

                        if (variableExists(before))
                        {
                            if (isString(before))
                            {
                                string tempVarString = (variables[indexOfVariable(before)].getString());

                                System.Collections.Generic.List<string> range = getBracketRange(arg3);

                                if (range.Count == 2)
                                {
                                    string rangeBegin = (range[0]), rangeEnd = (range[1]);

                                    if (rangeBegin.Length != 0 && rangeEnd.Length != 0)
                                    {
                                        if (isNumeric(rangeBegin) && isNumeric(rangeEnd))
                                        {
                                            if (stoi(rangeBegin) < stoi(rangeEnd))
                                            {
                                                if ((int)tempVarString.Length >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                                {
                                                    List newList = new("&l&i&s&t&");

                                                    for (int i = stoi(rangeBegin); i <= stoi(rangeEnd); i++)
                                                    {
                                                        string tempString = (string.Empty);
                                                        tempString += (tempVarString[i]);
                                                        newList.add(tempString);
                                                    }

                                                    __DefaultLoopSymbol = arg1;

                                                    successfulFor(newList);

                                                    lists = removeList(lists, "&l&i&s&t&");
                                                }
                                                else
                                                    error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                            }
                                            else if (stoi(rangeBegin) > stoi(rangeEnd))
                                            {
                                                if ((int)tempVarString.Length >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                                {
                                                    List newList = new("&l&i&s&t&");

                                                    for (int i = stoi(rangeBegin); i >= stoi(rangeEnd); i--)
                                                    {
                                                        string tempString = (string.Empty);
                                                        tempString += (tempVarString[i]);
                                                        newList.add(tempString);
                                                    }

                                                    __DefaultLoopSymbol = arg1;

                                                    successfulFor(newList);

                                                    lists = removeList(lists, "&l&i&s&t&");
                                                }
                                                else
                                                    error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                            }
                                            else
                                                error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                        }
                                        else
                                            error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                    }
                                    else
                                        error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                }
                                else
                                    error(ErrorLogger.OUT_OF_BOUNDS, arg3, false);
                            }
                            else
                            {
                                error(ErrorLogger.NULL_STRING, before, false);
                                failedFor();
                            }
                        }
                    }
                    else if (listExists(arg3))
                    {
                        __DefaultLoopSymbol = arg1;

                        successfulFor(lists[indexOfList(arg3)]);
                    }
                    else if (!zeroDots(arg3))
                    {
                        string _b = (beforeDot(arg3)), _a = (afterDot(arg3));

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
                        else if (objectExists(_b) && _a == "get_methods")
                        {
                            List newList = new();

                            System.Collections.Generic.List<Method> objMethods = objects[indexOfObject(_b)].getMethods();

                            for (int i = 0; i < (int)objMethods.Count; i++)
                                newList.add(objMethods[i].name());

                            __DefaultLoopSymbol = arg1;
                            successfulFor(newList);
                        }
                        else if (objectExists(_b) && _a == "get_variables")
                        {
                            List newList = new();

                            System.Collections.Generic.List<Variable> objVars = objects[indexOfObject(_b)].getVariables();

                            for (int i = 0; i < (int)objVars.Count; i++)
                                newList.add(objVars[i].name());

                            __DefaultLoopSymbol = arg1;
                            successfulFor(newList);
                        }
                        else if (variableExists(_b) && _a == "length")
                        {
                            if (isString(_b))
                            {
                                __DefaultLoopSymbol = arg1;
                                List newList = new();
                                string _t = variables[indexOfVariable(_b)].getString();
                                int _l = _t.Length;

                                for (int i = 0; i < _l; i++)
                                {
                                    string tmpStr = (string.Empty);
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
                                if (variableExists(_b))
                                {
                                    if (_a == "get_dirs")
                                    {
                                        if (System.IO.Directory.Exists(variables[indexOfVariable(_b)].getString()))
                                        {
                                            __DefaultLoopSymbol = arg1;
                                            successfulFor(getDirectoryList(_b, false));
                                        }
                                        else
                                        {
                                            error(ErrorLogger.READ_FAIL, variables[indexOfVariable(_b)].getString(), false);
                                            failedFor();
                                        }
                                    }
                                    else if (_a == "get_files")
                                    {
                                        if (System.IO.Directory.Exists(variables[indexOfVariable(_b)].getString()))
                                        {
                                            __DefaultLoopSymbol = arg1;
                                            successfulFor(getDirectoryList(_b, true));
                                        }
                                        else
                                        {
                                            error(ErrorLogger.READ_FAIL, variables[indexOfVariable(_b)].getString(), false);
                                            failedFor();
                                        }
                                    }
                                    else if (_a == "read")
                                    {
                                        if (System.IO.File.Exists(variables[indexOfVariable(_b)].getString()))
                                        {
                                            List newList = new();

                                            foreach (var line in System.IO.File.ReadAllLines(variables[indexOfVariable(_b)].getString()))
                                            {
                                                newList.add(line);
                                            }

                                            __DefaultLoopSymbol = arg1;
                                            successfulFor(newList);
                                        }
                                        else
                                        {
                                            error(ErrorLogger.READ_FAIL, variables[indexOfVariable(_b)].getString(), false);
                                            failedFor();
                                        }
                                    }
                                    else
                                    {
                                        error(ErrorLogger.METHOD_UNDEFINED, _a, false);
                                        failedFor();
                                    }
                                }
                                else
                                {
                                    error(ErrorLogger.VAR_UNDEFINED, _b, false);
                                    failedFor();
                                }
                            }
                        }
                    }
                    else
                    {
                        error(ErrorLogger.INVALID_OP, s, false);
                        failedFor();
                    }
                }
                else
                {
                    error(ErrorLogger.INVALID_OP, s, false);
                    failedFor();
                }
            }
            else if (arg0 == "while")
            {
                if (variableExists(arg1) && variableExists(arg3))
                {
                    if (isNumber(arg1) && isNumber(arg3))
                    {
                        if (arg2 == "<" || arg2 == "<=" || arg2 == ">=" || arg2 == ">" || arg2 == "==" || arg2 == "!=")
                            successfullWhile(arg1, arg2, arg3);
                        else
                        {
                            error(ErrorLogger.INVALID_OP, s, false);
                            failedWhile();
                        }
                    }
                    else
                    {
                        error(ErrorLogger.CONV_ERR, arg1 + arg2 + arg3, false);
                        failedWhile();
                    }
                }
                else if (isNumeric(arg3) && variableExists(arg1))
                {
                    if (isNumber(arg1))
                    {
                        if (arg2 == "<" || arg2 == "<=" || arg2 == ">=" || arg2 == ">" || arg2 == "==" || arg2 == "!=")
                            successfullWhile(arg1, arg2, arg3);
                        else
                        {
                            error(ErrorLogger.INVALID_OP, s, false);
                            failedWhile();
                        }
                    }
                    else
                    {
                        error(ErrorLogger.CONV_ERR, arg1 + arg2 + arg3, false);
                        failedWhile();
                    }
                }
                else if (isNumeric(arg1) && isNumeric(arg3))
                {
                    if (arg2 == "<" || arg2 == "<=" || arg2 == ">=" || arg2 == ">" || arg2 == "==" || arg2 == "!=")
                        successfullWhile(arg1, arg2, arg3);
                    else
                    {
                        error(ErrorLogger.INVALID_OP, s, false);
                        failedWhile();
                    }
                }
                else
                {
                    error(ErrorLogger.INVALID_OP, s, false);
                    failedWhile();
                }
            }
            else
                sysExec(s, command);
        }
        #endregion
    }
}
