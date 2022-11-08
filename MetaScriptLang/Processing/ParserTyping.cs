namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;

    public partial class Parser
    {
        #region Dynamic Typing
        bool IsNumberVariable(Variable var)
        {
            return var.getNumber() != __NullNum;
        }

        bool IsNumberVariable(string varName)
        {
            return GetVariableNumber(varName) != __NullNum;
        }

        bool IsStringVariable(Variable var)
        {
            return var.getString() != __Null;
        }

        bool IsStringVariable(string varName)
        {
            return GetVariableString(varName) != __Null;
        }
        #endregion

        #region String Stack
        bool secondIsNumber(string s)
        {
            if (VariableExists(s))
            {
                if (IsNumberVariable(s))
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
            if (StringHelper.ContainsString(arg2, "+") || StringHelper.ContainsString(arg2, "-") || StringHelper.ContainsString(arg2, "*") || StringHelper.ContainsString(arg2, "/") || StringHelper.ContainsString(arg2, "%") || StringHelper.ContainsString(arg2, "^"))
                return (true);

            return (false);
        }

        bool isStringStack(string arg2)
        {
            System.Text.StringBuilder temporaryBuild = new();
            string tempArgTwo = arg2;
            tempArgTwo = StringHelper.SubtractChars(tempArgTwo, "(");
            tempArgTwo = StringHelper.SubtractChars(tempArgTwo, ")");

            for (int i = 0; i < (int)tempArgTwo.Length; i++)
            {
                if (tempArgTwo[i] == ' ')
                {
                    if (temporaryBuild.Length != 0)
                    {
                        if (VariableExists(temporaryBuild.ToString()))
                        {
                            if (IsNumberVariable(temporaryBuild.ToString()))
                                temporaryBuild.Clear();
                            else if (IsStringVariable(temporaryBuild.ToString()))
                                return (true);
                        }
                        else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (IsStringVariable(temporaryBuild.ToString()))
                            return (true);
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (IsStringVariable(temporaryBuild.ToString()))
                            return (true);
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (IsStringVariable(temporaryBuild.ToString()))
                            return (true);
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (IsStringVariable(temporaryBuild.ToString()))
                            return (true);
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (IsStringVariable(temporaryBuild.ToString()))
                            return (true);
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (IsStringVariable(temporaryBuild.ToString()))
                            return (true);
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
            tempArgTwo = StringHelper.SubtractChars(tempArgTwo, "(");
            tempArgTwo = StringHelper.SubtractChars(tempArgTwo, ")");

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
                            if (VariableExists(temporaryBuild.ToString()))
                            {
                                if (IsNumberVariable(temporaryBuild.ToString()))
                                {
                                    vars.Add(temporaryBuild.ToString());
                                    contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
                                    temporaryBuild.Clear();
                                }
                                else if (IsStringVariable(temporaryBuild.ToString()))
                                {
                                    vars.Add(temporaryBuild.ToString());
                                    contents.Add(GetVariableString(temporaryBuild.ToString()));
                                    temporaryBuild.Clear();
                                }
                            }
                            else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("+");
                        }
                        else if (IsStringVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(GetVariableString(temporaryBuild.ToString()));
                            temporaryBuild.Clear();
                            contents.Add("+");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("-");
                        }
                        else if (IsStringVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(GetVariableString(temporaryBuild.ToString()));
                            temporaryBuild.Clear();
                            contents.Add("-");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("*");
                        }
                        else if (IsStringVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(GetVariableString(temporaryBuild.ToString()));
                            temporaryBuild.Clear();
                            contents.Add("*");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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

            if (VariableExists(temporaryBuild.ToString()))
            {
                if (IsNumberVariable(temporaryBuild.ToString()))
                {
                    vars.Add(temporaryBuild.ToString());
                    contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
                    temporaryBuild.Clear();
                }
                else if (IsStringVariable(temporaryBuild.ToString()))
                {
                    vars.Add(temporaryBuild.ToString());
                    contents.Add(GetVariableString(temporaryBuild.ToString()));
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
                        stackValue = StringHelper.SubtractString(stackValue, contents[i]);
                        subtractNext = false;
                    }
                    else if (multiplyNext)
                    {
                        if (StringHelper.IsNumeric(contents[i]))
                        {
                            string appendage = stackValue;

                            for (int z = 1; z < StringHelper.StoI(contents[i]); z++)
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
                    engine.DeleteVariable(vars[i]);

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
            tempArgTwo = StringHelper.SubtractChars(tempArgTwo, "(");
            tempArgTwo = StringHelper.SubtractChars(tempArgTwo, ")");

            double stackValue = (double)0.0;

            System.Collections.Generic.List<string> contents = new();
            System.Collections.Generic.List<string> vars = new();

            for (int i = 0; i < (int)tempArgTwo.Length; i++)
            {
                if (tempArgTwo[i] == ' ')
                {
                    if (temporaryBuild.Length != 0)
                    {
                        if (VariableExists(temporaryBuild.ToString()))
                        {
                            if (IsNumberVariable(temporaryBuild.ToString()))
                            {
                                vars.Add(temporaryBuild.ToString());
                                contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
                                temporaryBuild.Clear();
                            }
                        }
                        else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("+");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("-");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                        {
                            contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("*");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("/");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("%");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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
                    if (VariableExists(temporaryBuild.ToString()))
                    {
                        if (IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("^");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
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

            if (VariableExists(temporaryBuild.ToString()))
            {
                if (IsNumberVariable(temporaryBuild.ToString()))
                {
                    vars.Add(temporaryBuild.ToString());
                    contents.Add(StringHelper.DtoS(GetVariableNumber(temporaryBuild.ToString())));
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
                        stackValue += StringHelper.StoD(contents[i]);
                        addNext = false;
                    }
                    else if (subtractNext)
                    {
                        stackValue -= StringHelper.StoD(contents[i]);
                        subtractNext = false;
                    }
                    else if (multiplyNext)
                    {
                        stackValue *= StringHelper.StoD(contents[i]);
                        multiplyNext = false;
                    }
                    else if (divideNext)
                    {
                        stackValue /= StringHelper.StoD(contents[i]);
                        divideNext = false;
                    }
                    else if (moduloNext)
                    {
                        stackValue = ((int)stackValue % (int)StringHelper.StoD(contents[i]));
                        moduloNext = false;
                    }
                    else if (powerNext)
                    {
                        stackValue = System.Math.Pow(stackValue, (int)StringHelper.StoD(contents[i]));
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
                        stackValue = StringHelper.StoD(contents[i]);
                    }
                }
            }

            if (__Returning)
            {
                for (int i = 0; i < (int)vars.Count; i++)
                    engine.DeleteVariable(vars[i]);

                __Returning = false;
            }

            return (stackValue);
        }
        #endregion
    }
}
