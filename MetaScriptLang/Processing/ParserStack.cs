namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;

    public partial class Parser
    {
        #region String Stack
        bool IsStackReady(string arg2)
        {
            if (StringHelper.ContainsString(arg2, "+") || StringHelper.ContainsString(arg2, "-") || StringHelper.ContainsString(arg2, "*") || StringHelper.ContainsString(arg2, "/") || StringHelper.ContainsString(arg2, "%") || StringHelper.ContainsString(arg2, "^"))
                return true;

            return false;
        }

        bool IsStringStack(string arg2)
        {
            System.Text.StringBuilder temporaryBuild = new();
            string tempArgTwo = arg2;
            tempArgTwo = StringHelper.SubtractChars(tempArgTwo, "(");
            tempArgTwo = StringHelper.SubtractChars(tempArgTwo, ")");

            for (int i = 0; i < tempArgTwo.Length; i++)
            {
                if (tempArgTwo[i] == ' ')
                {
                    if (temporaryBuild.Length != 0)
                    {
                        if (engine.VariableExists(temporaryBuild.ToString()))
                        {
                            if (engine.IsNumberVariable(temporaryBuild.ToString()))
                                temporaryBuild.Clear();
                            else if (engine.IsStringVariable(temporaryBuild.ToString()))
                                return true;
                        }
                        else if (engine.MethodExists(temporaryBuild.ToString()))
                        {
                            ParseString(temporaryBuild.ToString());

                            if (StringHelper.IsNumeric(__LastValue))
                                temporaryBuild.Clear();
                            else
                                return true;
                        }
                        else
                            temporaryBuild.Clear();
                    }
                }
                else if (tempArgTwo[i] == '+')
                {
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (engine.IsStringVariable(temporaryBuild.ToString()))
                            return true;
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                            temporaryBuild.Clear();
                        else
                            return true;
                    }
                    else if (!StringHelper.IsNumeric(temporaryBuild.ToString()))
                        return true;
                    else
                        temporaryBuild.Clear();
                }
                else if (tempArgTwo[i] == '-')
                {
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (engine.IsStringVariable(temporaryBuild.ToString()))
                            return true;
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                            temporaryBuild.Clear();
                        else
                            return true;
                    }
                    else if (!StringHelper.IsNumeric(temporaryBuild.ToString()))
                        return true;
                    else
                        temporaryBuild.Clear();
                }
                else if (tempArgTwo[i] == '*')
                {
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (engine.IsStringVariable(temporaryBuild.ToString()))
                            return true;
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                            temporaryBuild.Clear();
                        else
                            return true;
                    }
                    else if (!StringHelper.IsNumeric(temporaryBuild.ToString()))
                        return true;
                    else
                        temporaryBuild.Clear();
                }
                else if (tempArgTwo[i] == '/')
                {
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (engine.IsStringVariable(temporaryBuild.ToString()))
                            return true;
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                            temporaryBuild.Clear();
                        else
                            return true;
                    }
                    else if (!StringHelper.IsNumeric(temporaryBuild.ToString()))
                        return true;
                    else
                        temporaryBuild.Clear();
                }
                else if (tempArgTwo[i] == '%')
                {
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (engine.IsStringVariable(temporaryBuild.ToString()))
                            return true;
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                            temporaryBuild.Clear();
                        else
                            return true;
                    }
                    else if (!StringHelper.IsNumeric(temporaryBuild.ToString()))
                        return true;
                    else
                        temporaryBuild.Clear();
                }
                else if (tempArgTwo[i] == '^')
                {
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                            temporaryBuild.Clear();
                        else if (engine.IsStringVariable(temporaryBuild.ToString()))
                            return true;
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

                        if (StringHelper.IsNumeric(__LastValue))
                            temporaryBuild.Clear();
                        else
                            return true;
                    }
                    else if (!StringHelper.IsNumeric(temporaryBuild.ToString()))
                        return true;
                    else
                        temporaryBuild.Clear();
                }
                else
                    temporaryBuild.Append(tempArgTwo[i]);
            }

            return false;
        }

        string GetStringStack(string arg2)
        {
            System.Text.StringBuilder temporaryBuild = new();
            string tempArgTwo = arg2;
            tempArgTwo = StringHelper.SubtractChars(tempArgTwo, "(");
            tempArgTwo = StringHelper.SubtractChars(tempArgTwo, ")");

            string stackValue = ("");

            System.Collections.Generic.List<string> vars = new();
            System.Collections.Generic.List<string> contents = new();

            bool quoted = false;

            for (int i = 0; i < tempArgTwo.Length; i++)
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
                            if (engine.VariableExists(temporaryBuild.ToString()))
                            {
                                if (engine.IsNumberVariable(temporaryBuild.ToString()))
                                {
                                    vars.Add(temporaryBuild.ToString());
                                    contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
                                    temporaryBuild.Clear();
                                }
                                else if (engine.IsStringVariable(temporaryBuild.ToString()))
                                {
                                    vars.Add(temporaryBuild.ToString());
                                    contents.Add(engine.GetVariableString(temporaryBuild.ToString()));
                                    temporaryBuild.Clear();
                                }
                            }
                            else if (engine.MethodExists(temporaryBuild.ToString()))
                            {
                                ParseString(temporaryBuild.ToString());

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
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("+");
                        }
                        else if (engine.IsStringVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(engine.GetVariableString(temporaryBuild.ToString()));
                            temporaryBuild.Clear();
                            contents.Add("+");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

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
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("-");
                        }
                        else if (engine.IsStringVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(engine.GetVariableString(temporaryBuild.ToString()));
                            temporaryBuild.Clear();
                            contents.Add("-");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

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
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("*");
                        }
                        else if (engine.IsStringVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(engine.GetVariableString(temporaryBuild.ToString()));
                            temporaryBuild.Clear();
                            contents.Add("*");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

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

            if (engine.VariableExists(temporaryBuild.ToString()))
            {
                if (engine.IsNumberVariable(temporaryBuild.ToString()))
                {
                    vars.Add(temporaryBuild.ToString());
                    contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
                    temporaryBuild.Clear();
                }
                else if (engine.IsStringVariable(temporaryBuild.ToString()))
                {
                    vars.Add(temporaryBuild.ToString());
                    contents.Add(engine.GetVariableString(temporaryBuild.ToString()));
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

            for (int i = 0; i < contents.Count; i++)
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

            if (engine.__Returning)
            {
                for (int i = 0; i < vars.Count; i++)
                    engine.DeleteVariable(vars[i]);

                engine.__Returning = false;
            }

            return (stackValue);
        }
        #endregion

        #region Stack
        double GetStack(string arg2)
        {
            System.Text.StringBuilder temporaryBuild = new();
            string tempArgTwo = arg2; 
            tempArgTwo = StringHelper.SubtractChars(tempArgTwo, "(");
            tempArgTwo = StringHelper.SubtractChars(tempArgTwo, ")");

            double stackValue = (double)0.0;

            System.Collections.Generic.List<string> contents = new();
            System.Collections.Generic.List<string> vars = new();

            for (int i = 0; i < tempArgTwo.Length; i++)
            {
                if (tempArgTwo[i] == ' ')
                {
                    if (temporaryBuild.Length != 0)
                    {
                        if (engine.VariableExists(temporaryBuild.ToString()))
                        {
                            if (engine.IsNumberVariable(temporaryBuild.ToString()))
                            {
                                vars.Add(temporaryBuild.ToString());
                                contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
                                temporaryBuild.Clear();
                            }
                        }
                        else if (engine.MethodExists(temporaryBuild.ToString()))
                        {
                            ParseString(temporaryBuild.ToString());

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
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("+");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

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
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("-");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

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
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                        {
                            contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("*");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

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
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("/");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

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
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("%");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

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
                    if (engine.VariableExists(temporaryBuild.ToString()))
                    {
                        if (engine.IsNumberVariable(temporaryBuild.ToString()))
                        {
                            vars.Add(temporaryBuild.ToString());
                            contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
                            temporaryBuild.Clear();
                            contents.Add("^");
                        }
                    }
                    else if (engine.MethodExists(temporaryBuild.ToString()))
                    {
                        ParseString(temporaryBuild.ToString());

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

            if (engine.VariableExists(temporaryBuild.ToString()))
            {
                if (engine.IsNumberVariable(temporaryBuild.ToString()))
                {
                    vars.Add(temporaryBuild.ToString());
                    contents.Add(StringHelper.DtoS(engine.GetVariableNumber(temporaryBuild.ToString())));
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

            for (int i = 0; i < contents.Count; i++)
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
                        stackValue = (stackValue % StringHelper.StoD(contents[i]));
                        moduloNext = false;
                    }
                    else if (powerNext)
                    {
                        stackValue = System.Math.Pow(stackValue, StringHelper.StoD(contents[i]));
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

            if (engine.__Returning)
            {
                for (int i = 0; i < vars.Count; i++)
                    engine.DeleteVariable(vars[i]);

                engine.__Returning = false;
            }

            return (stackValue);
        }
        #endregion
    }
}
