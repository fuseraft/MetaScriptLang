namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;

    public partial class Parser
    {
        #region Dynamic Typing
        bool isNumber(Variable var)
        {
            return var.getNumber() != __NullNum;
        }

        bool isNumber(string varName)
        {
            return GetVNumber(varName) != __NullNum;
        }

        bool isString(Variable var)
        {
            return var.getString() != __Null;
        }

        bool isString(string varName)
        {
            return GetVString(varName) != __Null;
        }
        #endregion

        #region Get String Value
        string getStringValue(string arg1, string op, string arg2)
        {
            string firstValue = (""), lastValue = (""), returnValue = ("");

            if (VExists(arg1))
            {
                if (isString(arg1))
                    firstValue = GetVString(arg1);
            }

            if (VExists(arg2))
            {
                if (isString(arg2))
                    lastValue = GetVString(arg2);
                else if (isNumber(arg2))
                    lastValue = dtos(GetVNumber(arg2));
            }
            else if (MExists(arg2))
            {
                parse(arg2);

                //lastValue = lastValue;
            }
            else if (!zeroDots(arg2))
            {
                string _beforeDot = (beforeDot(arg2)), _afterDot = (afterDot(arg2));

                if (_beforeDot == "env")
                {
                    InternalGetEnv("", _afterDot, 2);
                }
                else if (_beforeDot == "args")
                {
                    if (_afterDot == "size")
                        lastValue = itos(args.Count);
                    else
                        lastValue = "";
                }
                else if (objectExists(_beforeDot))
                {
                    executeTemplate(objects[indexOfObject(_beforeDot)].getMethod(_afterDot), getParameters(_afterDot));

                    lastValue = lastValue;
                }
                else
                    lastValue = arg2;
            }
            else if (containsBrackets(arg2))
            {
                string _beforeBrackets = (beforeBrackets(arg2)), _afterBrackets = (afterBrackets(arg2));

                if (_beforeBrackets == "args")
                {
                    System.Collections.Generic.List<string> parameters = getBracketRange(_afterBrackets);

                    if (StringHelper.IsNumeric(parameters[0]))
                    {
                        if ((int)args.Count - 1 >= stoi(parameters[0]) && stoi(parameters[0]) >= 0)
                        {
                            if (parameters[0] == "0")
                                lastValue = __CurrentScript;
                            else
                                lastValue = args[stoi(parameters[0])];
                        }
                        else
                            lastValue = "";
                    }
                    else
                        lastValue = "";
                }
                else if (listExists(_beforeBrackets))
                {
                    _afterBrackets = subtractString(_afterBrackets, "]");

                    if (lists[indexOfList(_beforeBrackets)].size() >= stoi(_afterBrackets))
                    {
                        if (stoi(_afterBrackets) >= 0)
                            lastValue = lists[indexOfList(_beforeBrackets)].at(stoi(_afterBrackets));
                        else
                            lastValue = "";
                    }
                    else
                        lastValue = "";
                }
            }
            else if (containsParameters(arg2))
            {
                if (beforeParameters(arg2).Length != 0)
                {
                    executeTemplate(GetM(arg2), getParameters(arg2));

                    //lastValue = lastValue;
                }
                else
                {
                    if (isStringStack(arg2))
                        lastValue = getStringStack(arg2);
                    else if (stackReady(arg2))
                        lastValue = dtos(getStack(arg2));
                }
            }
            else
                lastValue = arg2;

            if (op == "+=")
                returnValue = (firstValue + lastValue);
            else if (op == "-=")
                returnValue = subtractString(firstValue, lastValue);
            else if (op == "*=")
            {
                if (StringHelper.IsNumeric(lastValue))
                {
                    string bigString = ("");

                    for (int i = 0; i < (int)stod(lastValue); i++)
                        bigString += (firstValue);

                    returnValue = bigString;
                }
            }
            else if (op == "/=")
                returnValue = subtractString(firstValue, lastValue);
            else if (op == "**=")
                returnValue = dtos(System.Math.Pow(stod(firstValue), stod(lastValue)));
            else if (op == "=")
                returnValue = lastValue;

            setLastValue(returnValue);
            return returnValue;
        }
        #endregion

        #region Get Number Value
        double getNumberValue(string arg1, string op, string arg2)
        {
            double firstValue = 0, lastValue = 0, returnValue = 0;

            if (VExists(arg1))
            {
                if (isNumber(arg1))
                    firstValue = GetVNumber(arg1);
            }

            if (VExists(arg2))
            {
                if (isNumber(arg2))
                    lastValue = GetVNumber(arg2);
                else
                    lastValue = 0;
            }
            else if (MExists(arg2))
            {
                parse(arg2);

                if (StringHelper.IsNumeric(__LastValue))
                    lastValue = stod(__LastValue);
                else
                    lastValue = 0;
            }
            else if (!zeroDots(arg2))
            {
                string _beforeDot = (beforeDot(arg2)), _afterDot = (afterDot(arg2));
                if (_beforeDot == "env")
                {
                    InternalGetEnv("", _afterDot, 2);
                }
                else if (_beforeDot == "args")
                {
                    if (_afterDot == "size")
                        lastValue = stod(itos(args.Count));
                    else
                        lastValue = 0;
                }
                else if (objectExists(_beforeDot))
                {
                    executeTemplate(objects[indexOfObject(_beforeDot)].getMethod(_afterDot), getParameters(_afterDot));

                    if (StringHelper.IsNumeric(__LastValue))
                        lastValue = stod(__LastValue);
                    else
                        lastValue = 0;
                }
                else
                {
                    if (StringHelper.IsNumeric(__LastValue))
                        lastValue = stod(arg2);
                    else
                        lastValue = 0;
                }
            }
            else if (containsBrackets(arg2))
            {
                string _beforeBrackets = (beforeBrackets(arg2)), _afterBrackets = (afterBrackets(arg2));

                if (listExists(_beforeBrackets))
                {
                    _afterBrackets = subtractString(_afterBrackets, "]");

                    if (lists[indexOfList(_beforeBrackets)].size() >= stoi(_afterBrackets))
                    {
                        if (stoi(_afterBrackets) >= 0)
                        {
                            if (StringHelper.IsNumeric(lists[indexOfList(_beforeBrackets)].at(stoi(_afterBrackets))))
                                lastValue = stod(lists[indexOfList(_beforeBrackets)].at(stoi(_afterBrackets)));
                            else
                                lastValue = 0;
                        }
                        else
                            lastValue = 0;
                    }
                    else
                        lastValue = 0;
                }
            }
            else if (containsParameters(arg2))
            {
                if (beforeParameters(arg2).Length != 0)
                {
                    executeTemplate(GetM(arg2), getParameters(arg2));

                    if (StringHelper.IsNumeric(__LastValue))

                        lastValue = stod(__LastValue);
                    else
                        lastValue = 0;
                }
                else
                {
                    if (stackReady(arg2))
                        lastValue = getStack(arg2);
                    else
                        lastValue = 0;
                }
            }
            else
            {
                if (StringHelper.IsNumeric(arg2))
                    lastValue = stod(arg2);
                else
                    lastValue = 0;
            }

            if (op == "+=")
                returnValue = (firstValue + lastValue);
            else if (op == "-=")
                returnValue = (firstValue - lastValue);
            else if (op == "*=")
                returnValue = (firstValue * lastValue);
            else if (op == "/=")
                returnValue = (firstValue / lastValue);
            else if (op == "**=")
                returnValue = System.Math.Pow(firstValue, lastValue);
            else if (op == "=")
                returnValue = lastValue;

            setLastValue(dtos(returnValue));
            return (returnValue);
        }
        #endregion

        #region String Stack
        bool secondIsNumber(string s)
        {
            if (VExists(s))
            {
                if (isNumber(s))
                    return (true);
            }
            else if (stackReady(s))
            {
                if (!isStringStack(s))
                    return (true);
            }
            else
            {
                if (StringHelper.IsNumeric(s))
                    return (true);
            }

            return (false);
        }

        bool stackReady(string arg2)
        {
            if (contains(arg2, "+") || contains(arg2, "-") || contains(arg2, "*") || contains(arg2, "/") || contains(arg2, "%") || contains(arg2, "^"))
                return (true);

            return (false);
        }

        bool isStringStack(string arg2)
        {
            System.Text.StringBuilder temporaryBuild = new();
            string tempArgTwo = arg2;
            tempArgTwo = subtractChar(tempArgTwo, "(");
            tempArgTwo = subtractChar(tempArgTwo, ")");

            for (int i = 0; i < (int)tempArgTwo.Length; i++)
            {
                if (tempArgTwo[i] == ' ')
                {
                    if (temporaryBuild.Length != 0)
                    {
                        if (VExists(temporaryBuild.ToString()))
                        {
                            if (isNumber(temporaryBuild.ToString()))
                                temporaryBuild.Clear();
                            else if (isString(temporaryBuild.ToString()))
                                return (true);
                        }
                        else if (MExists(temporaryBuild.ToString()))
                        {
                            parse(temporaryBuild.ToString());

                            if (StringHelper.IsNumeric(__LastValue))
                                temporaryBuild.Clear();
                            else
                                return (true);
                        }
                        else
                            temporaryBuild.Clear();
                    }
                }
                else if (tempArgTwo[i] == '+')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (isString(temporaryBuild.ToString()))
                            return (true);
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                            temporaryBuild.Clear();
                        else
                            return (true);
                    }
                    else if (!StringHelper.IsNumeric(temporaryBuild.ToString()))
                        return (true);
                    else
                        temporaryBuild.Clear();
                }
                else if (tempArgTwo[i] == '-')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (isString(temporaryBuild.ToString()))
                            return (true);
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                            temporaryBuild.Clear();
                        else
                            return (true);
                    }
                    else if (!StringHelper.IsNumeric(temporaryBuild.ToString()))
                        return (true);
                    else
                        temporaryBuild.Clear();
                }
                else if (tempArgTwo[i] == '*')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (isString(temporaryBuild.ToString()))
                            return (true);
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                            temporaryBuild.Clear();
                        else
                            return (true);
                    }
                    else if (!StringHelper.IsNumeric(temporaryBuild.ToString()))
                        return (true);
                    else
                        temporaryBuild.Clear();
                }
                else if (tempArgTwo[i] == '/')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (isString(temporaryBuild.ToString()))
                            return (true);
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                            temporaryBuild.Clear();
                        else
                            return (true);
                    }
                    else if (!StringHelper.IsNumeric(temporaryBuild.ToString()))
                        return (true);
                    else
                        temporaryBuild.Clear();
                }
                else if (tempArgTwo[i] == '%')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (isString(temporaryBuild.ToString()))
                            return (true);
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                            temporaryBuild.Clear();
                        else
                            return (true);
                    }
                    else if (!StringHelper.IsNumeric(temporaryBuild.ToString()))
                        return (true);
                    else
                        temporaryBuild.Clear();
                }
                else if (tempArgTwo[i] == '^')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (isString(temporaryBuild.ToString()))
                            return (true);
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                            temporaryBuild.Clear();
                        else
                            return (true);
                    }
                    else if (!StringHelper.IsNumeric(temporaryBuild.ToString()))
                        return (true);
                    else
                        temporaryBuild.Clear();
                }
                else
                    temporaryBuild.Append(tempArgTwo[i]);
            }

            return (false);
        }

        string getStringStack(string arg2)
        {
            System.Text.StringBuilder temporaryBuild = new();
            string tempArgTwo = arg2;
            tempArgTwo = subtractChar(tempArgTwo, "(");
            tempArgTwo = subtractChar(tempArgTwo, ")");

            string stackValue = ("");

            System.Collections.Generic.List<string> vars = new();
            System.Collections.Generic.List<string> contents = new();

            bool quoted = false;

            for (int i = 0; i < (int)tempArgTwo.Length; i++)
            {
                if (tempArgTwo[i] == '\"')
                {
                    quoted = !quoted;
                    if (!quoted)
                    {
                        contents.Add(temporaryBuild.ToString());
                        temporaryBuild.Clear();
                    }
                }
                else if (tempArgTwo[i] == ' ')
                {
                    if (quoted)
                    {
                        temporaryBuild.Append(' ');
                    }
                    else
                    {
                        if (temporaryBuild.Length != 0)
                        {
                            if (VExists(temporaryBuild.ToString()))
                            {
                                if (isNumber(temporaryBuild.ToString()))
                                {
                                    vars.Add(temporaryBuild.ToString());
                                    contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                                    temporaryBuild.Clear();
                                }
                                else if (isString(temporaryBuild.ToString()))
                                {
                                    vars.Add(temporaryBuild.ToString());
                                    contents.Add(GetVString(temporaryBuild.ToString()));
                                    temporaryBuild.Clear();
                                }
                            }
                            else if (MExists(temporaryBuild.ToString()))
                            {
                                parse(temporaryBuild.ToString());

                                contents.Add(__LastValue);
                                temporaryBuild.Clear();
                            }
                            else
                            {
                                contents.Add(temporaryBuild.ToString());
                                temporaryBuild.Clear();
                            }
                        }
                    }
                }
                else if (tempArgTwo[i] == '+')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("+");
                        }
                        else if (isString(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(GetVString(temporaryBuild.ToString()));
                            temporaryBuild.Clear();
                            contents.Add("+");
                        }
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        contents.Add(__LastValue);
                        temporaryBuild.Clear();

                        contents.Add("+");
                    }
                    else
                    {
                        contents.Add(temporaryBuild.ToString());
                        temporaryBuild.Clear();
                        contents.Add("+");
                    }
                }
                else if (tempArgTwo[i] == '-')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("-");
                        }
                        else if (isString(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(GetVString(temporaryBuild.ToString()));
                            temporaryBuild.Clear();
                            contents.Add("-");
                        }
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        contents.Add(__LastValue);
                        temporaryBuild.Clear();

                        contents.Add("-");
                    }
                    else
                    {
                        contents.Add(temporaryBuild.ToString());
                        temporaryBuild.Clear();
                        contents.Add("-");
                    }
                }
                else if (tempArgTwo[i] == '*')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("*");
                        }
                        else if (isString(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(GetVString(temporaryBuild.ToString()));
                            temporaryBuild.Clear();
                            contents.Add("*");
                        }
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        contents.Add(__LastValue);
                        temporaryBuild.Clear();

                        contents.Add("*");
                    }
                    else
                    {
                        contents.Add(temporaryBuild.ToString());
                        temporaryBuild.Clear();
                        contents.Add("*");
                    }
                }
                else
                    temporaryBuild.Append(tempArgTwo[i]);
            }

            if (VExists(temporaryBuild.ToString()))
            {
                if (isNumber(temporaryBuild.ToString()))
                {
                    vars.Add(temporaryBuild.ToString());
                    contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                    temporaryBuild.Clear();
                }
                else if (isString(temporaryBuild.ToString()))
                {
                    vars.Add(temporaryBuild.ToString());
                    contents.Add(GetVString(temporaryBuild.ToString()));
                    temporaryBuild.Clear();
                }
            }
            else
            {
                contents.Add(temporaryBuild.ToString());
                temporaryBuild.Clear();
            }

            bool startOperating = false,
                 addNext = false,
                 subtractNext = false,
                 multiplyNext = false;

            for (int i = 0; i < (int)contents.Count; i++)
            {
                if (startOperating)
                {
                    if (addNext)
                    {
                        stackValue += contents[i];
                        addNext = false;
                    }
                    else if (subtractNext)
                    {
                        stackValue = subtractString(stackValue, contents[i]);
                        subtractNext = false;
                    }
                    else if (multiplyNext)
                    {
                        if (StringHelper.IsNumeric(contents[i]))
                        {
                            string appendage = stackValue;

                            for (int z = 1; z < stoi(contents[i]); z++)
                                stackValue += appendage;
                        }

                        multiplyNext = false;
                    }

                    if (contents[i] == "+")
                        addNext = true;
                    else if (contents[i] == "-")
                        subtractNext = true;
                    else if (contents[i] == "*")
                        multiplyNext = true;
                }
                else
                {
                    startOperating = true;
                    stackValue = contents[i];
                }
            }

            if (__Returning)
            {
                for (int i = 0; i < (int)vars.Count; i++)
                    DeleteV(vars[i]);

                __Returning = false;
            }

            return (stackValue);
        }
        #endregion

        #region Stack
        double getStack(string arg2)
        {
            System.Text.StringBuilder temporaryBuild = new();
            string tempArgTwo = arg2; 
            tempArgTwo = subtractChar(tempArgTwo, "(");
            tempArgTwo = subtractChar(tempArgTwo, ")");

            double stackValue = (double)0.0;

            System.Collections.Generic.List<string> contents = new();
            System.Collections.Generic.List<string> vars = new();

            for (int i = 0; i < (int)tempArgTwo.Length; i++)
            {
                if (tempArgTwo[i] == ' ')
                {
                    if (temporaryBuild.Length != 0)
                    {
                        if (VExists(temporaryBuild.ToString()))
                        {
                            if (isNumber(temporaryBuild.ToString()))
                            {
                                vars.Add(temporaryBuild.ToString());
                                contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                                temporaryBuild.Clear();
                            }
                        }
                        else if (MExists(temporaryBuild.ToString()))
                        {
                            parse(temporaryBuild.ToString());

                            if (StringHelper.IsNumeric(__LastValue))
                            {
                                contents.Add(__LastValue);
                                temporaryBuild.Clear();
                            }
                        }
                        else
                        {
                            contents.Add(temporaryBuild.ToString());
                            temporaryBuild.Clear();
                        }
                    }
                }
                else if (tempArgTwo[i] == '+')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("+");
                        }
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                        {
                            contents.Add(__LastValue);
                            temporaryBuild.Clear();
                        }

                        contents.Add("+");
                    }
                    else
                    {
                        contents.Add(temporaryBuild.ToString());
                        temporaryBuild.Clear();
                        contents.Add("+");
                    }
                }
                else if (tempArgTwo[i] == '-')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("-");
                        }
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                        {
                            contents.Add(__LastValue);
                            temporaryBuild.Clear();
                        }
                        contents.Add("-");
                    }
                    else
                    {
                        contents.Add(temporaryBuild.ToString());
                        temporaryBuild.Clear();
                        contents.Add("-");
                    }
                }
                else if (tempArgTwo[i] == '*')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                        {
                            contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("*");
                        }
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                        {
                            contents.Add(__LastValue);
                            temporaryBuild.Clear();
                        }

                        contents.Add("*");
                    }
                    else
                    {
                        contents.Add(temporaryBuild.ToString());
                        temporaryBuild.Clear();
                        contents.Add("*");
                    }
                }
                else if (tempArgTwo[i] == '/')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("/");
                        }
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                        {
                            contents.Add(__LastValue);
                            temporaryBuild.Clear();
                        }

                        contents.Add("/");
                    }
                    else
                    {
                        contents.Add(temporaryBuild.ToString());
                        temporaryBuild.Clear();
                        contents.Add("/");
                    }
                }
                else if (tempArgTwo[i] == '%')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("%");
                        }
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                        {
                            contents.Add(__LastValue);
                            temporaryBuild.Clear();
                        }
                        contents.Add("%");
                    }
                    else
                    {
                        contents.Add(temporaryBuild.ToString());
                        temporaryBuild.Clear();
                        contents.Add("%");
                    }
                }
                else if (tempArgTwo[i] == '^')
                {
                    if (VExists(temporaryBuild.ToString()))
                    {
                        if (isNumber(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("^");
                        }
                    }
                    else if (MExists(temporaryBuild.ToString()))
                    {
                        parse(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                        {
                            contents.Add(__LastValue);
                            temporaryBuild.Clear();
                        }
                        contents.Add("^");
                    }
                    else
                    {
                        contents.Add(temporaryBuild.ToString());
                        temporaryBuild.Clear();
                        contents.Add("^");
                    }
                }
                else
                    temporaryBuild.Append(tempArgTwo[i]);
            }

            if (VExists(temporaryBuild.ToString()))
            {
                if (isNumber(temporaryBuild.ToString()))
                {
                    vars.Add(temporaryBuild.ToString());
                    contents.Add(dtos(GetVNumber(temporaryBuild.ToString())));
                    temporaryBuild.Clear();
                }
            }
            else
            {
                contents.Add(temporaryBuild.ToString());
                temporaryBuild.Clear();
            }

            bool startOperating = false,
                 addNext = false,
                 subtractNext = false,
                 multiplyNext = false,
                 divideNext = false,
                 moduloNext = false,
                 powerNext = false;

            for (int i = 0; i < (int)contents.Count; i++)
            {
                if (startOperating)
                {
                    if (addNext)
                    {
                        stackValue += stod(contents[i]);
                        addNext = false;
                    }
                    else if (subtractNext)
                    {
                        stackValue -= stod(contents[i]);
                        subtractNext = false;
                    }
                    else if (multiplyNext)
                    {
                        stackValue *= stod(contents[i]);
                        multiplyNext = false;
                    }
                    else if (divideNext)
                    {
                        stackValue /= stod(contents[i]);
                        divideNext = false;
                    }
                    else if (moduloNext)
                    {
                        stackValue = ((int)stackValue % (int)stod(contents[i]));
                        moduloNext = false;
                    }
                    else if (powerNext)
                    {
                        stackValue = System.Math.Pow(stackValue, (int)stod(contents[i]));
                        powerNext = false;
                    }

                    if (contents[i] == "+")
                        addNext = true;
                    else if (contents[i] == "-")
                        subtractNext = true;
                    else if (contents[i] == "*")
                        multiplyNext = true;
                    else if (contents[i] == "/")
                        divideNext = true;
                    else if (contents[i] == "%")
                        moduloNext = true;
                    else if (contents[i] == "^")
                        powerNext = true;
                }
                else
                {
                    if (StringHelper.IsNumeric(contents[i]))
                    {
                        startOperating = true;
                        stackValue = stod(contents[i]);
                    }
                }
            }

            if (__Returning)
            {
                for (int i = 0; i < (int)vars.Count; i++)
                    DeleteV(vars[i]);

                __Returning = false;
            }

            return (stackValue);
        }
        #endregion
    }
}
