namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Engine;
    using MetaScriptLang.Engine.Memory;
    using MetaScriptLang.Helpers;
    using MetaScriptLang.IO;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        StateEngine engine = null;
        GarbageCollector gc = null;

        public Parser()
        {
            engine = new();
            gc = new(engine);
        }

        void whileLoop(Method m)
        {
            for (int i = 0; i < m.GetMethodSize(); i++)
            {
                if (m.GetLine(i) == "leave!")
                    __Breaking = true;
                else
                    ParseString(m.GetLine(i));
            }
        }

        void initializeVariable(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            string tmpObjName = StringHelper.BeforeDot(arg0), tmpVarName = StringHelper.AfterDot(arg0);
            bool tmpObjExists = engine.ObjectExists(tmpObjName);
            if (tmpObjExists || StringHelper.StringStartsWith(arg0, "@"))
            {
                if (tmpObjExists)
                {
                    if (engine.GetObjectVariableString(tmpObjName, tmpVarName) != __Null)
                    {
                        string tempObjectVariableName = ("@ " + tmpObjName + tmpVarName + "_string");
                        engine.CreateVariableString(tempObjectVariableName, engine.GetObjectVariableString(tmpObjName, tmpVarName));
                        ParseTwoSpaceString(tempObjectVariableName, arg1, arg2, "", command);
                        engine.SetVariableName(tempObjectVariableName, tmpVarName);
                        engine.DeleteObjectVariable(tmpObjName, tmpVarName);
                        engine.CreateObjectVariable(tmpObjName, engine.GetVariable(tmpVarName));
                        engine.DeleteVariable(tmpVarName);
                    }
                    else if (engine.GetObjectVariableNumber(tmpObjName, tmpVarName) != __NullNum)
                    {
                        string tempObjectVariableName = ("@____" + StringHelper.BeforeDot(arg0) + "___" + StringHelper.AfterDot(arg0) + "_number");
                        engine.CreateVariableNumber(tempObjectVariableName, engine.GetObjectVariableNumber(StringHelper.BeforeDot(arg0), StringHelper.AfterDot(arg0)));
                        ParseTwoSpaceString(tempObjectVariableName, arg1, arg2, tempObjectVariableName + " " + arg1 + " " + arg2, command);
                        engine.SetVariableName(tempObjectVariableName, StringHelper.AfterDot(arg0));
                        engine.DeleteObjectVariable(StringHelper.BeforeDot(arg0), StringHelper.AfterDot(arg0));
                        engine.CreateObjectVariable(StringHelper.BeforeDot(arg0), engine.GetVariable(StringHelper.AfterDot(arg0)));
                        engine.DeleteVariable(StringHelper.AfterDot(arg0));
                    }
                }
                else if (arg1 == "=")
                {
                    string before = StringHelper.BeforeDot(arg2), after = StringHelper.AfterDot(arg2);

                    if (StringHelper.ContainsBrackets(arg2) && (engine.VariableExists(StringHelper.BeforeBrackets(arg2)) || engine.ListExists(StringHelper.BeforeBrackets(arg2))))
                    {
                        string beforeBracket = StringHelper.BeforeBrackets(arg2), afterBracket = StringHelper.AfterBrackets(arg2);

                        afterBracket = StringHelper.SubtractString(afterBracket, "]");

                        if (engine.ListExists(beforeBracket))
                        {
                            if (engine.GetListSize(beforeBracket) >= StringHelper.StoI(afterBracket))
                            {
                                if (engine.GetListLine(beforeBracket, StringHelper.StoI(afterBracket)) == "#!=no_line")
                                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
                                else
                                {
                                    string listValue = engine.GetListLine(beforeBracket, StringHelper.StoI(afterBracket));

                                    if (StringHelper.IsNumeric(listValue))
                                    {
                                        if (engine.IsNumberVariable(arg0))
                                            engine.SetVariableNumber(arg0, StringHelper.StoD(listValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                    {
                                        if (engine.IsStringVariable(arg0))
                                            engine.SetVariableString(arg0, listValue);
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                }
                            }
                        }
                        else if (engine.IsStringVariable(beforeBracket))
                            setSubString(arg0, arg2, beforeBracket);
                        else
                            ErrorLogger.Error(ErrorLogger.LIST_UNDEFINED, beforeBracket, false);
                    }
                    else if (before.Length != 0 && after.Length != 0)
                    {
                        if (StringHelper.ContainsParameters(arg2))
                        {
                            if (StringHelper.BeforeParameters(arg2) == "random")
                            {
                                if (StringHelper.ContainsString(arg2, ".."))
                                {
                                    System.Collections.Generic.List<string> range = StringHelper.GetRange(arg2);
                                    string s0 = (range[0]), s2 = (range[1]);

                                    if (StringHelper.IsNumeric(s0) && StringHelper.IsNumeric(s2))
                                    {
                                        if (engine.IsNumberVariable(arg0))
                                        {
                                            double n0 = StringHelper.StoD(s0), n2 = StringHelper.StoD(s2);

                                            if (n0 < n2)
                                                engine.SetVariableNumber(arg0, (int)random(n0, n2));
                                            else if (n0 > n2)
                                                engine.SetVariableNumber(arg0, (int)random(n2, n0));
                                            else
                                                engine.SetVariableNumber(arg0, (int)random(n0, n2));
                                        }
                                        else if (engine.IsStringVariable(arg0))
                                        {
                                            double n0 = StringHelper.StoD(s0), n2 = StringHelper.StoD(s2);

                                            if (n0 < n2)
                                                engine.SetVariableString(arg0, StringHelper.ItoS((int)random(n0, n2)));
                                            else if (n0 > n2)
                                                engine.SetVariableString(arg0, StringHelper.ItoS((int)random(n2, n0)));
                                            else
                                                engine.SetVariableString(arg0, StringHelper.ItoS((int)random(n0, n2)));
                                        }
                                    }
                                    else if (StringHelper.IsAlphabetical(s0) && StringHelper.IsAlphabetical(s2))
                                    {
                                        if (engine.IsStringVariable(arg0))
                                        {
                                            if (StringHelper.GetCharAsInt32(s0[0]) < StringHelper.GetCharAsInt32(s2[0]))
                                                engine.SetVariableString(arg0, random(s0, s2));
                                            else if (StringHelper.GetCharAsInt32(s0[0]) > StringHelper.GetCharAsInt32(s2[0]))
                                                engine.SetVariableString(arg0, random(s2, s0));
                                            else
                                                engine.SetVariableString(arg0, random(s2, s0));
                                        }
                                        else
                                            ErrorLogger.Error(ErrorLogger.NULL_STRING, arg0, false);
                                    }
                                    else if (engine.VariableExists(s0) || engine.VariableExists(s2))
                                    {
                                        if (engine.VariableExists(s0))
                                        {
                                            if (engine.IsNumberVariable(s0))
                                                s0 = StringHelper.DtoS(engine.GetVariableNumber(s0));
                                            else if (engine.IsStringVariable(s0))
                                                s0 = engine.GetVariableString(s0);
                                        }

                                        if (engine.VariableExists(s2))
                                        {
                                            if (engine.IsNumberVariable(s2))
                                                s2 = StringHelper.DtoS(engine.GetVariableNumber(s2));
                                            else if (engine.IsStringVariable(s2))
                                                s2 = engine.GetVariableString(s2);
                                        }

                                        if (StringHelper.IsNumeric(s0) && StringHelper.IsNumeric(s2))
                                        {
                                            if (engine.IsNumberVariable(arg0))
                                            {
                                                double n0 = StringHelper.StoD(s0), n2 = StringHelper.StoD(s2);

                                                if (n0 < n2)
                                                    engine.SetVariableNumber(arg0, (int)random(n0, n2));
                                                else if (n0 > n2)
                                                    engine.SetVariableNumber(arg0, (int)random(n2, n0));
                                                else
                                                    engine.SetVariableNumber(arg0, (int)random(n0, n2));
                                            }
                                            else if (engine.IsStringVariable(arg0))
                                            {
                                                double n0 = StringHelper.StoD(s0), n2 = StringHelper.StoD(s2);

                                                if (n0 < n2)
                                                    engine.SetVariableString(arg0, StringHelper.ItoS((int)random(n0, n2)));
                                                else if (n0 > n2)
                                                    engine.SetVariableString(arg0, StringHelper.ItoS((int)random(n2, n0)));
                                                else
                                                    engine.SetVariableString(arg0, StringHelper.ItoS((int)random(n0, n2)));
                                            }
                                        }
                                        else if (StringHelper.IsAlphabetical(s0) && StringHelper.IsAlphabetical(s2))
                                        {
                                            if (engine.IsStringVariable(arg0))
                                            {
                                                if (StringHelper.GetCharAsInt32(s0[0]) < StringHelper.GetCharAsInt32(s2[0]))
                                                    engine.SetVariableString(arg0, random(s0, s2));
                                                else if (StringHelper.GetCharAsInt32(s0[0]) > StringHelper.GetCharAsInt32(s2[0]))
                                                    engine.SetVariableString(arg0, random(s2, s0));
                                                else
                                                    engine.SetVariableString(arg0, random(s2, s0));
                                            }
                                            else
                                                ErrorLogger.Error(ErrorLogger.NULL_STRING, arg0, false);
                                        }
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.INVALID_SEQ, s0 + "_" + s2, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.INVALID_SEQ_SEP, arg2, false);
                            }
                        }
                        else if (engine.ListExists(before) && after == "size")
                        {
                            if (engine.IsNumberVariable(arg0))
                                engine.SetVariableNumber(arg0, StringHelper.StoD(StringHelper.ItoS(engine.GetListSize(before))));
                            else if (engine.IsStringVariable(arg0))
                                engine.SetVariableString(arg0, StringHelper.ItoS(engine.GetListSize(before)));
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else if (before == "self")
                        {
                            if (engine.ObjectExists(__CurrentMethodObject))
                                ParseTwoSpaceString(arg0, arg1, (__CurrentMethodObject + "." + after), (arg0 + " " + arg1 + " " + (__CurrentMethodObject + "." + after)), command);
                            else
                                ParseTwoSpaceString(arg0, arg1, after, (arg0 + " " + arg1 + " " + after), command);
                        }
                        else if (engine.ObjectExists(before))
                        {
                            if (engine.ObjectVariableExists(before, after))
                            {
                                if (engine.GetObjectVariableString(before, after) != __Null)
                                    engine.SetVariableString(arg0, engine.GetObjectVariableString(before, after));
                                else if (engine.GetObjectVariableNumber(before, after) != __NullNum)
                                    engine.SetVariableNumber(arg0, engine.GetObjectVariableNumber(before, after));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else if (engine.ObjectMethodExists(before, after) && !StringHelper.ContainsParameters(after))
                            {
                                ParseString(arg2);

                                if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, __LastValue);
                                else if (engine.IsNumberVariable(arg0))
                                    engine.SetVariableNumber(arg0, StringHelper.StoD(__LastValue));
                            }
                            else if (StringHelper.ContainsParameters(after))
                            {
                                if (engine.ObjectMethodExists(before, StringHelper.BeforeParameters(after)))
                                {
                                    executeTemplate(engine.GetObjectMethod(before, StringHelper.BeforeParameters(after)), StringHelper.GetParameters(after));

                                    if (StringHelper.IsNumeric(__LastValue))
                                    {
                                        if (engine.IsStringVariable(arg0))
                                            engine.SetVariableString(arg0, __LastValue);
                                        else if (engine.IsNumberVariable(arg0))
                                            engine.SetVariableNumber(arg0, StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                    }
                                    else
                                    {
                                        if (engine.IsStringVariable(arg0))
                                            engine.SetVariableString(arg0, __LastValue);
                                        else if (engine.IsNumberVariable(arg0))
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                        else
                                            ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                    }
                                }
                                else
                                    sysExec(s, command);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, arg2, false);
                        }
                        else if (before == "env")
                        {
                            InternalGetEnv(arg0, after, 1);
                        }
                        else if (after == "to_int")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsStringVariable(before))
                                    engine.SetVariableNumber(arg0, engine.GetVariableString(before)[0]);
                                else if (engine.IsNumberVariable(before))
                                {
                                    engine.SetVariableNumber(arg0, engine.GetVariableNumber(before));
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (after == "to_double")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsStringVariable(before))
                                    engine.SetVariableNumber(arg0, engine.GetVariableString(before)[0]);
                                else if (engine.IsNumberVariable(before))
                                {
                                    engine.SetVariableNumber(arg0, engine.GetVariableNumber(before));
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (after == "to_string")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(before))
                                    engine.SetVariableString(arg0, StringHelper.DtoS(engine.GetVariableNumber(before)));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (after == "to_number")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsStringVariable(before))
                                    engine.SetVariableNumber(arg0, StringHelper.StoD(engine.GetVariableString(before)));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (before == "readline")
                        {
                            if (engine.VariableExists(after))
                            {
                                if (engine.IsStringVariable(after))
                                {
                                    ConsoleWrite(cleanString(engine.GetVariableString(after)));
                                    string line = ConsoleHelper.GetLine();

                                    if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(line))
                                            engine.SetVariableNumber(arg0, StringHelper.StoD(line));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (engine.IsStringVariable(arg0))
                                        engine.SetVariableString(arg0, line);
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                }
                                else
                                {
                                    string line = "";
                                    ConsoleHelper.Output = "readline: ";
                                    line = ConsoleHelper.GetLine();

                                    if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(line))
                                            engine.SetVariableNumber(arg0, StringHelper.StoD(line));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (engine.IsStringVariable(arg0))
                                        engine.SetVariableString(arg0, line);
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                }
                            }
                            else
                            {
                                string line = "";
                                ConsoleHelper.Output = cleanString(after);
                                line = ConsoleHelper.GetLine();

                                if (StringHelper.IsNumeric(line))
                                    engine.SetVariableNumber(arg0, StringHelper.StoD(line));
                                else
                                    engine.SetVariableString(arg0, line);
                            }
                        }
                        else if (before == "password")
                        {
                            if (engine.VariableExists(after))
                            {
                                if (engine.IsStringVariable(after))
                                {
                                    string line = "";
                                    line = getSilentOutput(engine.GetVariableString(after));

                                    if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(line))
                                            engine.SetVariableNumber(arg0, StringHelper.StoD(line));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (engine.IsStringVariable(arg0))
                                        engine.SetVariableString(arg0, line);
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);

                                    ConsoleHelper.Output = System.Environment.NewLine;
                                }
                                else
                                {
                                    string line = "";
                                    line = getSilentOutput("password: ");

                                    if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(line))
                                            engine.SetVariableNumber(arg0, StringHelper.StoD(line));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (engine.IsStringVariable(arg0))
                                        engine.SetVariableString(arg0, line);
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);

                                    ConsoleHelper.Output = System.Environment.NewLine;
                                }
                            }
                            else
                            {
                                string line = ("");
                                line = getSilentOutput(cleanString(after));

                                if (StringHelper.IsNumeric(line))
                                    engine.SetVariableNumber(arg0, StringHelper.StoD(line));
                                else
                                    engine.SetVariableString(arg0, line);

                                ConsoleHelper.Output = System.Environment.NewLine;
                            }
                        }
                        else if (after == "cos")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Cos(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Cos(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "acos")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Acos(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Acos(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "cosh")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Cosh(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Cosh(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "log")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Log(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Log(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "sqrt")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Sqrt(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Sqrt(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "abs")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Abs(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Abs(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "floor")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Floor(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Floor(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "ceil")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Ceiling(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Ceiling(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "exp")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Exp(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Exp(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "sin")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Sin(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Sin(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "sinh")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Sinh(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Sinh(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "asin")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Asin(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Asin(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "tan")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Tan(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Tan(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "tanh")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Tanh(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Tanh(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "atan")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableNumber(arg0, System.Math.Atan(engine.GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsNumberVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.DtoS(System.Math.Atan(engine.GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "to_lower")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsStringVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.ToLowercase(engine.GetVariableString(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "read")
                        {
                            if (engine.IsStringVariable(arg0))
                            {
                                if (engine.VariableExists(before))
                                {
                                    if (engine.IsStringVariable(before))
                                    {
                                        if (System.IO.File.Exists(engine.GetVariableString(before)))
                                        {
                                            string bigString = "";
                                            foreach (var line in System.IO.File.ReadAllLines(engine.GetVariableString(before)))
                                            {
                                                bigString += line + System.Environment.NewLine;
                                            }
                                            engine.SetVariableString(arg0, bigString);
                                        }
                                        else
                                            ErrorLogger.Error(ErrorLogger.READ_FAIL, engine.GetVariableString(before), false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_STRING, before, false);
                                }
                                else
                                {
                                    if (System.IO.File.Exists(before))
                                    {
                                        string bigString = ("");
                                        foreach (var line in System.IO.File.ReadAllLines(before))
                                        {
                                            bigString += (line + "\r\n");
                                        }
                                        engine.SetVariableString(arg0, bigString);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.READ_FAIL, before, false);
                                }
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.NULL_STRING, arg0, false);
                        }
                        else if (after == "to_upper")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsStringVariable(arg0))
                                {
                                    if (engine.IsStringVariable(before))
                                        engine.SetVariableString(arg0, StringHelper.ToUppercase(engine.GetVariableString(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "size")
                        {
                            if (engine.VariableExists(before))
                            {
                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (engine.IsStringVariable(before))
                                        engine.SetVariableNumber(arg0, engine.GetVariableString(before).Length);
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                            {
                                if (engine.IsNumberVariable(arg0))
                                    engine.SetVariableNumber(arg0, (double)before.Length);
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                        }
                        else if (after == "bytes")
                        {
                            if (engine.IsNumberVariable(arg0))
                            {
                                if (engine.VariableExists(before))
                                {
                                    if (engine.IsStringVariable(before))
                                    {
                                        if (System.IO.File.Exists(engine.GetVariableString(before)))
                                            engine.SetVariableNumber(arg0, getBytes(engine.GetVariableString(before)));
                                        else
                                            ErrorLogger.Error(ErrorLogger.READ_FAIL, engine.GetVariableString(before), false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                {
                                    if (System.IO.File.Exists(before))
                                        engine.SetVariableNumber(arg0, getBytes(before));
                                    else
                                        ErrorLogger.Error(ErrorLogger.READ_FAIL, before, false);
                                }
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                        }
                        else
                        {
                            if (engine.IsNumberVariable(arg0))
                            {
                                if (StringHelper.IsNumeric(arg2))
                                    engine.SetVariableNumber(arg0, StringHelper.StoD(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else if (engine.IsStringVariable(arg0))
                                engine.SetVariableString(arg0, arg2);
                            else if (engine.VariableWaiting(arg0))
                            {
                                if (StringHelper.IsNumeric(arg2))
                                    engine.SetVariableNumber(arg0, StringHelper.StoD(before + "." + after));
                                else
                                    engine.SetVariableString(arg0, arg2);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                        }
                    }
                    else
                    {
                        if (engine.VariableWaiting(arg0))
                        {
                            if (StringHelper.IsNumeric(arg2))
                                engine.SetVariableNumber(arg0, StringHelper.StoD(arg2));
                            else
                                engine.SetVariableString(arg0, arg2);
                        }
                        else if (arg2 == "null")
                        {
                            if (engine.IsStringVariable(arg0) || engine.IsNumberVariable(arg0))
                                engine.SetVariableNull(arg0);
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else if (engine.ConstantExists(arg2))
                        {
                            if (engine.IsStringVariable(arg0))
                            {
                                if (engine.IsNumberConstant(arg2))
                                    engine.SetVariableString(arg0, StringHelper.DtoS(engine.GetConstantNumber(arg2)));
                                else if (engine.IsStringConstant(arg2))
                                    engine.SetVariableString(arg0, engine.GetConstantString(arg2));
                            }
                            else if (engine.IsNumberVariable(arg0))
                            {
                                if (engine.IsNumberConstant(arg2))
                                    engine.SetVariableNumber(arg0, engine.GetConstantNumber(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else if (engine.MethodExists(arg2))
                        {
                            ParseString(arg2);

                            if (engine.IsStringVariable(arg0))
                                engine.SetVariableString(arg0, __LastValue);
                            else if (engine.IsNumberVariable(arg0))
                                engine.SetVariableNumber(arg0, StringHelper.StoD(__LastValue));
                        }
                        else if (engine.VariableExists(arg2))
                        {
                            if (engine.IsStringVariable(arg2))
                            {
                                if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, engine.GetVariableString(arg2));
                                else if (engine.IsNumberVariable(arg0))
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else if (engine.IsNumberVariable(arg2))
                            {
                                if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, StringHelper.DtoS(engine.GetVariableNumber(arg2)));
                                else if (engine.IsNumberVariable(arg0))
                                    engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else if (arg2 == "password" || arg2 == "readline")
                        {
                            if (arg2 == "password")
                            {
                                string passworder = ("");
                                passworder = getSilentOutput("");

                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (StringHelper.IsNumeric(passworder))
                                        engine.SetVariableNumber(arg0, StringHelper.StoD(passworder));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, passworder, false);
                                }
                                else if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, passworder);
                                else
                                    engine.SetVariableString(arg0, passworder);
                            }
                            else
                            {
                                string line = "";
                                ConsoleHelper.Output = "readline: ";
                                line = ConsoleHelper.GetLine();

                                if (StringHelper.IsNumeric(line))
                                    engine.CreateVariableNumber(arg0, StringHelper.StoD(line));
                                else
                                    engine.CreateVariableString(arg0, line);
                            }
                        }
                        else if (StringHelper.ContainsParameters(arg2))
                        {
                            if (engine.MethodExists(StringHelper.BeforeParameters(arg2)))
                            {
                                // execute the method
                                executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));
                                // set the variable = last value
                                if (engine.IsStringVariable(arg0))
                                {
                                    engine.SetVariableString(arg0, __LastValue);
                                }
                                else if (engine.IsNumberVariable(arg0))
                                {
                                    engine.SetVariableNumber(arg0, StringHelper.StoD(__LastValue));
                                }
                            }
                            else if (IsStringStack(arg2))
                            {
                                if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, GetStringStack(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else if (IsStackReady(arg2))
                            {
                                if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, StringHelper.DtoS(GetStack(arg2)));
                                else if (engine.IsNumberVariable(arg0))
                                    engine.SetVariableNumber(arg0, GetStack(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else
                        {
                            if (StringHelper.IsNumeric(arg2))
                            {
                                if (engine.IsNumberVariable(arg0))
                                    engine.SetVariableNumber(arg0, StringHelper.StoD(arg2));
                                else if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, arg2);
                            }
                            else
                            {
                                if (engine.IsNumberVariable(arg0))
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                else if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, cleanString(arg2));
                            }
                        }
                    }
                }
                else
                {
                    if (arg1 == "+=")
                    {
                        if (engine.VariableExists(arg2))
                        {
                            if (engine.IsStringVariable(arg0))
                            {
                                if (engine.IsStringVariable(arg2))
                                    engine.SetVariableString(arg0, engine.GetVariableString(arg0) + engine.GetVariableString(arg2));
                                else if (engine.IsNumberVariable(arg2))
                                    engine.SetVariableString(arg0, engine.GetVariableString(arg0) + StringHelper.DtoS(engine.GetVariableNumber(arg2)));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else if (engine.IsNumberVariable(arg0))
                            {
                                if (engine.IsStringVariable(arg2))
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                                else if (engine.IsNumberVariable(arg2))
                                    engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) + engine.GetVariableNumber(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else
                        {
                            if (StringHelper.ContainsParameters(arg2))
                            {
                                if (IsStringStack(arg2))
                                {
                                    if (engine.IsStringVariable(arg0))
                                        engine.SetVariableString(arg0, engine.GetVariableString(arg0) + GetStringStack(arg2));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else if (IsStackReady(arg2))
                                {
                                    if (engine.IsNumberVariable(arg0))
                                        engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) + GetStack(arg2));
                                }
                                else if (engine.MethodExists(StringHelper.BeforeParameters(arg2)))
                                {
                                    executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (engine.IsStringVariable(arg0))
                                        engine.SetVariableString(arg0, engine.GetVariableString(arg0) + __LastValue);
                                    else if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) + StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                }
                                else if (engine.ObjectExists(StringHelper.BeforeDot(arg2)))
                                {
                                    executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (engine.IsStringVariable(arg0))
                                        engine.SetVariableString(arg0, engine.GetVariableString(arg0) + __LastValue);
                                    else if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) + StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                }
                            }
                            else if (engine.MethodExists(arg2))
                            {
                                ParseString(arg2);

                                if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, engine.GetVariableString(arg0) + __LastValue);
                                else if (engine.IsNumberVariable(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) + StringHelper.StoD(__LastValue));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, engine.GetVariableString(arg0) + arg2);
                                else if (engine.IsNumberVariable(arg0))
                                    engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) + StringHelper.StoD(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                            {
                                if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, engine.GetVariableString(arg0) + cleanString(arg2));
                                else if (engine.IsNumberVariable(arg0))
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                    }
                    else if (arg1 == "-=")
                    {
                        if (engine.VariableExists(arg2))
                        {
                            if (engine.IsStringVariable(arg0))
                            {
                                if (engine.IsStringVariable(arg2))
                                {
                                    if (engine.GetVariableString(arg2).Length == 1)
                                        engine.SetVariableString(arg0, StringHelper.SubtractChars(engine.GetVariableString(arg0), engine.GetVariableString(arg2)));
                                    else
                                        engine.SetVariableString(arg0, StringHelper.SubtractString(engine.GetVariableString(arg0), engine.GetVariableString(arg2)));
                                }
                                else if (engine.IsNumberVariable(arg2))
                                    engine.SetVariableString(arg0, StringHelper.SubtractString(engine.GetVariableString(arg0), StringHelper.DtoS(engine.GetVariableNumber(arg2))));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else if (engine.IsNumberVariable(arg0))
                            {
                                if (engine.IsStringVariable(arg2))
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                                else if (engine.IsNumberVariable(arg2))
                                    engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) - engine.GetVariableNumber(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else
                        {
                            if (StringHelper.ContainsParameters(arg2))
                            {
                                if (IsStringStack(arg2))
                                {
                                    if (engine.IsStringVariable(arg0))
                                        engine.SetVariableString(arg0, StringHelper.SubtractString(engine.GetVariableString(arg0), GetStringStack(arg2)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else if (IsStackReady(arg2))
                                {
                                    if (engine.IsNumberVariable(arg0))
                                        engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) - GetStack(arg2));
                                }
                                else if (engine.MethodExists(StringHelper.BeforeParameters(arg2)))
                                {
                                    executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (engine.IsStringVariable(arg0))
                                        engine.SetVariableString(arg0, StringHelper.SubtractString(engine.GetVariableString(arg0), __LastValue));
                                    else if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) - StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                }
                                else if (engine.ObjectExists(StringHelper.BeforeDot(arg2)))
                                {
                                    executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (engine.IsStringVariable(arg0))
                                        engine.SetVariableString(arg0, StringHelper.SubtractString(engine.GetVariableString(arg0), __LastValue));
                                    else if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) - StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                }
                            }
                            else if (engine.MethodExists(arg2))
                            {
                                ParseString(arg2);

                                if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, StringHelper.SubtractString(engine.GetVariableString(arg0), __LastValue));
                                else if (engine.IsNumberVariable(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) - StringHelper.StoD(__LastValue));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (engine.IsStringVariable(arg0))
                                {
                                    if (arg2.Length == 1)
                                        engine.SetVariableString(arg0, StringHelper.SubtractChars(engine.GetVariableString(arg0), arg2));
                                    else
                                        engine.SetVariableString(arg0, StringHelper.SubtractString(engine.GetVariableString(arg0), arg2));
                                }
                                else if (engine.IsNumberVariable(arg0))
                                    engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) - StringHelper.StoD(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                            {
                                if (engine.IsStringVariable(arg0))
                                {
                                    if (arg2.Length == 1)
                                        engine.SetVariableString(arg0, StringHelper.SubtractChars(engine.GetVariableString(arg0), arg2));
                                    else
                                        engine.SetVariableString(arg0, StringHelper.SubtractString(engine.GetVariableString(arg0), cleanString(arg2)));
                                }
                                else if (engine.IsNumberVariable(arg0))
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                    }
                    else if (arg1 == "*=")
                    {
                        if (engine.VariableExists(arg2))
                        {
                            if (engine.IsNumberVariable(arg2))
                                engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) * engine.GetVariableNumber(arg2));
                            else if (engine.IsStringVariable(arg2))
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else
                        {
                            if (StringHelper.ContainsParameters(arg2))
                            {
                                if (IsStackReady(arg2))
                                {
                                    if (engine.IsNumberVariable(arg0))
                                        engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) * GetStack(arg2));
                                }
                                else if (engine.MethodExists(StringHelper.BeforeParameters(arg2)))
                                {
                                    executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) * StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                                else if (engine.ObjectExists(StringHelper.BeforeDot(arg2)))
                                {
                                    executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) * StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                            }
                            else if (engine.MethodExists(arg2))
                            {
                                ParseString(arg2);

                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) * StringHelper.StoD(__LastValue));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (engine.IsNumberVariable(arg0))
                                    engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) * StringHelper.StoD(arg2));
                            }
                            else
                                engine.SetVariableString(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "%=")
                    {
                        if (engine.VariableExists(arg2))
                        {
                            if (engine.IsNumberVariable(arg2))
                                engine.SetVariableNumber(arg0, (int)engine.GetVariableNumber(arg0) % (int)engine.GetVariableNumber(arg2));
                            else if (engine.IsStringVariable(arg2))
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else if (engine.MethodExists(arg2))
                        {
                            ParseString(arg2);

                            if (engine.IsNumberVariable(arg0))
                            {
                                if (StringHelper.IsNumeric(__LastValue))
                                    engine.SetVariableNumber(arg0, (int)engine.GetVariableNumber(arg0) % (int)StringHelper.StoD(__LastValue));
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                        }
                        else
                        {
                            if (StringHelper.IsNumeric(arg2))
                            {
                                if (engine.IsNumberVariable(arg0))
                                    engine.SetVariableNumber(arg0, (int)engine.GetVariableNumber(arg0) % (int)StringHelper.StoD(arg2));
                            }
                            else
                                engine.SetVariableString(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "**=")
                    {
                        if (engine.VariableExists(arg2))
                        {
                            if (engine.IsNumberVariable(arg2))
                                engine.SetVariableNumber(arg0, System.Math.Pow(engine.GetVariableNumber(arg0), engine.GetVariableNumber(arg2)));
                            else if (engine.IsStringVariable(arg2))
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else
                        {
                            if (StringHelper.ContainsParameters(arg2))
                            {
                                if (IsStackReady(arg2))
                                {
                                    if (engine.IsNumberVariable(arg0))
                                        engine.SetVariableNumber(arg0, System.Math.Pow(engine.GetVariableNumber(arg0), (int)GetStack(arg2)));
                                }
                                else if (engine.MethodExists(StringHelper.BeforeParameters(arg2)))
                                {
                                    executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            engine.SetVariableNumber(arg0, System.Math.Pow(engine.GetVariableNumber(arg0), (int)StringHelper.StoD(__LastValue)));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                                else if (engine.ObjectExists(StringHelper.BeforeDot(arg2)))
                                {
                                    executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            engine.SetVariableNumber(arg0, System.Math.Pow(engine.GetVariableNumber(arg0), (int)StringHelper.StoD(__LastValue)));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                            }
                            else if (engine.MethodExists(arg2))
                            {
                                ParseString(arg2);

                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        engine.SetVariableNumber(arg0, System.Math.Pow(engine.GetVariableNumber(arg0), (int)StringHelper.StoD(__LastValue)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (engine.IsNumberVariable(arg0))
                                    engine.SetVariableNumber(arg0, System.Math.Pow(engine.GetVariableNumber(arg0), StringHelper.StoD(arg2)));
                            }
                            else
                                engine.SetVariableString(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "/=")
                    {
                        if (engine.VariableExists(arg2))
                        {
                            if (engine.IsNumberVariable(arg2))
                                engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) / engine.GetVariableNumber(arg2));
                            else if (engine.IsStringVariable(arg2))
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else
                        {
                            if (StringHelper.ContainsParameters(arg2))
                            {
                                if (IsStackReady(arg2))
                                {
                                    if (engine.IsNumberVariable(arg0))
                                        engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) / GetStack(arg2));
                                }
                                else if (engine.MethodExists(StringHelper.BeforeParameters(arg2)))
                                {
                                    executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) / StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                                else if (engine.ObjectExists(StringHelper.BeforeDot(arg2)))
                                {
                                    executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (engine.IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) / StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                            }
                            else if (engine.MethodExists(arg2))
                            {
                                ParseString(arg2);

                                if (engine.IsNumberVariable(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) / StringHelper.StoD(__LastValue));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (engine.IsNumberVariable(arg0))
                                    engine.SetVariableNumber(arg0, engine.GetVariableNumber(arg0) / StringHelper.StoD(arg2));
                            }
                            else
                                engine.SetVariableString(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "++=")
                    {
                        if (engine.VariableExists(arg2))
                        {
                            if (engine.IsNumberVariable(arg2))
                            {
                                if (engine.IsStringVariable(arg0))
                                {
                                    int tempVarNumber = ((int)engine.GetVariableNumber(arg2));
                                    string tempVarString = (engine.GetVariableString(arg0));
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) + tempVarNumber));

                                    engine.SetVariableString(arg0, cleaned);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (StringHelper.IsNumeric(arg2))
                            {
                                int tempVarNumber = (StringHelper.StoI(arg2));
                                string tempVarString = (engine.GetVariableString(arg0));

                                if (tempVarString != __Null)
                                {
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) + tempVarNumber));

                                    engine.SetVariableString(arg0, cleaned);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, tempVarString, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                    }
                    else if (arg1 == "--=")
                    {
                        if (engine.VariableExists(arg2))
                        {
                            if (engine.IsNumberVariable(arg2))
                            {
                                if (engine.IsStringVariable(arg0))
                                {
                                    int tempVarNumber = ((int)engine.GetVariableNumber(arg2));
                                    string tempVarString = (engine.GetVariableString(arg0));
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) - tempVarNumber));

                                    engine.SetVariableString(arg0, cleaned);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (StringHelper.IsNumeric(arg2))
                            {
                                int tempVarNumber = (StringHelper.StoI(arg2));
                                string tempVarString = (engine.GetVariableString(arg0));

                                if (tempVarString != __Null)
                                {
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) - tempVarNumber));

                                    engine.SetVariableString(arg0, cleaned);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, tempVarString, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                    }
                    else if (arg1 == "?")
                    {
                        if (engine.VariableExists(arg2))
                        {
                            if (engine.IsStringVariable(arg2))
                            {
                                if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, getStdout(engine.GetVariableString(arg2)));
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (engine.IsStringVariable(arg0))
                                engine.SetVariableString(arg0, getStdout(cleanString(arg2)));
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                        }
                    }
                    else if (arg1 == "!")
                    {
                        if (engine.VariableExists(arg2))
                        {
                            if (engine.IsStringVariable(arg2))
                            {
                                if (engine.IsStringVariable(arg0))
                                    engine.SetVariableString(arg0, ParseStringAndCapture(engine.GetVariableString(arg2)));
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (engine.IsStringVariable(arg0))
                                engine.SetVariableString(arg0, ParseStringAndCapture(cleanString(arg2)));
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                        }
                    }
                    else
                    {
                        ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg1, false);
                    }
                }
            }
        }

        void initializeListValues(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            string _b = (StringHelper.BeforeDot(arg2)), _a = (StringHelper.AfterDot(arg2)), __b = (StringHelper.BeforeParameters(arg2));

            if (StringHelper.ContainsBrackets(arg0))
            {
                string after = (StringHelper.AfterBrackets(arg0)), before = (StringHelper.BeforeBrackets(arg0));
                after = StringHelper.SubtractString(after, "]");

                if (engine.GetListSize(before) >= StringHelper.StoI(after))
                {
                    if (StringHelper.StoI(after) == 0)
                    {
                        if (arg1 == "=")
                        {
                            if (engine.VariableExists(arg2))
                            {
                                if (engine.IsStringVariable(arg2))
                                    engine.ListReplace(before, after, engine.GetVariableString(arg2));
                                else if (engine.IsNumberVariable(arg2))
                                    engine.ListReplace(before, after, StringHelper.DtoS(engine.GetVariableNumber(arg2)));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else
                                engine.ListReplace(before, after, arg2);
                        }
                    }
                    else if (engine.GetListLine(before, StringHelper.StoI(after))  == "#!=no_line")
                        ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, arg0, false);
                    else
                    {
                        if (arg1 == "=")
                        {
                            if (engine.VariableExists(arg2))
                            {
                                if (engine.IsStringVariable(arg2))
                                    engine.ListReplace(before, after, engine.GetVariableString(arg2));
                                else if (engine.IsNumberVariable(arg2))
                                    engine.ListReplace(before, after, StringHelper.DtoS(engine.GetVariableNumber(arg2)));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else
                                engine.ListReplace(before, after, arg2);
                        }
                    }
                }
                else
                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
            }
            else if (StringHelper.ContainsBrackets(arg2)) // INITIALIZE LIST FROM RANGE
            {
                string listName = (StringHelper.BeforeBrackets(arg2));

                if (engine.ListExists(listName))
                {
                    System.Collections.Generic.List<string> listRange = StringHelper.GetBracketRange(arg2);

                    if (listRange.Count == 2)
                    {
                        string rangeBegin = (listRange[0]), rangeEnd = (listRange[1]);

                        if (rangeBegin.Length != 0 && rangeEnd.Length != 0)
                        {
                            if (StringHelper.IsNumeric(rangeBegin) && StringHelper.IsNumeric(rangeEnd))
                            {
                                if (StringHelper.StoI(rangeBegin) < StringHelper.StoI(rangeEnd))
                                {
                                    if (engine.GetListSize(listName) >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0)
                                    {
                                        if (StringHelper.StoI(rangeBegin) >= 0)
                                        {
                                            if (arg1 == "+=")
                                            {
                                                for (int i = StringHelper.StoI(rangeBegin); i <= StringHelper.StoI(rangeEnd); i++)
                                                    engine.AddToList(arg0, engine.GetListLine(listName, i));
                                            }
                                            else if (arg1 == "=")
                                            {
                                                engine.ListClear(arg0);

                                                for (int i = StringHelper.StoI(rangeBegin); i <= StringHelper.StoI(rangeEnd); i++)
                                                    engine.AddToList(arg0, engine.GetListLine(listName, i));
                                            }
                                            else
                                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg1, false);
                                        }
                                        else
                                            ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeEnd, false);
                                }
                                else if (StringHelper.StoI(rangeBegin) > StringHelper.StoI(rangeEnd))
                                {
                                    if (engine.GetListSize(listName) >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0)
                                    {
                                        if (StringHelper.StoI(rangeBegin) >= 0)
                                        {
                                            if (arg1 == "+=")
                                            {
                                                for (int i = StringHelper.StoI(rangeBegin); i >= StringHelper.StoI(rangeEnd); i--)
                                                    engine.AddToList(arg0, engine.GetListLine(listName, i));
                                            }
                                            else if (arg1 == "=")
                                            {
                                                engine.ListClear(arg0);

                                                for (int i = StringHelper.StoI(rangeBegin); i >= StringHelper.StoI(rangeEnd); i--)
                                                    engine.AddToList(arg0, engine.GetListLine(listName, i));
                                            }
                                            else
                                                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg1, false);
                                        }
                                        else
                                            ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeEnd, false);
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
                        ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
                }
                else
                    ErrorLogger.Error(ErrorLogger.LIST_UNDEFINED, listName, false);
            }
            else if (engine.VariableExists(_b) && StringHelper.ContainsString(_a, "split") && arg1 == "=")
            {
                if (engine.IsStringVariable(_b))
                {
                    System.Collections.Generic.List<string> parameters = StringHelper.GetParameters(_a);
                    System.Collections.Generic.List<string> elements = new();

                    if (parameters[0] == "")
                        elements = StringHelper.SplitString(engine.GetVariableString(_b), ' ');
                    else
                    {
                        if (parameters[0][0] == ';')
                            elements = StringHelper.SplitString(engine.GetVariableString(_b), ';');
                        else
                            elements = StringHelper.SplitString(engine.GetVariableString(_b), parameters[0][0]);
                    }

                    engine.ListClear(arg0);

                    for (int i = 0; i < elements.Count; i++)
                        engine.AddToList(arg0, elements[i]);
                }
                else
                    ErrorLogger.Error(ErrorLogger.NULL_STRING, _b, false);
            }
            else if (StringHelper.ContainsParameters(arg2)) // ADD/REMOVE ARRAY FROM LIST
            {
                System.Collections.Generic.List<string> parameters = StringHelper.GetParameters(arg2);

                if (arg1 == "=")
                {
                    engine.ListClear(arg0);

                    setList(arg0, arg2, parameters);
                }
                else if (arg1 == "+=")
                    setList(arg0, arg2, parameters);
                else if (arg1 == "-=")
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        if (engine.VariableExists(parameters[i]))
                        {
                            if (engine.IsStringVariable(parameters[i]))
                                engine.RemoveFromList(arg0, engine.GetVariableString(parameters[i]));
                            else if (engine.IsNumberVariable(parameters[i]))
                                engine.RemoveFromList(arg0, StringHelper.DtoS(engine.GetVariableNumber(parameters[i])));
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, parameters[i], false);
                        }
                        else
                            engine.RemoveFromList(arg0, parameters[i]);
                    }
                }
                else
                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else if (engine.VariableExists(arg2)) // ADD/REMOVE VARIABLE VALUE TO/FROM LIST
            {
                if (arg1 == "+=")
                {
                    if (engine.IsStringVariable(arg2))
                        engine.AddToList(arg0, engine.GetVariableString(arg2));
                    else if (engine.IsNumberVariable(arg2))
                        engine.AddToList(arg0, StringHelper.DtoS(engine.GetVariableNumber(arg2)));
                    else
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else if (arg1 == "-=")
                {
                    if (engine.IsStringVariable(arg2))
                        engine.RemoveFromList(arg0, engine.GetVariableString(arg2));
                    else if (engine.IsNumberVariable(arg2))
                        engine.RemoveFromList(arg0, StringHelper.DtoS(engine.GetVariableNumber(arg2)));
                    else
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else if (engine.MethodExists(arg2)) // INITIALIZE LIST FROM METHOD RETURN
            {
                ParseString(arg2);

                System.Collections.Generic.List<string> _p = StringHelper.GetParameters(__LastValue);

                if (arg1 == "=")
                {
                    engine.ListClear(arg0);

                    for (int i = 0; i < _p.Count; i++)
                        engine.AddToList(arg0, _p[i]);
                }
                else if (arg1 == "+=")
                {
                    for (int i = 0; i < _p.Count; i++)
                        engine.AddToList(arg0, _p[i]);
                }
                else
                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else // ADD/REMOVE STRING TO/FROM LIST
            {
                if (arg1 == "+=")
                {
                    if (arg2.Length != 0)
                        engine.AddToList(arg0, arg2);
                    else
                        ErrorLogger.Error(ErrorLogger.IS_EMPTY, arg2, false);
                }
                else if (arg1 == "-=")
                {
                    if (arg2.Length != 0)
                        engine.RemoveFromList(arg0, arg2);
                    else
                        ErrorLogger.Error(ErrorLogger.IS_EMPTY, arg2, false);
                }
            }
        }

        void createGlobalVariable(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            if (arg1 == "=")
            {
                string before = (StringHelper.BeforeDot(arg2)), after = (StringHelper.AfterDot(arg2));

                if (StringHelper.ContainsBrackets(arg2) && (engine.VariableExists(StringHelper.BeforeBrackets(arg2)) || engine.ListExists(StringHelper.BeforeBrackets(arg2))))
                {
                    string beforeBracket = (StringHelper.BeforeBrackets(arg2)), afterBracket = (StringHelper.AfterBrackets(arg2));

                    afterBracket = StringHelper.SubtractString(afterBracket, "]");

                    if (engine.ListExists(beforeBracket))
                    {
                        if (engine.GetListSize(beforeBracket) >= StringHelper.StoI(afterBracket))
                        {
                            if (engine.GetListLine(beforeBracket, StringHelper.StoI(afterBracket)) == "#!=no_line")
                                ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
                            else
                            {
                                string listValue = engine.GetListLine(beforeBracket, StringHelper.StoI(afterBracket));

                                if (StringHelper.IsNumeric(listValue))
                                    engine.CreateVariableNumber(arg0, StringHelper.StoD(listValue));
                                else
                                    engine.CreateVariableString(arg0, listValue);
                            }
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
                    }
                    else if (engine.VariableExists(beforeBracket))
                        setSubString(arg0, arg2, beforeBracket);
                    else
                        ErrorLogger.Error(ErrorLogger.LIST_UNDEFINED, beforeBracket, false);
                }
                else if (engine.ListExists(before) && after == "size")
                    engine.CreateVariableNumber(arg0, StringHelper.StoD(StringHelper.ItoS(engine.GetListSize(before))));
                else if (before == "self")
                {
                    if (engine.ObjectExists(__CurrentMethodObject))
                        ParseTwoSpaceString(arg0, arg1, (__CurrentMethodObject + "." + after), (arg0 + " " + arg1 + " " + (__CurrentMethodObject + "." + after)), command);
                    else
                        ParseTwoSpaceString(arg0, arg1, after, (arg0 + " " + arg1 + " " + after), command);
                }
                else if (after == "to_integer")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsStringVariable(before))
                            engine.CreateVariableNumber(arg0, engine.GetVariableString(before)[0]);
                        else if (engine.IsNumberVariable(before))
                        {
                            engine.CreateVariableNumber(arg0, engine.GetVariableNumber(before));
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_double")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsStringVariable(before))
                            engine.CreateVariableNumber(arg0, engine.GetVariableString(before)[0]);
                        else if (engine.IsNumberVariable(before))
                        {
                            engine.CreateVariableNumber(arg0, engine.GetVariableNumber(before));
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_string")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableString(arg0, StringHelper.DtoS(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_number")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsStringVariable(before))
                            engine.CreateVariableNumber(arg0, StringHelper.StoD(engine.GetVariableString(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (engine.ObjectExists(before))
                {
                    if (engine.ObjectMethodExists(before, after) && !StringHelper.ContainsParameters(after))
                    {
                        ParseString(arg2);

                        if (StringHelper.IsNumeric(__LastValue))
                            engine.CreateVariableNumber(arg0, StringHelper.StoD(__LastValue));
                        else
                            engine.CreateVariableString(arg0, __LastValue);
                    }
                    else if (StringHelper.ContainsParameters(after))
                    {
                        if (engine.ObjectMethodExists(before, StringHelper.BeforeParameters(after)))
                        {
                            executeTemplate(engine.GetObjectMethod(before, StringHelper.BeforeParameters(after)), StringHelper.GetParameters(after));

                            if (StringHelper.IsNumeric(__LastValue))
                                engine.CreateVariableNumber(arg0, StringHelper.StoD(__LastValue));
                            else
                                engine.CreateVariableString(arg0, __LastValue);
                        }
                        else
                            sysExec(s, command);
                    }
                    else if (engine.ObjectVariableExists(before, after))
                    {
                        if (engine.GetObjectVariableString(before, after) != __Null)
                            engine.CreateVariableString(arg0, engine.GetObjectVariableString(before, after));
                        else if (engine.GetObjectVariableNumber(before, after) != __NullNum)
                            engine.CreateVariableNumber(arg0, engine.GetObjectVariableNumber(before, after));
                        else
                            ErrorLogger.Error(ErrorLogger.IS_NULL, engine.GetObjectVariableName(before, after), false);
                    }
                }
                else if (engine.VariableExists(before) && after == "read")
                {
                    if (engine.IsStringVariable(before))
                    {
                        if (System.IO.File.Exists(engine.GetVariableString(before)))
                        {
                            string bigString = ("");
                            foreach (var line in System.IO.File.ReadAllLines(engine.GetVariableString(before)))
                            {
                                bigString += (line + "\r\n");
                            }
                            engine.CreateVariableString(arg0, bigString);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.READ_FAIL, engine.GetVariableString(before), false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.NULL_STRING, before, false);
                }
                else if (__DefiningObject)
                {
                    if (StringHelper.IsNumeric(arg2))
                    {
                        Variable newVariable = new(arg0, StringHelper.StoD(arg2));

                        if (__DefiningPrivateCode)
                            newVariable.MakePrivate();
                        else if (__DefiningPublicCode)
                            newVariable.MakePublic();

                        engine.CreateObjectVariable(__CurrentObject, newVariable);
                    }
                    else
                    {
                        Variable newVariable = new(arg0, arg2);

                        if (__DefiningPrivateCode)
                            newVariable.MakePrivate();
                        else if (__DefiningPublicCode)
                            newVariable.MakePublic();

                        engine.CreateObjectVariable(__CurrentObject, newVariable);
                    }
                }
                else if (arg2 == "null")
                    engine.CreateVariableString(arg0, arg2);
                else if (engine.MethodExists(arg2))
                {
                    ParseString(arg2);

                    if (StringHelper.IsNumeric(__LastValue))
                        engine.CreateVariableNumber(arg0, StringHelper.StoD(__LastValue));
                    else
                        engine.CreateVariableString(arg0, __LastValue);
                }
                else if (engine.ConstantExists(arg2))
                {
                    if (engine.IsNumberConstant(arg2))
                        engine.CreateVariableNumber(arg0, engine.GetConstantNumber(arg2));
                    else if (engine.IsStringConstant(arg2))
                        engine.CreateVariableString(arg0, engine.GetConstantString(arg2));
                    else
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else if (StringHelper.ContainsParameters(arg2))
                {
                    if (IsStringStack(arg2))
                        engine.CreateVariableString(arg0, GetStringStack(arg2));
                    else if (IsStackReady(arg2))
                        engine.CreateVariableNumber(arg0, GetStack(arg2));
                    else if (StringHelper.BeforeParameters(arg2) == "random")
                    {
                        if (StringHelper.ContainsString(arg2, ".."))
                        {
                            System.Collections.Generic.List<string> range = StringHelper.GetRange(arg2);
                            string s0 = (range[0]), s2 = (range[1]);

                            if (StringHelper.IsNumeric(s0) && StringHelper.IsNumeric(s2))
                            {
                                double n0 = StringHelper.StoD(s0), n2 = StringHelper.StoD(s2);

                                if (n0 < n2)
                                    engine.CreateVariableNumber(arg0, (int)random(n0, n2));
                                else if (n0 > n2)
                                    engine.CreateVariableNumber(arg0, (int)random(n2, n0));
                                else
                                    engine.CreateVariableNumber(arg0, (int)random(n0, n2));
                            }
                            else if (StringHelper.IsAlphabetical(s0) && StringHelper.IsAlphabetical(s2))
                            {
                                if (StringHelper.GetCharAsInt32(s0[0]) < StringHelper.GetCharAsInt32(s2[0]))
                                    engine.CreateVariableString(arg0, random(s0, s2));
                                else if (StringHelper.GetCharAsInt32(s0[0]) > StringHelper.GetCharAsInt32(s2[0]))
                                    engine.CreateVariableString(arg0, random(s2, s0));
                                else
                                    engine.CreateVariableString(arg0, random(s2, s0));
                            }
                            else if (engine.VariableExists(s0) || engine.VariableExists(s2))
                            {
                                if (engine.VariableExists(s0))
                                {
                                    if (engine.IsNumberVariable(s0))
                                        s0 = StringHelper.DtoS(engine.GetVariableNumber(s0));
                                    else if (engine.IsStringVariable(s0))
                                        s0 = engine.GetVariableString(s0);
                                }

                                if (engine.VariableExists(s2))
                                {
                                    if (engine.IsNumberVariable(s2))
                                        s2 = StringHelper.DtoS(engine.GetVariableNumber(s2));
                                    else if (engine.IsStringVariable(s2))
                                        s2 = engine.GetVariableString(s2);
                                }

                                if (StringHelper.IsNumeric(s0) && StringHelper.IsNumeric(s2))
                                {
                                    double n0 = StringHelper.StoD(s0), n2 = StringHelper.StoD(s2);

                                    if (n0 < n2)
                                        engine.CreateVariableNumber(arg0, (int)random(n0, n2));
                                    else if (n0 > n2)
                                        engine.CreateVariableNumber(arg0, (int)random(n2, n0));
                                    else
                                        engine.CreateVariableNumber(arg0, (int)random(n0, n2));
                                }
                                else if (StringHelper.IsAlphabetical(s0) && StringHelper.IsAlphabetical(s2))
                                {
                                    if (StringHelper.GetCharAsInt32(s0[0]) < StringHelper.GetCharAsInt32(s2[0]))
                                        engine.CreateVariableString(arg0, random(s0, s2));
                                    else if (StringHelper.GetCharAsInt32(s0[0]) > StringHelper.GetCharAsInt32(s2[0]))
                                        engine.CreateVariableString(arg0, random(s2, s0));
                                    else
                                        engine.CreateVariableString(arg0, random(s2, s0));
                                }
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, s0 + ".." + s2, false);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.INVALID_RANGE_SEP, arg2, false);
                    }
                    else
                    {
                        executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                        if (StringHelper.IsNumeric(__LastValue))
                            engine.CreateVariableNumber(arg0, StringHelper.StoD(__LastValue));
                        else
                            engine.CreateVariableString(arg0, __LastValue);
                    }
                }
                else if (engine.VariableExists(arg2))
                {
                    if (engine.IsNumberVariable(arg2))
                        engine.CreateVariableNumber(arg0, engine.GetVariableNumber(arg2));
                    else if (engine.IsStringVariable(arg2))
                        engine.CreateVariableString(arg0, engine.GetVariableString(arg2));
                    else
                        engine.CreateVariableString(arg0, __Null);
                }
                else if (arg2 == "password" || arg2 == "readline")
                {
                    string line = "";
                    if (arg2 == "password")
                    {
                        line = getSilentOutput("");

                        if (StringHelper.IsNumeric(line))
                            engine.CreateVariableNumber(arg0, StringHelper.StoD(line));
                        else
                            engine.CreateVariableString(arg0, line);
                    }
                    else
                    {
                        ConsoleHelper.Output = "readline: ";
                        line = ConsoleHelper.GetLine();

                        if (StringHelper.IsNumeric(line))
                            engine.CreateVariableNumber(arg0, StringHelper.StoD(line));
                        else
                            engine.CreateVariableString(arg0, line);
                    }
                }
                else if (arg2 == "args.size")
                    engine.CreateVariableNumber(arg0, (double)__ArgumentCount);
                else if (before == "readline")
                {
                    if (engine.VariableExists(after))
                    {
                        if (engine.IsStringVariable(after))
                        {
                            string line = "";
                            ConsoleHelper.Output = cleanString(engine.GetVariableString(after));
                            line = ConsoleHelper.GetLine();

                            if (StringHelper.IsNumeric(line))
                                engine.CreateVariableNumber(arg0, StringHelper.StoD(line));
                            else
                                engine.CreateVariableString(arg0, line);
                        }
                        else
                        {
                            string line = "";
                            ConsoleHelper.Output = "readline: ";
                            line = ConsoleHelper.GetLine();

                            if (StringHelper.IsNumeric(line))
                                engine.CreateVariableNumber(arg0, StringHelper.StoD(line));
                            else
                                engine.CreateVariableString(arg0, line);
                        }
                    }
                    else
                    {
                        string line = "";
                        ConsoleHelper.Output = cleanString(after);
                        line = ConsoleHelper.GetLine();

                        if (StringHelper.IsNumeric(line))
                            engine.CreateVariableNumber(arg0, StringHelper.StoD(line));
                        else
                            engine.CreateVariableString(arg0, line);
                    }
                }
                else if (before == "password")
                {
                    if (engine.VariableExists(after))
                    {
                        if (engine.IsStringVariable(after))
                        {
                            string line = "";
                            line = getSilentOutput(engine.GetVariableString(after));

                            if (StringHelper.IsNumeric(line))
                                engine.CreateVariableNumber(arg0, StringHelper.StoD(line));
                            else
                                engine.CreateVariableString(arg0, line);

                            ConsoleHelper.Output = System.Environment.NewLine;
                        }
                        else
                        {
                            string line = "";
                            line = getSilentOutput("password: ");

                            if (StringHelper.IsNumeric(line))
                                engine.CreateVariableNumber(arg0, StringHelper.StoD(line));
                            else
                                engine.CreateVariableString(arg0, line);

                            ConsoleHelper.Output = System.Environment.NewLine;
                        }
                    }
                    else
                    {
                        string line = "";
                        line = getSilentOutput(cleanString(after));

                        if (StringHelper.IsNumeric(line))
                            engine.CreateVariableNumber(arg0, StringHelper.StoD(line));
                        else
                            engine.CreateVariableString(arg0, line);

                        ConsoleHelper.Output = System.Environment.NewLine;
                    }
                }
                else if (after == "size")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsStringVariable(before))
                            engine.CreateVariableNumber(arg0, engine.GetVariableString(before).Length);
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        engine.CreateVariableNumber(arg0, (double)before.Length);
                }
                else if (after == "sin")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Sin(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "sinh")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Sinh(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "asin")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Asin(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "tan")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Tan(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "tanh")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Tanh(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "atan")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Atan(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "cos")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Cos(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "acos")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Acos(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "cosh")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Cosh(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "log")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Log(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "sqrt")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Sqrt(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "abs")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Abs(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "floor")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Floor(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "ceil")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Ceiling(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "exp")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsNumberVariable(before))
                            engine.CreateVariableNumber(arg0, System.Math.Exp(engine.GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_upper")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsStringVariable(before))
                            engine.CreateVariableString(arg0, StringHelper.ToUppercase(engine.GetVariableString(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_lower")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsStringVariable(before))
                            engine.CreateVariableString(arg0, StringHelper.ToLowercase(engine.GetVariableString(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "bytes")
                {
                    if (engine.VariableExists(before))
                    {
                        if (engine.IsStringVariable(before))
                        {
                            if (System.IO.File.Exists(engine.GetVariableString(before)))
                                engine.CreateVariableNumber(arg0, getBytes(engine.GetVariableString(before)));
                            else
                                ErrorLogger.Error(ErrorLogger.READ_FAIL, engine.GetVariableString(before), false);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                    {
                        if (System.IO.File.Exists(before))
                            engine.CreateVariableNumber(arg0, getBytes(before));
                        else
                            ErrorLogger.Error(ErrorLogger.READ_FAIL, before, false);
                    }
                }
                else if (before == "env")
                {
                    InternalGetEnv(arg0, after, 0);
                }
                else
                {
                    if (StringHelper.IsNumeric(arg2))
                        engine.CreateVariableNumber(arg0, StringHelper.StoD(arg2));
                    else
                        engine.CreateVariableString(arg0, cleanString(arg2));
                }
            }
            else if (arg1 == "+=")
            {
                if (engine.VariableExists(arg2))
                {
                    if (engine.IsStringVariable(arg2))
                        engine.CreateVariableString(arg0, engine.GetVariableString(arg2));
                    else if (engine.IsNumberVariable(arg2))
                        engine.CreateVariableNumber(arg0, engine.GetVariableNumber(arg2));
                    else
                        engine.CreateVariableString(arg0, __Null);
                }
                else
                {
                    if (StringHelper.IsNumeric(arg2))
                        engine.CreateVariableNumber(arg0, StringHelper.StoD(arg2));
                    else
                        engine.CreateVariableString(arg0, cleanString(arg2));
                }
            }
            else if (arg1 == "-=")
            {
                if (engine.VariableExists(arg2))
                {
                    if (engine.IsNumberVariable(arg2))
                        engine.CreateVariableNumber(arg0, 0 - engine.GetVariableNumber(arg2));
                    else
                        engine.CreateVariableString(arg0, __Null);
                }
                else
                {
                    if (StringHelper.IsNumeric(arg2))
                        engine.CreateVariableNumber(arg0, StringHelper.StoD(arg2));
                    else
                        engine.CreateVariableString(arg0, cleanString(arg2));
                }
            }
            else if (arg1 == "?")
            {
                if (engine.VariableExists(arg2))
                {
                    if (engine.IsStringVariable(arg2))
                        engine.CreateVariableString(arg0, getStdout(engine.GetVariableString(arg2)));
                    else
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                    engine.CreateVariableString(arg0, getStdout(cleanString(arg2)));
            }
            else if (arg1 == "!")
            {
                if (engine.VariableExists(arg2))
                {
                    if (engine.IsStringVariable(arg2))
                        engine.CreateVariableString(arg0, ParseStringAndCapture(engine.GetVariableString(arg2)));
                    else
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                    engine.CreateVariableString(arg0, ParseStringAndCapture(cleanString(arg2)));
            }
            else
                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg2, false);
        }

        void createObjectVariable(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            string before = StringHelper.BeforeDot(arg2),
                   after = StringHelper.AfterDot(arg2);

            if (engine.ObjectExists(before))
            {
                if (arg1 == "=")
                {
                    if (engine.GetObjectVariableString(before, after) != __Null)
                        engine.CreateVariableString(arg0, engine.GetObjectVariableString(before, after));
                    else if (engine.GetObjectVariableNumber(before, after) != __NullNum)
                        engine.CreateVariableNumber(arg0, engine.GetObjectVariableNumber(before, after));
                }
            }
        }

        void createConstant(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            if (!engine.ConstantExists(arg0))
            {
                if (arg1 == "=")
                {
                    if (StringHelper.IsNumeric(arg2))
                    {
                        Constant newConstant = new(arg0, StringHelper.StoD(arg2));
                        constants.Add(arg0, newConstant);
                    }
                    else
                    {
                        Constant newConstant = new(arg0, arg2);
                        constants.Add(arg0, newConstant);
                    }
                }
                else
                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else
                ErrorLogger.Error(ErrorLogger.CONST_UNDEFINED, arg0, false);
        }

        void executeSimpleStatement(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            if (StringHelper.IsNumeric(arg0) && StringHelper.IsNumeric(arg2))
            {
                if (arg1 == "+")
                    ConsoleWriteLine(StringHelper.DtoS(StringHelper.StoD(arg0) + StringHelper.StoD(arg2)));
                else if (arg1 == "-")
                    ConsoleWriteLine(StringHelper.DtoS(StringHelper.StoD(arg0) - StringHelper.StoD(arg2)));
                else if (arg1 == "*")
                    ConsoleWriteLine(StringHelper.DtoS(StringHelper.StoD(arg0) * StringHelper.StoD(arg2)));
                else if (arg1 == "/")
                    ConsoleWriteLine(StringHelper.DtoS(StringHelper.StoD(arg0) / StringHelper.StoD(arg2)));
                else if (arg1 == "**")
                    ConsoleWriteLine(StringHelper.DtoS(System.Math.Pow(StringHelper.StoD(arg0), StringHelper.StoD(arg2))));
                else if (arg1 == "%")
                {
                    if ((int)StringHelper.StoD(arg2) == 0)
                        ErrorLogger.Error(ErrorLogger.DIVIDED_BY_ZERO, s, false);
                    else
                        ConsoleWriteLine(StringHelper.DtoS((int)StringHelper.StoD(arg0) % (int)StringHelper.StoD(arg2)));
                }
                else
                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else
            {
                if (arg1 == "+")
                    ConsoleWriteLine(arg0 + arg2);
                else if (arg1 == "-")
                    ConsoleWriteLine(StringHelper.SubtractString(arg0, arg2));
                else if (arg1 == "*")
                {
                    if (!StringHelper.ZeroNumbers(arg2))
                    {
                        string bigstr = string.Empty;
                        for (int i = 0; i < StringHelper.StoI(arg2); i++)
                        {
                            bigstr += (arg0);
                            ConsoleWrite(arg0);
                        }

                        SetLastValue(bigstr);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.INVALID_OP, s, false);
                }
                else if (arg1 == "/")
                    ConsoleWriteLine(StringHelper.SubtractString(arg0, arg2));
                else
                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
        }

        void InternalEncryptDecrypt(string arg0, string arg1)
        {
            Crypt c = new();
            string text = engine.VariableExists(arg1) ? (engine.IsStringVariable(arg1) ? engine.GetVariableString(arg1) : StringHelper.DtoS(engine.GetVariableNumber(arg1))) : arg1;
            ConsoleWrite(arg0 == "encrypt" ? c.e(text) : c.d(text));
        }

        void delay(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        double random(double min, double max)
        {
            double r = new System.Random().NextDouble() / Double.MaxValue;
            return (min + (r * (max - min)));
        }

        string random(string start, string sc)
        {
            string s = string.Empty;
            char c = start[0] == sc[0] ? sc[0] : (char)((new System.Random().Next() % StringHelper.GetCharAsInt32(sc[0])) + start[0]);
            s += (c);

            return (s);
        }

        void uninstall()
        {
            if (System.IO.Directory.Exists(__SavedVarsPath))
            {
                if (System.IO.File.Exists(__SavedVars))
                    System.IO.File.Delete(__SavedVars);
                else
                    ConsoleHelper.Error = "...no remembered variables" + System.Environment.NewLine;

                System.IO.Directory.CreateDirectory(__SavedVarsPath);

                if (!System.IO.Directory.Exists(__SavedVarsPath) && !System.IO.File.Exists(__SavedVars))
                    ConsoleHelper.Output = "...removed successfully" + System.Environment.NewLine;
                else
                    ConsoleHelper.Error = "...failed to remove" + System.Environment.NewLine;
            }
            else
                ConsoleHelper.Error = "...found nothing to remove" + System.Environment.NewLine;
        }

        double getBytes(string path)
        {
            return System.IO.File.ReadAllBytes(path).LongLength;
        }


        string getSilentOutput(string text)
        {
            // todo read in as password/without printing
            return string.Empty;
        }

        void setup()
        {
            __BadMethodCount = 0;
            __BadObjectCount = 0;
            __BadVarCount = 0;
            __CurrentLineNumber = 0;
            __IfStatementCount = 0;
            __ForLoopCount = 0;
            __WhileLoopCount = 0;
            __ParamVarCount = 0;
            __CaptureParse = false;
            __IsCommented = false;
            __UseCustomPrompt = false;
            engine.__DontCollectMethodVars = false;
            engine.__FailedIfStatement = false;
            engine.__GoToLabel = false;
            engine.__ExecutedIfStatement = false;
            engine.__InDefaultCase = false;
            engine.__ExecutedMethod = false;
            engine.__DefiningSwitchBlock = false;
            __DefiningIfStatement = false;
            __DefiningForLoop = false;
            engine.__DefiningWhileLoop = false;
            __DefiningModule = false;
            __DefiningPrivateCode = false;
            __DefiningPublicCode = false;
            __DefiningScript = false;
            engine.__ExecutedTemplate = false; // remove
            __ExecutedTryBlock = false;
            __Breaking = false;
            __DefiningMethod = false;
            __MultilineComment = false;
            __Negligence = false;
            engine.__FailedNest = false;
            __DefiningNest = false;
            __DefiningObject = false;
            __DefiningObjectMethod = false;
            __DefiningParameterizedMethod = false;
            engine.__Returning = false;
            __SkipCatchBlock = false;
            __RaiseCatchBlock = false;
            __DefiningLocalSwitchBlock = false;
            __DefiningLocalWhileLoop = false;
            __DefiningLocalForLoop = false;

            __CurrentObject = "";
            __CurrentMethodObject = "";
            __CurrentModule = "";
            __CurrentScript = "";
            __ErrorVarName = "";
            __GoTo = "";
            __LastError = "";
            __LastValue = "";
            __ParsedOutput = "";
            __PreviousScript = "";
            __CurrentScriptName = "";
            __SwitchVarName = "";
            __CurrentLine = "";
            __DefaultLoopSymbol = "$";

            __Null = "[null]";

            __ArgumentCount = 0;
            __NullNum = -Double.MaxValue;

            if (StringHelper.ContainsString(System.Environment.GetEnvironmentVariable("HOMEPATH"), "Users"))
            {
                __GuessedOS = "win64";
                __SavedVarsPath = (System.Environment.GetEnvironmentVariable("HOMEPATH") + "\\AppData") + "\\.__SavedVarsPath";
                __SavedVars = __SavedVarsPath + "\\.__SavedVars";
            }
            else if (StringHelper.ContainsString(System.Environment.GetEnvironmentVariable("HOMEPATH"), "Documents"))
            {
                __GuessedOS = "win32";
                __SavedVarsPath = System.Environment.GetEnvironmentVariable("HOMEPATH") + "\\Application Data\\.__SavedVarsPath";
                __SavedVars = __SavedVarsPath + "\\.__SavedVars";
            }
            else if (StringHelper.StringStartsWith(System.Environment.GetEnvironmentVariable("HOME"), "/"))
            {
                __GuessedOS = "linux";
                __SavedVarsPath = System.Environment.GetEnvironmentVariable("HOME") + "/.__SavedVarsPath";
                __SavedVars = __SavedVarsPath + "/.__SavedVars";
            }
            else
            {
                __GuessedOS = "unknown";
                __SavedVarsPath = "\\.__SavedVarsPath"; 
                __SavedVars = __SavedVarsPath + "\\.__SavedVars";
            }
        }
    }
}
