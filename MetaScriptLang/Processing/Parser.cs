namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Engine;
    using MetaScriptLang.Engine.Memory;
    using MetaScriptLang.Helpers;
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
                    parse(m.GetLine(i));
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
                    if (GetObjectVariableString(tmpObjName, tmpVarName) != __Null)
                    {
                        string tempObjectVariableName = ("@ " + tmpObjName + tmpVarName + "_string");
                        CreateVariableString(tempObjectVariableName, GetObjectVariableString(tmpObjName, tmpVarName));
                        twoSpace(tempObjectVariableName, arg1, arg2, "", command);
                        SetVariableName(tempObjectVariableName, tmpVarName);
                        engine.DeleteObjectVariable(tmpObjName, tmpVarName);
                        CreateObjectVariable(tmpObjName, GetVariable(tmpVarName));
                        DeleteVariable(tmpVarName);
                    }
                    else if (GetObjectVariableNumber(tmpObjName, tmpVarName) != __NullNum)
                    {
                        string tempObjectVariableName = ("@____" + StringHelper.BeforeDot(arg0) + "___" + StringHelper.AfterDot(arg0) + "_number");
                        CreateVariableNumber(tempObjectVariableName, GetObjectVariableNumber(StringHelper.BeforeDot(arg0), StringHelper.AfterDot(arg0)));
                        twoSpace(tempObjectVariableName, arg1, arg2, tempObjectVariableName + " " + arg1 + " " + arg2, command);
                        SetVariableName(tempObjectVariableName, StringHelper.AfterDot(arg0));
                        engine.DeleteObjectVariable(StringHelper.BeforeDot(arg0), StringHelper.AfterDot(arg0));
                        CreateObjectVariable(StringHelper.BeforeDot(arg0), GetVariable(StringHelper.AfterDot(arg0)));
                        DeleteVariable(StringHelper.AfterDot(arg0));
                    }
                }
                else if (arg1 == "=")
                {
                    string before = (StringHelper.BeforeDot(arg2)), after = (StringHelper.AfterDot(arg2));

                    if (StringHelper.ContainsBrackets(arg2) && (VariableExists(StringHelper.BeforeBrackets(arg2)) || engine.ListExists(StringHelper.BeforeBrackets(arg2))))
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
                        else if (IsStringVariable(beforeBracket))
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
                                        if (IsNumberVariable(arg0))
                                        {
                                            double n0 = StringHelper.StoD(s0), n2 = StringHelper.StoD(s2);

                                            if (n0 < n2)
                                                SetVariableNumber(arg0, (int)random(n0, n2));
                                            else if (n0 > n2)
                                                SetVariableNumber(arg0, (int)random(n2, n0));
                                            else
                                                SetVariableNumber(arg0, (int)random(n0, n2));
                                        }
                                        else if (IsStringVariable(arg0))
                                        {
                                            double n0 = StringHelper.StoD(s0), n2 = StringHelper.StoD(s2);

                                            if (n0 < n2)
                                                SetVariableString(arg0, StringHelper.ItoS((int)random(n0, n2)));
                                            else if (n0 > n2)
                                                SetVariableString(arg0, StringHelper.ItoS((int)random(n2, n0)));
                                            else
                                                SetVariableString(arg0, StringHelper.ItoS((int)random(n0, n2)));
                                        }
                                    }
                                    else if (StringHelper.IsAlphabetical(s0) && StringHelper.IsAlphabetical(s2))
                                    {
                                        if (IsStringVariable(arg0))
                                        {
                                            if (StringHelper.GetCharAsInt32(s0[0]) < StringHelper.GetCharAsInt32(s2[0]))
                                                SetVariableString(arg0, random(s0, s2));
                                            else if (StringHelper.GetCharAsInt32(s0[0]) > StringHelper.GetCharAsInt32(s2[0]))
                                                SetVariableString(arg0, random(s2, s0));
                                            else
                                                SetVariableString(arg0, random(s2, s0));
                                        }
                                        else
                                            ErrorLogger.Error(ErrorLogger.NULL_STRING, arg0, false);
                                    }
                                    else if (VariableExists(s0) || VariableExists(s2))
                                    {
                                        if (VariableExists(s0))
                                        {
                                            if (IsNumberVariable(s0))
                                                s0 = StringHelper.DtoS(GetVariableNumber(s0));
                                            else if (IsStringVariable(s0))
                                                s0 = GetVariableString(s0);
                                        }

                                        if (VariableExists(s2))
                                        {
                                            if (IsNumberVariable(s2))
                                                s2 = StringHelper.DtoS(GetVariableNumber(s2));
                                            else if (IsStringVariable(s2))
                                                s2 = GetVariableString(s2);
                                        }

                                        if (StringHelper.IsNumeric(s0) && StringHelper.IsNumeric(s2))
                                        {
                                            if (IsNumberVariable(arg0))
                                            {
                                                double n0 = StringHelper.StoD(s0), n2 = StringHelper.StoD(s2);

                                                if (n0 < n2)
                                                    SetVariableNumber(arg0, (int)random(n0, n2));
                                                else if (n0 > n2)
                                                    SetVariableNumber(arg0, (int)random(n2, n0));
                                                else
                                                    SetVariableNumber(arg0, (int)random(n0, n2));
                                            }
                                            else if (IsStringVariable(arg0))
                                            {
                                                double n0 = StringHelper.StoD(s0), n2 = StringHelper.StoD(s2);

                                                if (n0 < n2)
                                                    SetVariableString(arg0, StringHelper.ItoS((int)random(n0, n2)));
                                                else if (n0 > n2)
                                                    SetVariableString(arg0, StringHelper.ItoS((int)random(n2, n0)));
                                                else
                                                    SetVariableString(arg0, StringHelper.ItoS((int)random(n0, n2)));
                                            }
                                        }
                                        else if (StringHelper.IsAlphabetical(s0) && StringHelper.IsAlphabetical(s2))
                                        {
                                            if (IsStringVariable(arg0))
                                            {
                                                if (StringHelper.GetCharAsInt32(s0[0]) < StringHelper.GetCharAsInt32(s2[0]))
                                                    SetVariableString(arg0, random(s0, s2));
                                                else if (StringHelper.GetCharAsInt32(s0[0]) > StringHelper.GetCharAsInt32(s2[0]))
                                                    SetVariableString(arg0, random(s2, s0));
                                                else
                                                    SetVariableString(arg0, random(s2, s0));
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
                            if (IsNumberVariable(arg0))
                                SetVariableNumber(arg0, StringHelper.StoD(StringHelper.ItoS(engine.GetListSize(before))));
                            else if (IsStringVariable(arg0))
                                SetVariableString(arg0, StringHelper.ItoS(engine.GetListSize(before)));
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else if (before == "self")
                        {
                            if (engine.ObjectExists(__CurrentMethodObject))
                                twoSpace(arg0, arg1, (__CurrentMethodObject + "." + after), (arg0 + " " + arg1 + " " + (__CurrentMethodObject + "." + after)), command);
                            else
                                twoSpace(arg0, arg1, after, (arg0 + " " + arg1 + " " + after), command);
                        }
                        else if (engine.ObjectExists(before))
                        {
                            if (ObjectVariableExists(before, after))
                            {
                                if (GetObjectVariableString(before, after) != __Null)
                                    SetVariableString(arg0, GetObjectVariableString(before, after));
                                else if (GetObjectVariableNumber(before, after) != __NullNum)
                                    SetVariableNumber(arg0, GetObjectVariableNumber(before, after));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else if (ObjectMethodExists(before, after) && !StringHelper.ContainsParameters(after))
                            {
                                parse(arg2);

                                if (IsStringVariable(arg0))
                                    SetVariableString(arg0, __LastValue);
                                else if (IsNumberVariable(arg0))
                                    SetVariableNumber(arg0, StringHelper.StoD(__LastValue));
                            }
                            else if (StringHelper.ContainsParameters(after))
                            {
                                if (ObjectMethodExists(before, StringHelper.BeforeParameters(after)))
                                {
                                    executeTemplate(GetObjectMethod(before, StringHelper.BeforeParameters(after)), StringHelper.GetParameters(after));

                                    if (StringHelper.IsNumeric(__LastValue))
                                    {
                                        if (IsStringVariable(arg0))
                                            SetVariableString(arg0, __LastValue);
                                        else if (IsNumberVariable(arg0))
                                            SetVariableNumber(arg0, StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                    }
                                    else
                                    {
                                        if (IsStringVariable(arg0))
                                            SetVariableString(arg0, __LastValue);
                                        else if (IsNumberVariable(arg0))
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
                            if (VariableExists(before))
                            {
                                if (IsStringVariable(before))
                                    SetVariableNumber(arg0, (int)GetVariableString(before)[0]);
                                else if (IsNumberVariable(before))
                                {
                                    int i = (int)GetVariableNumber(before);
                                    SetVariableNumber(arg0, (double)i);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (after == "to_double")
                        {
                            if (VariableExists(before))
                            {
                                if (IsStringVariable(before))
                                    SetVariableNumber(arg0, (double)GetVariableString(before)[0]);
                                else if (IsNumberVariable(before))
                                {
                                    double i = GetVariableNumber(before);
                                    SetVariableNumber(arg0, (double)i);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (after == "to_string")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(before))
                                    SetVariableString(arg0, StringHelper.DtoS(GetVariableNumber(before)));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (after == "to_number")
                        {
                            if (VariableExists(before))
                            {
                                if (IsStringVariable(before))
                                    SetVariableNumber(arg0, StringHelper.StoD(GetVariableString(before)));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (before == "readline")
                        {
                            if (VariableExists(after))
                            {
                                if (IsStringVariable(after))
                                {
                                    string line = "";
                                    write(cleanString(GetVariableString(after)));
                                    line = Console.ReadLine();

                                    if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(line))
                                            SetVariableNumber(arg0, StringHelper.StoD(line));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (IsStringVariable(arg0))
                                        SetVariableString(arg0, line);
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                }
                                else
                                {
                                    string line = "";
                                    cout = "readline: ";
                                    line = Console.ReadLine();

                                    if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(line))
                                            SetVariableNumber(arg0, StringHelper.StoD(line));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (IsStringVariable(arg0))
                                        SetVariableString(arg0, line);
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                }
                            }
                            else
                            {
                                string line = "";
                                cout = cleanString(after);
                                line = Console.ReadLine();

                                if (StringHelper.IsNumeric(line))
                                    SetVariableNumber(arg0, StringHelper.StoD(line));
                                else
                                    SetVariableString(arg0, line);
                            }
                        }
                        else if (before == "password")
                        {
                            if (VariableExists(after))
                            {
                                if (IsStringVariable(after))
                                {
                                    string line = "";
                                    line = getSilentOutput(GetVariableString(after));

                                    if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(line))
                                            SetVariableNumber(arg0, StringHelper.StoD(line));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (IsStringVariable(arg0))
                                        SetVariableString(arg0, line);
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);

                                    cout = System.Environment.NewLine;
                                }
                                else
                                {
                                    string line = "";
                                    line = getSilentOutput("password: ");

                                    if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(line))
                                            SetVariableNumber(arg0, StringHelper.StoD(line));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (IsStringVariable(arg0))
                                        SetVariableString(arg0, line);
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);

                                    cout = System.Environment.NewLine;
                                }
                            }
                            else
                            {
                                string line = ("");
                                line = getSilentOutput(cleanString(after));

                                if (StringHelper.IsNumeric(line))
                                    SetVariableNumber(arg0, StringHelper.StoD(line));
                                else
                                    SetVariableString(arg0, line);

                                cout = System.Environment.NewLine;
                            }
                        }
                        else if (after == "cos")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Cos(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Cos(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "acos")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Acos(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Acos(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "cosh")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Cosh(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Cosh(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "log")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Log(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Log(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "sqrt")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Sqrt(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Sqrt(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "abs")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Abs(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Abs(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "floor")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Floor(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Floor(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "ceil")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Ceiling(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Ceiling(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "exp")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Exp(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Exp(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "sin")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Sin(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Sin(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "sinh")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Sinh(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Sinh(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "asin")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Asin(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Asin(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "tan")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Tan(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Tan(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "tanh")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Tanh(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Tanh(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "atan")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableNumber(arg0, System.Math.Atan(GetVariableNumber(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (IsStringVariable(arg0))
                                {
                                    if (IsNumberVariable(before))
                                        SetVariableString(arg0, StringHelper.DtoS(System.Math.Atan(GetVariableNumber(before))));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "to_lower")
                        {
                            if (VariableExists(before))
                            {
                                if (IsStringVariable(arg0))
                                {
                                    if (IsStringVariable(before))
                                        SetVariableString(arg0, StringHelper.ToLowercase(GetVariableString(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "read")
                        {
                            if (IsStringVariable(arg0))
                            {
                                if (VariableExists(before))
                                {
                                    if (IsStringVariable(before))
                                    {
                                        if (System.IO.File.Exists(GetVariableString(before)))
                                        {
                                            string bigString = "";
                                            foreach (var line in System.IO.File.ReadAllLines(GetVariableString(before)))
                                            {
                                                bigString += line + System.Environment.NewLine;
                                            }
                                            SetVariableString(arg0, bigString);
                                        }
                                        else
                                            ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(before), false);
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
                                        SetVariableString(arg0, bigString);
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
                            if (VariableExists(before))
                            {
                                if (IsStringVariable(arg0))
                                {
                                    if (IsStringVariable(before))
                                        SetVariableString(arg0, StringHelper.ToUppercase(GetVariableString(before)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "size")
                        {
                            if (VariableExists(before))
                            {
                                if (IsNumberVariable(arg0))
                                {
                                    if (IsStringVariable(before))
                                        SetVariableNumber(arg0, (double)GetVariableString(before).Length);
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                            {
                                if (IsNumberVariable(arg0))
                                    SetVariableNumber(arg0, (double)before.Length);
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                        }
                        else if (after == "bytes")
                        {
                            if (IsNumberVariable(arg0))
                            {
                                if (VariableExists(before))
                                {
                                    if (IsStringVariable(before))
                                    {
                                        if (System.IO.File.Exists(GetVariableString(before)))
                                            SetVariableNumber(arg0, getBytes(GetVariableString(before)));
                                        else
                                            ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(before), false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                {
                                    if (System.IO.File.Exists(before))
                                        SetVariableNumber(arg0, getBytes(before));
                                    else
                                        ErrorLogger.Error(ErrorLogger.READ_FAIL, before, false);
                                }
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                        }
                        else
                        {
                            if (IsNumberVariable(arg0))
                            {
                                if (StringHelper.IsNumeric(arg2))
                                    SetVariableNumber(arg0, StringHelper.StoD(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else if (IsStringVariable(arg0))
                                SetVariableString(arg0, arg2);
                            else if (VariableWaiting(arg0))
                            {
                                if (StringHelper.IsNumeric(arg2))
                                    SetVariableNumber(arg0, StringHelper.StoD(before + "." + after));
                                else
                                    SetVariableString(arg0, arg2);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                        }
                    }
                    else
                    {
                        if (VariableWaiting(arg0))
                        {
                            if (StringHelper.IsNumeric(arg2))
                                SetVariableNumber(arg0, StringHelper.StoD(arg2));
                            else
                                SetVariableString(arg0, arg2);
                        }
                        else if (arg2 == "null")
                        {
                            if (IsStringVariable(arg0) || IsNumberVariable(arg0))
                                SetVariableNull(arg0);
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else if (engine.ConstantExists(arg2))
                        {
                            if (IsStringVariable(arg0))
                            {
                                if (engine.IsNumberConstant(arg2))
                                    SetVariableString(arg0, StringHelper.DtoS(engine.GetConstantNumber(arg2)));
                                else if (engine.IsStringConstant(arg2))
                                    SetVariableString(arg0, engine.GetConstantString(arg2));
                            }
                            else if (IsNumberVariable(arg0))
                            {
                                if (engine.IsNumberConstant(arg2))
                                    SetVariableNumber(arg0, engine.GetConstantNumber(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else if (MethodExists(arg2))
                        {
                            parse(arg2);

                            if (IsStringVariable(arg0))
                                SetVariableString(arg0, __LastValue);
                            else if (IsNumberVariable(arg0))
                                SetVariableNumber(arg0, StringHelper.StoD(__LastValue));
                        }
                        else if (VariableExists(arg2))
                        {
                            if (IsStringVariable(arg2))
                            {
                                if (IsStringVariable(arg0))
                                    SetVariableString(arg0, GetVariableString(arg2));
                                else if (IsNumberVariable(arg0))
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else if (IsNumberVariable(arg2))
                            {
                                if (IsStringVariable(arg0))
                                    SetVariableString(arg0, StringHelper.DtoS(GetVariableNumber(arg2)));
                                else if (IsNumberVariable(arg0))
                                    SetVariableNumber(arg0, GetVariableNumber(arg2));
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

                                if (IsNumberVariable(arg0))
                                {
                                    if (StringHelper.IsNumeric(passworder))
                                        SetVariableNumber(arg0, StringHelper.StoD(passworder));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, passworder, false);
                                }
                                else if (IsStringVariable(arg0))
                                    SetVariableString(arg0, passworder);
                                else
                                    SetVariableString(arg0, passworder);
                            }
                            else
                            {
                                string line = "";
                                cout = "readline: ";
                                line = Console.ReadLine();

                                if (StringHelper.IsNumeric(line))
                                    CreateVariableNumber(arg0, StringHelper.StoD(line));
                                else
                                    CreateVariableString(arg0, line);
                            }
                        }
                        else if (StringHelper.ContainsParameters(arg2))
                        {
                            if (MethodExists(StringHelper.BeforeParameters(arg2)))
                            {
                                // execute the method
                                executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));
                                // set the variable = last value
                                if (IsStringVariable(arg0))
                                {
                                    SetVariableString(arg0, __LastValue);
                                }
                                else if (IsNumberVariable(arg0))
                                {
                                    SetVariableNumber(arg0, StringHelper.StoD(__LastValue));
                                }
                            }
                            else if (isStringStack(arg2))
                            {
                                if (IsStringVariable(arg0))
                                    SetVariableString(arg0, getStringStack(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else if (stackReady(arg2))
                            {
                                if (IsStringVariable(arg0))
                                    SetVariableString(arg0, StringHelper.DtoS(getStack(arg2)));
                                else if (IsNumberVariable(arg0))
                                    SetVariableNumber(arg0, getStack(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else
                        {
                            if (StringHelper.IsNumeric(arg2))
                            {
                                if (IsNumberVariable(arg0))
                                    SetVariableNumber(arg0, StringHelper.StoD(arg2));
                                else if (IsStringVariable(arg0))
                                    SetVariableString(arg0, arg2);
                            }
                            else
                            {
                                if (IsNumberVariable(arg0))
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                else if (IsStringVariable(arg0))
                                    SetVariableString(arg0, cleanString(arg2));
                            }
                        }
                    }
                }
                else
                {
                    if (arg1 == "+=")
                    {
                        if (VariableExists(arg2))
                        {
                            if (IsStringVariable(arg0))
                            {
                                if (IsStringVariable(arg2))
                                    SetVariableString(arg0, GetVariableString(arg0) + GetVariableString(arg2));
                                else if (IsNumberVariable(arg2))
                                    SetVariableString(arg0, GetVariableString(arg0) + StringHelper.DtoS(GetVariableNumber(arg2)));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else if (IsNumberVariable(arg0))
                            {
                                if (IsStringVariable(arg2))
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                                else if (IsNumberVariable(arg2))
                                    SetVariableNumber(arg0, GetVariableNumber(arg0) + GetVariableNumber(arg2));
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
                                if (isStringStack(arg2))
                                {
                                    if (IsStringVariable(arg0))
                                        SetVariableString(arg0, GetVariableString(arg0) + getStringStack(arg2));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else if (stackReady(arg2))
                                {
                                    if (IsNumberVariable(arg0))
                                        SetVariableNumber(arg0, GetVariableNumber(arg0) + getStack(arg2));
                                }
                                else if (MethodExists(StringHelper.BeforeParameters(arg2)))
                                {
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (IsStringVariable(arg0))
                                        SetVariableString(arg0, GetVariableString(arg0) + __LastValue);
                                    else if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVariableNumber(arg0, GetVariableNumber(arg0) + StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                }
                                else if (engine.ObjectExists(StringHelper.BeforeDot(arg2)))
                                {
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (IsStringVariable(arg0))
                                        SetVariableString(arg0, GetVariableString(arg0) + __LastValue);
                                    else if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVariableNumber(arg0, GetVariableNumber(arg0) + StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                }
                            }
                            else if (MethodExists(arg2))
                            {
                                parse(arg2);

                                if (IsStringVariable(arg0))
                                    SetVariableString(arg0, GetVariableString(arg0) + __LastValue);
                                else if (IsNumberVariable(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        SetVariableNumber(arg0, GetVariableNumber(arg0) + StringHelper.StoD(__LastValue));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (IsStringVariable(arg0))
                                    SetVariableString(arg0, GetVariableString(arg0) + arg2);
                                else if (IsNumberVariable(arg0))
                                    SetVariableNumber(arg0, GetVariableNumber(arg0) + StringHelper.StoD(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                            {
                                if (IsStringVariable(arg0))
                                    SetVariableString(arg0, GetVariableString(arg0) + cleanString(arg2));
                                else if (IsNumberVariable(arg0))
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                    }
                    else if (arg1 == "-=")
                    {
                        if (VariableExists(arg2))
                        {
                            if (IsStringVariable(arg0))
                            {
                                if (IsStringVariable(arg2))
                                {
                                    if (GetVariableString(arg2).Length == 1)
                                        SetVariableString(arg0, StringHelper.SubtractChars(GetVariableString(arg0), GetVariableString(arg2)));
                                    else
                                        SetVariableString(arg0, StringHelper.SubtractString(GetVariableString(arg0), GetVariableString(arg2)));
                                }
                                else if (IsNumberVariable(arg2))
                                    SetVariableString(arg0, StringHelper.SubtractString(GetVariableString(arg0), StringHelper.DtoS(GetVariableNumber(arg2))));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else if (IsNumberVariable(arg0))
                            {
                                if (IsStringVariable(arg2))
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                                else if (IsNumberVariable(arg2))
                                    SetVariableNumber(arg0, GetVariableNumber(arg0) - GetVariableNumber(arg2));
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
                                if (isStringStack(arg2))
                                {
                                    if (IsStringVariable(arg0))
                                        SetVariableString(arg0, StringHelper.SubtractString(GetVariableString(arg0), getStringStack(arg2)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else if (stackReady(arg2))
                                {
                                    if (IsNumberVariable(arg0))
                                        SetVariableNumber(arg0, GetVariableNumber(arg0) - getStack(arg2));
                                }
                                else if (MethodExists(StringHelper.BeforeParameters(arg2)))
                                {
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (IsStringVariable(arg0))
                                        SetVariableString(arg0, StringHelper.SubtractString(GetVariableString(arg0), __LastValue));
                                    else if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVariableNumber(arg0, GetVariableNumber(arg0) - StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                }
                                else if (engine.ObjectExists(StringHelper.BeforeDot(arg2)))
                                {
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (IsStringVariable(arg0))
                                        SetVariableString(arg0, StringHelper.SubtractString(GetVariableString(arg0), __LastValue));
                                    else if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVariableNumber(arg0, GetVariableNumber(arg0) - StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                                }
                            }
                            else if (MethodExists(arg2))
                            {
                                parse(arg2);

                                if (IsStringVariable(arg0))
                                    SetVariableString(arg0, StringHelper.SubtractString(GetVariableString(arg0), __LastValue));
                                else if (IsNumberVariable(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        SetVariableNumber(arg0, GetVariableNumber(arg0) - StringHelper.StoD(__LastValue));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (IsStringVariable(arg0))
                                {
                                    if (arg2.Length == 1)
                                        SetVariableString(arg0, StringHelper.SubtractChars(GetVariableString(arg0), arg2));
                                    else
                                        SetVariableString(arg0, StringHelper.SubtractString(GetVariableString(arg0), arg2));
                                }
                                else if (IsNumberVariable(arg0))
                                    SetVariableNumber(arg0, GetVariableNumber(arg0) - StringHelper.StoD(arg2));
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                            {
                                if (IsStringVariable(arg0))
                                {
                                    if (arg2.Length == 1)
                                        SetVariableString(arg0, StringHelper.SubtractChars(GetVariableString(arg0), arg2));
                                    else
                                        SetVariableString(arg0, StringHelper.SubtractString(GetVariableString(arg0), cleanString(arg2)));
                                }
                                else if (IsNumberVariable(arg0))
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                else
                                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                    }
                    else if (arg1 == "*=")
                    {
                        if (VariableExists(arg2))
                        {
                            if (IsNumberVariable(arg2))
                                SetVariableNumber(arg0, GetVariableNumber(arg0) * GetVariableNumber(arg2));
                            else if (IsStringVariable(arg2))
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else
                        {
                            if (StringHelper.ContainsParameters(arg2))
                            {
                                if (stackReady(arg2))
                                {
                                    if (IsNumberVariable(arg0))
                                        SetVariableNumber(arg0, GetVariableNumber(arg0) * getStack(arg2));
                                }
                                else if (MethodExists(StringHelper.BeforeParameters(arg2)))
                                {
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVariableNumber(arg0, GetVariableNumber(arg0) * StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                                else if (engine.ObjectExists(StringHelper.BeforeDot(arg2)))
                                {
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVariableNumber(arg0, GetVariableNumber(arg0) * StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                            }
                            else if (MethodExists(arg2))
                            {
                                parse(arg2);

                                if (IsNumberVariable(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        SetVariableNumber(arg0, GetVariableNumber(arg0) * StringHelper.StoD(__LastValue));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (IsNumberVariable(arg0))
                                    SetVariableNumber(arg0, GetVariableNumber(arg0) * StringHelper.StoD(arg2));
                            }
                            else
                                SetVariableString(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "%=")
                    {
                        if (VariableExists(arg2))
                        {
                            if (IsNumberVariable(arg2))
                                SetVariableNumber(arg0, (int)GetVariableNumber(arg0) % (int)GetVariableNumber(arg2));
                            else if (IsStringVariable(arg2))
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else if (MethodExists(arg2))
                        {
                            parse(arg2);

                            if (IsNumberVariable(arg0))
                            {
                                if (StringHelper.IsNumeric(__LastValue))
                                    SetVariableNumber(arg0, (int)GetVariableNumber(arg0) % (int)StringHelper.StoD(__LastValue));
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
                                if (IsNumberVariable(arg0))
                                    SetVariableNumber(arg0, (int)GetVariableNumber(arg0) % (int)StringHelper.StoD(arg2));
                            }
                            else
                                SetVariableString(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "**=")
                    {
                        if (VariableExists(arg2))
                        {
                            if (IsNumberVariable(arg2))
                                SetVariableNumber(arg0, System.Math.Pow(GetVariableNumber(arg0), GetVariableNumber(arg2)));
                            else if (IsStringVariable(arg2))
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else
                        {
                            if (StringHelper.ContainsParameters(arg2))
                            {
                                if (stackReady(arg2))
                                {
                                    if (IsNumberVariable(arg0))
                                        SetVariableNumber(arg0, System.Math.Pow(GetVariableNumber(arg0), (int)getStack(arg2)));
                                }
                                else if (MethodExists(StringHelper.BeforeParameters(arg2)))
                                {
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVariableNumber(arg0, System.Math.Pow(GetVariableNumber(arg0), (int)StringHelper.StoD(__LastValue)));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                                else if (engine.ObjectExists(StringHelper.BeforeDot(arg2)))
                                {
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVariableNumber(arg0, System.Math.Pow(GetVariableNumber(arg0), (int)StringHelper.StoD(__LastValue)));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                            }
                            else if (MethodExists(arg2))
                            {
                                parse(arg2);

                                if (IsNumberVariable(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        SetVariableNumber(arg0, System.Math.Pow(GetVariableNumber(arg0), (int)StringHelper.StoD(__LastValue)));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (IsNumberVariable(arg0))
                                    SetVariableNumber(arg0, System.Math.Pow(GetVariableNumber(arg0), StringHelper.StoD(arg2)));
                            }
                            else
                                SetVariableString(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "/=")
                    {
                        if (VariableExists(arg2))
                        {
                            if (IsNumberVariable(arg2))
                                SetVariableNumber(arg0, GetVariableNumber(arg0) / GetVariableNumber(arg2));
                            else if (IsStringVariable(arg2))
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else
                        {
                            if (StringHelper.ContainsParameters(arg2))
                            {
                                if (stackReady(arg2))
                                {
                                    if (IsNumberVariable(arg0))
                                        SetVariableNumber(arg0, GetVariableNumber(arg0) / getStack(arg2));
                                }
                                else if (MethodExists(StringHelper.BeforeParameters(arg2)))
                                {
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVariableNumber(arg0, GetVariableNumber(arg0) / StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                                else if (engine.ObjectExists(StringHelper.BeforeDot(arg2)))
                                {
                                    executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                                    if (IsNumberVariable(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVariableNumber(arg0, GetVariableNumber(arg0) / StringHelper.StoD(__LastValue));
                                        else
                                            ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                            }
                            else if (MethodExists(arg2))
                            {
                                parse(arg2);

                                if (IsNumberVariable(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        SetVariableNumber(arg0, GetVariableNumber(arg0) / StringHelper.StoD(__LastValue));
                                    else
                                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.NULL_NUMBER, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (IsNumberVariable(arg0))
                                    SetVariableNumber(arg0, GetVariableNumber(arg0) / StringHelper.StoD(arg2));
                            }
                            else
                                SetVariableString(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "++=")
                    {
                        if (VariableExists(arg2))
                        {
                            if (IsNumberVariable(arg2))
                            {
                                if (IsStringVariable(arg0))
                                {
                                    int tempVarNumber = ((int)GetVariableNumber(arg2));
                                    string tempVarString = (GetVariableString(arg0));
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) + tempVarNumber));

                                    SetVariableString(arg0, cleaned);
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
                                string tempVarString = (GetVariableString(arg0));

                                if (tempVarString != __Null)
                                {
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) + tempVarNumber));

                                    SetVariableString(arg0, cleaned);
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
                        if (VariableExists(arg2))
                        {
                            if (IsNumberVariable(arg2))
                            {
                                if (IsStringVariable(arg0))
                                {
                                    int tempVarNumber = ((int)GetVariableNumber(arg2));
                                    string tempVarString = (GetVariableString(arg0));
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) - tempVarNumber));

                                    SetVariableString(arg0, cleaned);
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
                                string tempVarString = (GetVariableString(arg0));

                                if (tempVarString != __Null)
                                {
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) - tempVarNumber));

                                    SetVariableString(arg0, cleaned);
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
                        if (VariableExists(arg2))
                        {
                            if (IsStringVariable(arg2))
                            {
                                if (IsStringVariable(arg0))
                                    SetVariableString(arg0, getStdout(GetVariableString(arg2)));
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (IsStringVariable(arg0))
                                SetVariableString(arg0, getStdout(cleanString(arg2)));
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                        }
                    }
                    else if (arg1 == "!")
                    {
                        if (VariableExists(arg2))
                        {
                            if (IsStringVariable(arg2))
                            {
                                if (IsStringVariable(arg0))
                                    SetVariableString(arg0, getParsedOutput(GetVariableString(arg2)));
                                else
                                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (IsStringVariable(arg0))
                                SetVariableString(arg0, getParsedOutput(cleanString(arg2)));
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
                            if (VariableExists(arg2))
                            {
                                if (IsStringVariable(arg2))
                                    engine.ListReplace(before, after, GetVariableString(arg2));
                                else if (IsNumberVariable(arg2))
                                    engine.ListReplace(before, after, StringHelper.DtoS(GetVariableNumber(arg2)));
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
                            if (VariableExists(arg2))
                            {
                                if (IsStringVariable(arg2))
                                    engine.ListReplace(before, after, GetVariableString(arg2));
                                else if (IsNumberVariable(arg2))
                                    engine.ListReplace(before, after, StringHelper.DtoS(GetVariableNumber(arg2)));
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
            else if (VariableExists(_b) && StringHelper.ContainsString(_a, "split") && arg1 == "=")
            {
                if (IsStringVariable(_b))
                {
                    System.Collections.Generic.List<string> parameters = StringHelper.GetParameters(_a);
                    System.Collections.Generic.List<string> elements = new();

                    if (parameters[0] == "")
                        elements = StringHelper.SplitString(GetVariableString(_b), ' ');
                    else
                    {
                        if (parameters[0][0] == ';')
                            elements = StringHelper.SplitString(GetVariableString(_b), ';');
                        else
                            elements = StringHelper.SplitString(GetVariableString(_b), parameters[0][0]);
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
                        if (VariableExists(parameters[i]))
                        {
                            if (IsStringVariable(parameters[i]))
                                engine.RemoveFromList(arg0, GetVariableString(parameters[i]));
                            else if (IsNumberVariable(parameters[i]))
                                engine.RemoveFromList(arg0, StringHelper.DtoS(GetVariableNumber(parameters[i])));
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
            else if (VariableExists(arg2)) // ADD/REMOVE VARIABLE VALUE TO/FROM LIST
            {
                if (arg1 == "+=")
                {
                    if (IsStringVariable(arg2))
                        engine.AddToList(arg0, GetVariableString(arg2));
                    else if (IsNumberVariable(arg2))
                        engine.AddToList(arg0, StringHelper.DtoS(GetVariableNumber(arg2)));
                    else
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else if (arg1 == "-=")
                {
                    if (IsStringVariable(arg2))
                        engine.RemoveFromList(arg0, GetVariableString(arg2));
                    else if (IsNumberVariable(arg2))
                        engine.RemoveFromList(arg0, StringHelper.DtoS(GetVariableNumber(arg2)));
                    else
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else if (MethodExists(arg2)) // INITIALIZE LIST FROM METHOD RETURN
            {
                parse(arg2);

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

                if (StringHelper.ContainsBrackets(arg2) && (VariableExists(StringHelper.BeforeBrackets(arg2)) || engine.ListExists(StringHelper.BeforeBrackets(arg2))))
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
                                    CreateVariableNumber(arg0, StringHelper.StoD(listValue));
                                else
                                    CreateVariableString(arg0, listValue);
                            }
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
                    }
                    else if (VariableExists(beforeBracket))
                        setSubString(arg0, arg2, beforeBracket);
                    else
                        ErrorLogger.Error(ErrorLogger.LIST_UNDEFINED, beforeBracket, false);
                }
                else if (engine.ListExists(before) && after == "size")
                    CreateVariableNumber(arg0, StringHelper.StoD(StringHelper.ItoS(engine.GetListSize(before))));
                else if (before == "self")
                {
                    if (engine.ObjectExists(__CurrentMethodObject))
                        twoSpace(arg0, arg1, (__CurrentMethodObject + "." + after), (arg0 + " " + arg1 + " " + (__CurrentMethodObject + "." + after)), command);
                    else
                        twoSpace(arg0, arg1, after, (arg0 + " " + arg1 + " " + after), command);
                }
                else if (after == "to_integer")
                {
                    if (VariableExists(before))
                    {
                        if (IsStringVariable(before))
                            CreateVariableNumber(arg0, (int)GetVariableString(before)[0]);
                        else if (IsNumberVariable(before))
                        {
                            int i = (int)GetVariableNumber(before);
                            CreateVariableNumber(arg0, (double)i);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_double")
                {
                    if (VariableExists(before))
                    {
                        if (IsStringVariable(before))
                            CreateVariableNumber(arg0, (double)GetVariableString(before)[0]);
                        else if (IsNumberVariable(before))
                        {
                            double i = GetVariableNumber(before);
                            CreateVariableNumber(arg0, (double)i);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_string")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableString(arg0, StringHelper.DtoS(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_number")
                {
                    if (VariableExists(before))
                    {
                        if (IsStringVariable(before))
                            CreateVariableNumber(arg0, StringHelper.StoD(GetVariableString(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (engine.ObjectExists(before))
                {
                    if (ObjectMethodExists(before, after) && !StringHelper.ContainsParameters(after))
                    {
                        parse(arg2);

                        if (StringHelper.IsNumeric(__LastValue))
                            CreateVariableNumber(arg0, StringHelper.StoD(__LastValue));
                        else
                            CreateVariableString(arg0, __LastValue);
                    }
                    else if (StringHelper.ContainsParameters(after))
                    {
                        if (ObjectMethodExists(before, StringHelper.BeforeParameters(after)))
                        {
                            executeTemplate(GetObjectMethod(before, StringHelper.BeforeParameters(after)), StringHelper.GetParameters(after));

                            if (StringHelper.IsNumeric(__LastValue))
                                CreateVariableNumber(arg0, StringHelper.StoD(__LastValue));
                            else
                                CreateVariableString(arg0, __LastValue);
                        }
                        else
                            sysExec(s, command);
                    }
                    else if (ObjectVariableExists(before, after))
                    {
                        if (GetObjectVariableString(before, after) != __Null)
                            CreateVariableString(arg0, GetObjectVariableString(before, after));
                        else if (GetObjectVariableNumber(before, after) != __NullNum)
                            CreateVariableNumber(arg0, GetObjectVariableNumber(before, after));
                        else
                            ErrorLogger.Error(ErrorLogger.IS_NULL, GetObjectVariableName(before, after), false);
                    }
                }
                else if (VariableExists(before) && after == "read")
                {
                    if (IsStringVariable(before))
                    {
                        if (System.IO.File.Exists(GetVariableString(before)))
                        {
                            string bigString = ("");
                            foreach (var line in System.IO.File.ReadAllLines(GetVariableString(before)))
                            {
                                bigString += (line + "\r\n");
                            }
                            CreateVariableString(arg0, bigString);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(before), false);
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
                            newVariable.setPrivate();
                        else if (__DefiningPublicCode)
                            newVariable.setPublic();

                        CreateObjectVariable(__CurrentObject, newVariable);
                    }
                    else
                    {
                        Variable newVariable = new(arg0, arg2);

                        if (__DefiningPrivateCode)
                            newVariable.setPrivate();
                        else if (__DefiningPublicCode)
                            newVariable.setPublic();

                        CreateObjectVariable(__CurrentObject, newVariable);
                    }
                }
                else if (arg2 == "null")
                    CreateVariableString(arg0, arg2);
                else if (MethodExists(arg2))
                {
                    parse(arg2);

                    if (StringHelper.IsNumeric(__LastValue))
                        CreateVariableNumber(arg0, StringHelper.StoD(__LastValue));
                    else
                        CreateVariableString(arg0, __LastValue);
                }
                else if (engine.ConstantExists(arg2))
                {
                    if (engine.IsNumberConstant(arg2))
                        CreateVariableNumber(arg0, engine.GetConstantNumber(arg2));
                    else if (engine.IsStringConstant(arg2))
                        CreateVariableString(arg0, engine.GetConstantString(arg2));
                    else
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else if (StringHelper.ContainsParameters(arg2))
                {
                    if (isStringStack(arg2))
                        CreateVariableString(arg0, getStringStack(arg2));
                    else if (stackReady(arg2))
                        CreateVariableNumber(arg0, getStack(arg2));
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
                                    CreateVariableNumber(arg0, (int)random(n0, n2));
                                else if (n0 > n2)
                                    CreateVariableNumber(arg0, (int)random(n2, n0));
                                else
                                    CreateVariableNumber(arg0, (int)random(n0, n2));
                            }
                            else if (StringHelper.IsAlphabetical(s0) && StringHelper.IsAlphabetical(s2))
                            {
                                if (StringHelper.GetCharAsInt32(s0[0]) < StringHelper.GetCharAsInt32(s2[0]))
                                    CreateVariableString(arg0, random(s0, s2));
                                else if (StringHelper.GetCharAsInt32(s0[0]) > StringHelper.GetCharAsInt32(s2[0]))
                                    CreateVariableString(arg0, random(s2, s0));
                                else
                                    CreateVariableString(arg0, random(s2, s0));
                            }
                            else if (VariableExists(s0) || VariableExists(s2))
                            {
                                if (VariableExists(s0))
                                {
                                    if (IsNumberVariable(s0))
                                        s0 = StringHelper.DtoS(GetVariableNumber(s0));
                                    else if (IsStringVariable(s0))
                                        s0 = GetVariableString(s0);
                                }

                                if (VariableExists(s2))
                                {
                                    if (IsNumberVariable(s2))
                                        s2 = StringHelper.DtoS(GetVariableNumber(s2));
                                    else if (IsStringVariable(s2))
                                        s2 = GetVariableString(s2);
                                }

                                if (StringHelper.IsNumeric(s0) && StringHelper.IsNumeric(s2))
                                {
                                    double n0 = StringHelper.StoD(s0), n2 = StringHelper.StoD(s2);

                                    if (n0 < n2)
                                        CreateVariableNumber(arg0, (int)random(n0, n2));
                                    else if (n0 > n2)
                                        CreateVariableNumber(arg0, (int)random(n2, n0));
                                    else
                                        CreateVariableNumber(arg0, (int)random(n0, n2));
                                }
                                else if (StringHelper.IsAlphabetical(s0) && StringHelper.IsAlphabetical(s2))
                                {
                                    if (StringHelper.GetCharAsInt32(s0[0]) < StringHelper.GetCharAsInt32(s2[0]))
                                        CreateVariableString(arg0, random(s0, s2));
                                    else if (StringHelper.GetCharAsInt32(s0[0]) > StringHelper.GetCharAsInt32(s2[0]))
                                        CreateVariableString(arg0, random(s2, s0));
                                    else
                                        CreateVariableString(arg0, random(s2, s0));
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
                        executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), StringHelper.GetParameters(arg2));

                        if (StringHelper.IsNumeric(__LastValue))
                            CreateVariableNumber(arg0, StringHelper.StoD(__LastValue));
                        else
                            CreateVariableString(arg0, __LastValue);
                    }
                }
                else if (VariableExists(arg2))
                {
                    if (IsNumberVariable(arg2))
                        CreateVariableNumber(arg0, GetVariableNumber(arg2));
                    else if (IsStringVariable(arg2))
                        CreateVariableString(arg0, GetVariableString(arg2));
                    else
                        CreateVariableString(arg0, __Null);
                }
                else if (arg2 == "password" || arg2 == "readline")
                {
                    string line = "";
                    if (arg2 == "password")
                    {
                        line = getSilentOutput("");

                        if (StringHelper.IsNumeric(line))
                            CreateVariableNumber(arg0, StringHelper.StoD(line));
                        else
                            CreateVariableString(arg0, line);
                    }
                    else
                    {
                        cout = "readline: ";
                        line = Console.ReadLine();

                        if (StringHelper.IsNumeric(line))
                            CreateVariableNumber(arg0, StringHelper.StoD(line));
                        else
                            CreateVariableString(arg0, line);
                    }
                }
                else if (arg2 == "args.size")
                    CreateVariableNumber(arg0, (double)__ArgumentCount);
                else if (before == "readline")
                {
                    if (VariableExists(after))
                    {
                        if (IsStringVariable(after))
                        {
                            string line = "";
                            cout = cleanString(GetVariableString(after));
                            line = Console.ReadLine();

                            if (StringHelper.IsNumeric(line))
                                CreateVariableNumber(arg0, StringHelper.StoD(line));
                            else
                                CreateVariableString(arg0, line);
                        }
                        else
                        {
                            string line = "";
                            cout = "readline: ";
                            line = Console.ReadLine();

                            if (StringHelper.IsNumeric(line))
                                CreateVariableNumber(arg0, StringHelper.StoD(line));
                            else
                                CreateVariableString(arg0, line);
                        }
                    }
                    else
                    {
                        string line = "";
                        cout = cleanString(after);
                        line = Console.ReadLine();

                        if (StringHelper.IsNumeric(line))
                            CreateVariableNumber(arg0, StringHelper.StoD(line));
                        else
                            CreateVariableString(arg0, line);
                    }
                }
                else if (before == "password")
                {
                    if (VariableExists(after))
                    {
                        if (IsStringVariable(after))
                        {
                            string line = "";
                            line = getSilentOutput(GetVariableString(after));

                            if (StringHelper.IsNumeric(line))
                                CreateVariableNumber(arg0, StringHelper.StoD(line));
                            else
                                CreateVariableString(arg0, line);

                            cout = System.Environment.NewLine;
                        }
                        else
                        {
                            string line = "";
                            line = getSilentOutput("password: ");

                            if (StringHelper.IsNumeric(line))
                                CreateVariableNumber(arg0, StringHelper.StoD(line));
                            else
                                CreateVariableString(arg0, line);

                            cout = System.Environment.NewLine;
                        }
                    }
                    else
                    {
                        string line = "";
                        line = getSilentOutput(cleanString(after));

                        if (StringHelper.IsNumeric(line))
                            CreateVariableNumber(arg0, StringHelper.StoD(line));
                        else
                            CreateVariableString(arg0, line);

                        cout = System.Environment.NewLine;
                    }
                }
                else if (after == "size")
                {
                    if (VariableExists(before))
                    {
                        if (IsStringVariable(before))
                            CreateVariableNumber(arg0, (double)GetVariableString(before).Length);
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        CreateVariableNumber(arg0, (double)before.Length);
                }
                else if (after == "sin")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Sin(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "sinh")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Sinh(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "asin")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Asin(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "tan")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Tan(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "tanh")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Tanh(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "atan")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Atan(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "cos")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Cos(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "acos")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Acos(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "cosh")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Cosh(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "log")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Log(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "sqrt")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Sqrt(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "abs")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Abs(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "floor")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Floor(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "ceil")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Ceiling(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "exp")
                {
                    if (VariableExists(before))
                    {
                        if (IsNumberVariable(before))
                            CreateVariableNumber(arg0, System.Math.Exp(GetVariableNumber(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_upper")
                {
                    if (VariableExists(before))
                    {
                        if (IsStringVariable(before))
                            CreateVariableString(arg0, StringHelper.ToUppercase(GetVariableString(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_lower")
                {
                    if (VariableExists(before))
                    {
                        if (IsStringVariable(before))
                            CreateVariableString(arg0, StringHelper.ToLowercase(GetVariableString(before)));
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "bytes")
                {
                    if (VariableExists(before))
                    {
                        if (IsStringVariable(before))
                        {
                            if (System.IO.File.Exists(GetVariableString(before)))
                                CreateVariableNumber(arg0, getBytes(GetVariableString(before)));
                            else
                                ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(before), false);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                    {
                        if (System.IO.File.Exists(before))
                            CreateVariableNumber(arg0, getBytes(before));
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
                        CreateVariableNumber(arg0, StringHelper.StoD(arg2));
                    else
                        CreateVariableString(arg0, cleanString(arg2));
                }
            }
            else if (arg1 == "+=")
            {
                if (VariableExists(arg2))
                {
                    if (IsStringVariable(arg2))
                        CreateVariableString(arg0, GetVariableString(arg2));
                    else if (IsNumberVariable(arg2))
                        CreateVariableNumber(arg0, GetVariableNumber(arg2));
                    else
                        CreateVariableString(arg0, __Null);
                }
                else
                {
                    if (StringHelper.IsNumeric(arg2))
                        CreateVariableNumber(arg0, StringHelper.StoD(arg2));
                    else
                        CreateVariableString(arg0, cleanString(arg2));
                }
            }
            else if (arg1 == "-=")
            {
                if (VariableExists(arg2))
                {
                    if (IsNumberVariable(arg2))
                        CreateVariableNumber(arg0, 0 - GetVariableNumber(arg2));
                    else
                        CreateVariableString(arg0, __Null);
                }
                else
                {
                    if (StringHelper.IsNumeric(arg2))
                        CreateVariableNumber(arg0, StringHelper.StoD(arg2));
                    else
                        CreateVariableString(arg0, cleanString(arg2));
                }
            }
            else if (arg1 == "?")
            {
                if (VariableExists(arg2))
                {
                    if (IsStringVariable(arg2))
                        CreateVariableString(arg0, getStdout(GetVariableString(arg2)));
                    else
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                    CreateVariableString(arg0, getStdout(cleanString(arg2)));
            }
            else if (arg1 == "!")
            {
                if (VariableExists(arg2))
                {
                    if (IsStringVariable(arg2))
                        CreateVariableString(arg0, getParsedOutput(GetVariableString(arg2)));
                    else
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                    CreateVariableString(arg0, getParsedOutput(cleanString(arg2)));
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
                    if (GetObjectVariableString(before, after) != __Null)
                        CreateVariableString(arg0, GetObjectVariableString(before, after));
                    else if (GetObjectVariableNumber(before, after) != __NullNum)
                        CreateVariableNumber(arg0, GetObjectVariableNumber(before, after));
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
                    writeline(StringHelper.DtoS(StringHelper.StoD(arg0) + StringHelper.StoD(arg2)));
                else if (arg1 == "-")
                    writeline(StringHelper.DtoS(StringHelper.StoD(arg0) - StringHelper.StoD(arg2)));
                else if (arg1 == "*")
                    writeline(StringHelper.DtoS(StringHelper.StoD(arg0) * StringHelper.StoD(arg2)));
                else if (arg1 == "/")
                    writeline(StringHelper.DtoS(StringHelper.StoD(arg0) / StringHelper.StoD(arg2)));
                else if (arg1 == "**")
                    writeline(StringHelper.DtoS(System.Math.Pow(StringHelper.StoD(arg0), StringHelper.StoD(arg2))));
                else if (arg1 == "%")
                {
                    if ((int)StringHelper.StoD(arg2) == 0)
                        ErrorLogger.Error(ErrorLogger.DIVIDED_BY_ZERO, s, false);
                    else
                        writeline(StringHelper.DtoS((int)StringHelper.StoD(arg0) % (int)StringHelper.StoD(arg2)));
                }
                else
                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else
            {
                if (arg1 == "+")
                    writeline(arg0 + arg2);
                else if (arg1 == "-")
                    writeline(StringHelper.SubtractString(arg0, arg2));
                else if (arg1 == "*")
                {
                    if (!StringHelper.ZeroNumbers(arg2))
                    {
                        string bigstr = string.Empty;
                        for (int i = 0; i < StringHelper.StoI(arg2); i++)
                        {
                            bigstr += (arg0);
                            write(arg0);
                        }

                        SetLastValue(bigstr);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.INVALID_OP, s, false);
                }
                else if (arg1 == "/")
                    writeline(StringHelper.SubtractString(arg0, arg2));
                else
                    ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
        }

        void InternalEncryptDecrypt(string arg0, string arg1)
        {
            Crypt c = new();
            string text = VariableExists(arg1) ? (IsStringVariable(arg1) ? GetVariableString(arg1) : StringHelper.DtoS(GetVariableNumber(arg1))) : arg1;
            write(arg0 == "encrypt" ? c.e(text) : c.d(text));
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
                    cerr = "...no remembered variables" + System.Environment.NewLine;

                System.IO.Directory.CreateDirectory(__SavedVarsPath);

                if (!System.IO.Directory.Exists(__SavedVarsPath) && !System.IO.File.Exists(__SavedVars))
                    cout = "...removed successfully" + System.Environment.NewLine;
                else
                    cerr = "...failed to remove" + System.Environment.NewLine;
            }
            else
                cerr = "...found nothing to remove" + System.Environment.NewLine;
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
            __DontCollectMethodVars = false;
            __FailedIfStatement = false;
            __GoToLabel = false;
            __ExecutedIfStatement = false;
            __InDefaultCase = false;
            __ExecutedMethod = false;
            __DefiningSwitchBlock = false;
            __DefiningIfStatement = false;
            __DefiningForLoop = false;
            __DefiningWhileLoop = false;
            __DefiningModule = false;
            __DefiningPrivateCode = false;
            __DefiningPublicCode = false;
            __DefiningScript = false;
            __ExecutedTemplate = false; // remove
            __ExecutedTryBlock = false;
            __Breaking = false;
            __DefiningMethod = false;
            __MultilineComment = false;
            __Negligence = false;
            __FailedNest = false;
            __DefiningNest = false;
            __DefiningObject = false;
            __DefiningObjectMethod = false;
            __DefiningParameterizedMethod = false;
            __Returning = false;
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
