namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
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
            string tmpObjName = beforeDot(arg0), tmpVarName = afterDot(arg0);
            bool tmpObjExists = OExists(tmpObjName);
            if (tmpObjExists || startsWith(arg0, "@"))
            {
                if (tmpObjExists)
                {
                    if (GetOVString(tmpObjName, tmpVarName) != __Null)
                    {
                        string tempObjectVariableName = ("@ " + tmpObjName + tmpVarName + "_string");
                        CreateVString(tempObjectVariableName, GetOVString(tmpObjName, tmpVarName));
                        twoSpace(tempObjectVariableName, arg1, arg2, "", command);
                        SetVName(tempObjectVariableName, tmpVarName);
                        DeleteOV(tmpObjName, tmpVarName);
                        CreateOV(tmpObjName, GetV(tmpVarName));
                        DeleteV(tmpVarName);
                    }
                    else if (GetOVNumber(tmpObjName, tmpVarName) != __NullNum)
                    {
                        string tempObjectVariableName = ("@____" + beforeDot(arg0) + "___" + afterDot(arg0) + "_number");
                        CreateVNumber(tempObjectVariableName, GetOVNumber(beforeDot(arg0), afterDot(arg0)));
                        twoSpace(tempObjectVariableName, arg1, arg2, tempObjectVariableName + " " + arg1 + " " + arg2, command);
                        SetVName(tempObjectVariableName, afterDot(arg0));
                        DeleteOV(beforeDot(arg0), afterDot(arg0));
                        CreateOV(beforeDot(arg0), GetV(afterDot(arg0)));
                        DeleteV(afterDot(arg0));
                    }
                }
                else if (arg1 == "=")
                {
                    string before = (beforeDot(arg2)), after = (afterDot(arg2));

                    if (containsBrackets(arg2) && (VExists(beforeBrackets(arg2)) || LExists(beforeBrackets(arg2))))
                    {
                        string beforeBracket = (beforeBrackets(arg2)), afterBracket = (afterBrackets(arg2));

                        afterBracket = subtractString(afterBracket, "]");

                        if (LExists(beforeBracket))
                        {
                            if (GetLSize(beforeBracket) >= stoi(afterBracket))
                            {
                                if (GetLLine(beforeBracket, stoi(afterBracket)) == "#!=no_line")
                                    error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
                                else
                                {
                                    string listValue = GetLLine(beforeBracket, stoi(afterBracket));

                                    if (StringHelper.IsNumeric(listValue))
                                    {
                                        if (isNumber(arg0))
                                            SetVNumber(arg0, stod(listValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                    {
                                        if (isString(arg0))
                                            SetVString(arg0, listValue);
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                }
                            }
                        }
                        else if (isString(beforeBracket))
                            setSubString(arg0, arg2, beforeBracket);
                        else
                            error(ErrorLogger.LIST_UNDEFINED, beforeBracket, false);
                    }
                    else if (before.Length != 0 && after.Length != 0)
                    {
                        if (containsParameters(arg2))
                        {
                            if (beforeParameters(arg2) == "random")
                            {
                                if (contains(arg2, ".."))
                                {
                                    System.Collections.Generic.List<string> range = getRange(arg2);
                                    string s0 = (range[0]), s2 = (range[1]);

                                    if (StringHelper.IsNumeric(s0) && StringHelper.IsNumeric(s2))
                                    {
                                        if (isNumber(arg0))
                                        {
                                            double n0 = stod(s0), n2 = stod(s2);

                                            if (n0 < n2)
                                                SetVNumber(arg0, (int)random(n0, n2));
                                            else if (n0 > n2)
                                                SetVNumber(arg0, (int)random(n2, n0));
                                            else
                                                SetVNumber(arg0, (int)random(n0, n2));
                                        }
                                        else if (isString(arg0))
                                        {
                                            double n0 = stod(s0), n2 = stod(s2);

                                            if (n0 < n2)
                                                SetVString(arg0, itos((int)random(n0, n2)));
                                            else if (n0 > n2)
                                                SetVString(arg0, itos((int)random(n2, n0)));
                                            else
                                                SetVString(arg0, itos((int)random(n0, n2)));
                                        }
                                    }
                                    else if (isAlpha(s0) && isAlpha(s2))
                                    {
                                        if (isString(arg0))
                                        {
                                            if (get_alpha_num(s0[0]) < get_alpha_num(s2[0]))
                                                SetVString(arg0, random(s0, s2));
                                            else if (get_alpha_num(s0[0]) > get_alpha_num(s2[0]))
                                                SetVString(arg0, random(s2, s0));
                                            else
                                                SetVString(arg0, random(s2, s0));
                                        }
                                        else
                                            error(ErrorLogger.NULL_STRING, arg0, false);
                                    }
                                    else if (VExists(s0) || VExists(s2))
                                    {
                                        if (VExists(s0))
                                        {
                                            if (isNumber(s0))
                                                s0 = dtos(GetVNumber(s0));
                                            else if (isString(s0))
                                                s0 = GetVString(s0);
                                        }

                                        if (VExists(s2))
                                        {
                                            if (isNumber(s2))
                                                s2 = dtos(GetVNumber(s2));
                                            else if (isString(s2))
                                                s2 = GetVString(s2);
                                        }

                                        if (StringHelper.IsNumeric(s0) && StringHelper.IsNumeric(s2))
                                        {
                                            if (isNumber(arg0))
                                            {
                                                double n0 = stod(s0), n2 = stod(s2);

                                                if (n0 < n2)
                                                    SetVNumber(arg0, (int)random(n0, n2));
                                                else if (n0 > n2)
                                                    SetVNumber(arg0, (int)random(n2, n0));
                                                else
                                                    SetVNumber(arg0, (int)random(n0, n2));
                                            }
                                            else if (isString(arg0))
                                            {
                                                double n0 = stod(s0), n2 = stod(s2);

                                                if (n0 < n2)
                                                    SetVString(arg0, itos((int)random(n0, n2)));
                                                else if (n0 > n2)
                                                    SetVString(arg0, itos((int)random(n2, n0)));
                                                else
                                                    SetVString(arg0, itos((int)random(n0, n2)));
                                            }
                                        }
                                        else if (isAlpha(s0) && isAlpha(s2))
                                        {
                                            if (isString(arg0))
                                            {
                                                if (get_alpha_num(s0[0]) < get_alpha_num(s2[0]))
                                                    SetVString(arg0, random(s0, s2));
                                                else if (get_alpha_num(s0[0]) > get_alpha_num(s2[0]))
                                                    SetVString(arg0, random(s2, s0));
                                                else
                                                    SetVString(arg0, random(s2, s0));
                                            }
                                            else
                                                error(ErrorLogger.NULL_STRING, arg0, false);
                                        }
                                    }
                                    else
                                        error(ErrorLogger.INVALID_SEQ, s0 + "_" + s2, false);
                                }
                                else
                                    error(ErrorLogger.INVALID_SEQ_SEP, arg2, false);
                            }
                        }
                        else if (LExists(before) && after == "size")
                        {
                            if (isNumber(arg0))
                                SetVNumber(arg0, stod(itos(GetLSize(before))));
                            else if (isString(arg0))
                                SetVString(arg0, itos(GetLSize(before)));
                            else
                                error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else if (before == "self")
                        {
                            if (OExists(__CurrentMethodObject))
                                twoSpace(arg0, arg1, (__CurrentMethodObject + "." + after), (arg0 + " " + arg1 + " " + (__CurrentMethodObject + "." + after)), command);
                            else
                                twoSpace(arg0, arg1, after, (arg0 + " " + arg1 + " " + after), command);
                        }
                        else if (OExists(before))
                        {
                            if (OVExists(before, after))
                            {
                                if (GetOVString(before, after) != __Null)
                                    SetVString(arg0, GetOVString(before, after));
                                else if (GetOVNumber(before, after) != __NullNum)
                                    SetVNumber(arg0, GetOVNumber(before, after));
                                else
                                    error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else if (OMExists(before, after) && !containsParameters(after))
                            {
                                parse(arg2);

                                if (isString(arg0))
                                    SetVString(arg0, __LastValue);
                                else if (isNumber(arg0))
                                    SetVNumber(arg0, stod(__LastValue));
                            }
                            else if (containsParameters(after))
                            {
                                if (OMExists(before, beforeParameters(after)))
                                {
                                    executeTemplate(GetOM(before, beforeParameters(after)), getParameters(after));

                                    if (StringHelper.IsNumeric(__LastValue))
                                    {
                                        if (isString(arg0))
                                            SetVString(arg0, __LastValue);
                                        else if (isNumber(arg0))
                                            SetVNumber(arg0, stod(__LastValue));
                                        else
                                            error(ErrorLogger.IS_NULL, arg0, false);
                                    }
                                    else
                                    {
                                        if (isString(arg0))
                                            SetVString(arg0, __LastValue);
                                        else if (isNumber(arg0))
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                        else
                                            error(ErrorLogger.IS_NULL, arg0, false);
                                    }
                                }
                                else
                                    sysExec(s, command);
                            }
                            else
                                error(ErrorLogger.VAR_UNDEFINED, arg2, false);
                        }
                        else if (before == "env")
                        {
                            InternalGetEnv(arg0, after, 1);
                        }
                        else if (after == "to_int")
                        {
                            if (VExists(before))
                            {
                                if (isString(before))
                                    SetVNumber(arg0, (int)GetVString(before)[0]);
                                else if (isNumber(before))
                                {
                                    int i = (int)GetVNumber(before);
                                    SetVNumber(arg0, (double)i);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (after == "to_double")
                        {
                            if (VExists(before))
                            {
                                if (isString(before))
                                    SetVNumber(arg0, (double)GetVString(before)[0]);
                                else if (isNumber(before))
                                {
                                    double i = GetVNumber(before);
                                    SetVNumber(arg0, (double)i);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (after == "to_string")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(before))
                                    SetVString(arg0, dtos(GetVNumber(before)));
                                else
                                    error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (after == "to_number")
                        {
                            if (VExists(before))
                            {
                                if (isString(before))
                                    SetVNumber(arg0, stod(GetVString(before)));
                                else
                                    error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (before == "readline")
                        {
                            if (VExists(after))
                            {
                                if (isString(after))
                                {
                                    string line = "";
                                    write(cleanString(GetVString(after)));
                                    line = Console.ReadLine();

                                    if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(line))
                                            SetVNumber(arg0, stod(line));
                                        else
                                            error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (isString(arg0))
                                        SetVString(arg0, line);
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);
                                }
                                else
                                {
                                    string line = "";
                                    cout = "readline: ";
                                    line = Console.ReadLine();

                                    if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(line))
                                            SetVNumber(arg0, stod(line));
                                        else
                                            error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (isString(arg0))
                                        SetVString(arg0, line);
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);
                                }
                            }
                            else
                            {
                                string line = "";
                                cout = cleanString(after);
                                line = Console.ReadLine();

                                if (StringHelper.IsNumeric(line))
                                    SetVNumber(arg0, stod(line));
                                else
                                    SetVString(arg0, line);
                            }
                        }
                        else if (before == "password")
                        {
                            if (VExists(after))
                            {
                                if (isString(after))
                                {
                                    string line = "";
                                    line = getSilentOutput(GetVString(after));

                                    if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(line))
                                            SetVNumber(arg0, stod(line));
                                        else
                                            error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (isString(arg0))
                                        SetVString(arg0, line);
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);

                                    cout = System.Environment.NewLine;
                                }
                                else
                                {
                                    string line = "";
                                    line = getSilentOutput("password: ");

                                    if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(line))
                                            SetVNumber(arg0, stod(line));
                                        else
                                            error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (isString(arg0))
                                        SetVString(arg0, line);
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);

                                    cout = System.Environment.NewLine;
                                }
                            }
                            else
                            {
                                string line = ("");
                                line = getSilentOutput(cleanString(after));

                                if (StringHelper.IsNumeric(line))
                                    SetVNumber(arg0, stod(line));
                                else
                                    SetVString(arg0, line);

                                cout = System.Environment.NewLine;
                            }
                        }
                        else if (after == "cos")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Cos(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Cos(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "acos")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Acos(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Acos(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "cosh")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Cosh(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Cosh(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "log")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Log(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Log(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "sqrt")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Sqrt(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Sqrt(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "abs")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Abs(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Abs(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "floor")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Floor(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Floor(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "ceil")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Ceiling(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Ceiling(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "exp")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Exp(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Exp(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "sin")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Sin(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Sin(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "sinh")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Sinh(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Sinh(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "asin")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Asin(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Asin(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "tan")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Tan(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Tan(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "tanh")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Tanh(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Tanh(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "atan")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        SetVNumber(arg0, System.Math.Atan(GetVNumber(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        SetVString(arg0, dtos(System.Math.Atan(GetVNumber(before))));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "to_lower")
                        {
                            if (VExists(before))
                            {
                                if (isString(arg0))
                                {
                                    if (isString(before))
                                        SetVString(arg0, getLower(GetVString(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "read")
                        {
                            if (isString(arg0))
                            {
                                if (VExists(before))
                                {
                                    if (isString(before))
                                    {
                                        if (System.IO.File.Exists(GetVString(before)))
                                        {
                                            string bigString = "";
                                            foreach (var line in System.IO.File.ReadAllLines(GetVString(before)))
                                            {
                                                bigString += line + System.Environment.NewLine;
                                            }
                                            SetVString(arg0, bigString);
                                        }
                                        else
                                            error(ErrorLogger.READ_FAIL, GetVString(before), false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_STRING, before, false);
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
                                        SetVString(arg0, bigString);
                                    }
                                    else
                                        error(ErrorLogger.READ_FAIL, before, false);
                                }
                            }
                            else
                                error(ErrorLogger.NULL_STRING, arg0, false);
                        }
                        else if (after == "to_upper")
                        {
                            if (VExists(before))
                            {
                                if (isString(arg0))
                                {
                                    if (isString(before))
                                        SetVString(arg0, getUpper(GetVString(before)));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "size")
                        {
                            if (VExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isString(before))
                                        SetVNumber(arg0, (double)GetVString(before).Length);
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                            {
                                if (isNumber(arg0))
                                    SetVNumber(arg0, (double)before.Length);
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                        }
                        else if (after == "bytes")
                        {
                            if (isNumber(arg0))
                            {
                                if (VExists(before))
                                {
                                    if (isString(before))
                                    {
                                        if (System.IO.File.Exists(GetVString(before)))
                                            SetVNumber(arg0, getBytes(GetVString(before)));
                                        else
                                            error(ErrorLogger.READ_FAIL, GetVString(before), false);
                                    }
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                {
                                    if (System.IO.File.Exists(before))
                                        SetVNumber(arg0, getBytes(before));
                                    else
                                        error(ErrorLogger.READ_FAIL, before, false);
                                }
                            }
                            else
                                error(ErrorLogger.CONV_ERR, arg0, false);
                        }
                        else
                        {
                            if (isNumber(arg0))
                            {
                                if (StringHelper.IsNumeric(arg2))
                                    SetVNumber(arg0, stod(arg2));
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else if (isString(arg0))
                                SetVString(arg0, arg2);
                            else if (GetVWaiting(arg0))
                            {
                                if (StringHelper.IsNumeric(arg2))
                                    SetVNumber(arg0, stod(before + "." + after));
                                else
                                    SetVString(arg0, arg2);
                            }
                            else
                                error(ErrorLogger.IS_NULL, arg0, false);
                        }
                    }
                    else
                    {
                        if (GetVWaiting(arg0))
                        {
                            if (StringHelper.IsNumeric(arg2))
                                SetVNumber(arg0, stod(arg2));
                            else
                                SetVString(arg0, arg2);
                        }
                        else if (arg2 == "null")
                        {
                            if (isString(arg0) || isNumber(arg0))
                                SetVNull(arg0);
                            else
                                error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else if (CExists(arg2))
                        {
                            if (isString(arg0))
                            {
                                if (IsCNumber(arg2))
                                    SetVString(arg0, dtos(GetCNumber(arg2)));
                                else if (IsCString(arg2))
                                    SetVString(arg0, GetCString(arg2));
                            }
                            else if (isNumber(arg0))
                            {
                                if (IsCNumber(arg2))
                                    SetVNumber(arg0, GetCNumber(arg2));
                                else
                                    error(ErrorLogger.CONV_ERR, arg2, false);
                            }
                            else
                                error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else if (MExists(arg2))
                        {
                            parse(arg2);

                            if (isString(arg0))
                                SetVString(arg0, __LastValue);
                            else if (isNumber(arg0))
                                SetVNumber(arg0, stod(__LastValue));
                        }
                        else if (VExists(arg2))
                        {
                            if (isString(arg2))
                            {
                                if (isString(arg0))
                                    SetVString(arg0, GetVString(arg2));
                                else if (isNumber(arg0))
                                    error(ErrorLogger.CONV_ERR, arg2, false);
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else if (isNumber(arg2))
                            {
                                if (isString(arg0))
                                    SetVString(arg0, dtos(GetVNumber(arg2)));
                                else if (isNumber(arg0))
                                    SetVNumber(arg0, GetVNumber(arg2));
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                                error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else if (arg2 == "password" || arg2 == "readline")
                        {
                            if (arg2 == "password")
                            {
                                string passworder = ("");
                                passworder = getSilentOutput("");

                                if (isNumber(arg0))
                                {
                                    if (StringHelper.IsNumeric(passworder))
                                        SetVNumber(arg0, stod(passworder));
                                    else
                                        error(ErrorLogger.CONV_ERR, passworder, false);
                                }
                                else if (isString(arg0))
                                    SetVString(arg0, passworder);
                                else
                                    SetVString(arg0, passworder);
                            }
                            else
                            {
                                string line = "";
                                cout = "readline: ";
                                line = Console.ReadLine();

                                if (StringHelper.IsNumeric(line))
                                    CreateVNumber(arg0, stod(line));
                                else
                                    CreateVString(arg0, line);
                            }
                        }
                        else if (containsParameters(arg2))
                        {
                            if (MExists(beforeParameters(arg2)))
                            {
                                // execute the method
                                executeTemplate(GetM(beforeParameters(arg2)), getParameters(arg2));
                                // set the variable = last value
                                if (isString(arg0))
                                {
                                    SetVString(arg0, __LastValue);
                                }
                                else if (isNumber(arg0))
                                {
                                    SetVNumber(arg0, stod(__LastValue));
                                }
                            }
                            else if (isStringStack(arg2))
                            {
                                if (isString(arg0))
                                    SetVString(arg0, getStringStack(arg2));
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else if (stackReady(arg2))
                            {
                                if (isString(arg0))
                                    SetVString(arg0, dtos(getStack(arg2)));
                                else if (isNumber(arg0))
                                    SetVNumber(arg0, getStack(arg2));
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else
                        {
                            if (StringHelper.IsNumeric(arg2))
                            {
                                if (isNumber(arg0))
                                    SetVNumber(arg0, stod(arg2));
                                else if (isString(arg0))
                                    SetVString(arg0, arg2);
                            }
                            else
                            {
                                if (isNumber(arg0))
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                                else if (isString(arg0))
                                    SetVString(arg0, cleanString(arg2));
                            }
                        }
                    }
                }
                else
                {
                    if (arg1 == "+=")
                    {
                        if (VExists(arg2))
                        {
                            if (isString(arg0))
                            {
                                if (isString(arg2))
                                    SetVString(arg0, GetVString(arg0) + GetVString(arg2));
                                else if (isNumber(arg2))
                                    SetVString(arg0, GetVString(arg0) + dtos(GetVNumber(arg2)));
                                else
                                    error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else if (isNumber(arg0))
                            {
                                if (isString(arg2))
                                    error(ErrorLogger.CONV_ERR, arg2, false);
                                else if (isNumber(arg2))
                                    SetVNumber(arg0, GetVNumber(arg0) + GetVNumber(arg2));
                                else
                                    error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else
                                error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else
                        {
                            if (containsParameters(arg2))
                            {
                                if (isStringStack(arg2))
                                {
                                    if (isString(arg0))
                                        SetVString(arg0, GetVString(arg0) + getStringStack(arg2));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else if (stackReady(arg2))
                                {
                                    if (isNumber(arg0))
                                        SetVNumber(arg0, GetVNumber(arg0) + getStack(arg2));
                                }
                                else if (MExists(beforeParameters(arg2)))
                                {
                                    executeTemplate(GetM(beforeParameters(arg2)), getParameters(arg2));

                                    if (isString(arg0))
                                        SetVString(arg0, GetVString(arg0) + __LastValue);
                                    else if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVNumber(arg0, GetVNumber(arg0) + stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);
                                }
                                else if (OExists(beforeDot(arg2)))
                                {
                                    executeTemplate(GetM(beforeParameters(arg2)), getParameters(arg2));

                                    if (isString(arg0))
                                        SetVString(arg0, GetVString(arg0) + __LastValue);
                                    else if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVNumber(arg0, GetVNumber(arg0) + stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);
                                }
                            }
                            else if (MExists(arg2))
                            {
                                parse(arg2);

                                if (isString(arg0))
                                    SetVString(arg0, GetVString(arg0) + __LastValue);
                                else if (isNumber(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        SetVNumber(arg0, GetVNumber(arg0) + stod(__LastValue));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (isString(arg0))
                                    SetVString(arg0, GetVString(arg0) + arg2);
                                else if (isNumber(arg0))
                                    SetVNumber(arg0, GetVNumber(arg0) + stod(arg2));
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                            {
                                if (isString(arg0))
                                    SetVString(arg0, GetVString(arg0) + cleanString(arg2));
                                else if (isNumber(arg0))
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                    }
                    else if (arg1 == "-=")
                    {
                        if (VExists(arg2))
                        {
                            if (isString(arg0))
                            {
                                if (isString(arg2))
                                {
                                    if (GetVString(arg2).Length == 1)
                                        SetVString(arg0, subtractChar(GetVString(arg0), GetVString(arg2)));
                                    else
                                        SetVString(arg0, subtractString(GetVString(arg0), GetVString(arg2)));
                                }
                                else if (isNumber(arg2))
                                    SetVString(arg0, subtractString(GetVString(arg0), dtos(GetVNumber(arg2))));
                                else
                                    error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else if (isNumber(arg0))
                            {
                                if (isString(arg2))
                                    error(ErrorLogger.CONV_ERR, arg2, false);
                                else if (isNumber(arg2))
                                    SetVNumber(arg0, GetVNumber(arg0) - GetVNumber(arg2));
                                else
                                    error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else
                                error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else
                        {
                            if (containsParameters(arg2))
                            {
                                if (isStringStack(arg2))
                                {
                                    if (isString(arg0))
                                        SetVString(arg0, subtractString(GetVString(arg0), getStringStack(arg2)));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else if (stackReady(arg2))
                                {
                                    if (isNumber(arg0))
                                        SetVNumber(arg0, GetVNumber(arg0) - getStack(arg2));
                                }
                                else if (MExists(beforeParameters(arg2)))
                                {
                                    executeTemplate(GetM(beforeParameters(arg2)), getParameters(arg2));

                                    if (isString(arg0))
                                        SetVString(arg0, subtractString(GetVString(arg0), __LastValue));
                                    else if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVNumber(arg0, GetVNumber(arg0) - stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);
                                }
                                else if (OExists(beforeDot(arg2)))
                                {
                                    executeTemplate(GetM(beforeParameters(arg2)), getParameters(arg2));

                                    if (isString(arg0))
                                        SetVString(arg0, subtractString(GetVString(arg0), __LastValue));
                                    else if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVNumber(arg0, GetVNumber(arg0) - stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);
                                }
                            }
                            else if (MExists(arg2))
                            {
                                parse(arg2);

                                if (isString(arg0))
                                    SetVString(arg0, subtractString(GetVString(arg0), __LastValue));
                                else if (isNumber(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        SetVNumber(arg0, GetVNumber(arg0) - stod(__LastValue));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (isString(arg0))
                                {
                                    if (arg2.Length == 1)
                                        SetVString(arg0, subtractChar(GetVString(arg0), arg2));
                                    else
                                        SetVString(arg0, subtractString(GetVString(arg0), arg2));
                                }
                                else if (isNumber(arg0))
                                    SetVNumber(arg0, GetVNumber(arg0) - stod(arg2));
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                            {
                                if (isString(arg0))
                                {
                                    if (arg2.Length == 1)
                                        SetVString(arg0, subtractChar(GetVString(arg0), arg2));
                                    else
                                        SetVString(arg0, subtractString(GetVString(arg0), cleanString(arg2)));
                                }
                                else if (isNumber(arg0))
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                    }
                    else if (arg1 == "*=")
                    {
                        if (VExists(arg2))
                        {
                            if (isNumber(arg2))
                                SetVNumber(arg0, GetVNumber(arg0) * GetVNumber(arg2));
                            else if (isString(arg2))
                                error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else
                        {
                            if (containsParameters(arg2))
                            {
                                if (stackReady(arg2))
                                {
                                    if (isNumber(arg0))
                                        SetVNumber(arg0, GetVNumber(arg0) * getStack(arg2));
                                }
                                else if (MExists(beforeParameters(arg2)))
                                {
                                    executeTemplate(GetM(beforeParameters(arg2)), getParameters(arg2));

                                    if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVNumber(arg0, GetVNumber(arg0) * stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                                else if (OExists(beforeDot(arg2)))
                                {
                                    executeTemplate(GetM(beforeParameters(arg2)), getParameters(arg2));

                                    if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVNumber(arg0, GetVNumber(arg0) * stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                            }
                            else if (MExists(arg2))
                            {
                                parse(arg2);

                                if (isNumber(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        SetVNumber(arg0, GetVNumber(arg0) * stod(__LastValue));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    error(ErrorLogger.NULL_NUMBER, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (isNumber(arg0))
                                    SetVNumber(arg0, GetVNumber(arg0) * stod(arg2));
                            }
                            else
                                SetVString(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "%=")
                    {
                        if (VExists(arg2))
                        {
                            if (isNumber(arg2))
                                SetVNumber(arg0, (int)GetVNumber(arg0) % (int)GetVNumber(arg2));
                            else if (isString(arg2))
                                error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else if (MExists(arg2))
                        {
                            parse(arg2);

                            if (isNumber(arg0))
                            {
                                if (StringHelper.IsNumeric(__LastValue))
                                    SetVNumber(arg0, (int)GetVNumber(arg0) % (int)stod(__LastValue));
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                                error(ErrorLogger.NULL_NUMBER, arg0, false);
                        }
                        else
                        {
                            if (StringHelper.IsNumeric(arg2))
                            {
                                if (isNumber(arg0))
                                    SetVNumber(arg0, (int)GetVNumber(arg0) % (int)stod(arg2));
                            }
                            else
                                SetVString(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "**=")
                    {
                        if (VExists(arg2))
                        {
                            if (isNumber(arg2))
                                SetVNumber(arg0, System.Math.Pow(GetVNumber(arg0), GetVNumber(arg2)));
                            else if (isString(arg2))
                                error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else
                        {
                            if (containsParameters(arg2))
                            {
                                if (stackReady(arg2))
                                {
                                    if (isNumber(arg0))
                                        SetVNumber(arg0, System.Math.Pow(GetVNumber(arg0), (int)getStack(arg2)));
                                }
                                else if (MExists(beforeParameters(arg2)))
                                {
                                    executeTemplate(GetM(beforeParameters(arg2)), getParameters(arg2));

                                    if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVNumber(arg0, System.Math.Pow(GetVNumber(arg0), (int)stod(__LastValue)));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                                else if (OExists(beforeDot(arg2)))
                                {
                                    executeTemplate(GetM(beforeParameters(arg2)), getParameters(arg2));

                                    if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVNumber(arg0, System.Math.Pow(GetVNumber(arg0), (int)stod(__LastValue)));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                            }
                            else if (MExists(arg2))
                            {
                                parse(arg2);

                                if (isNumber(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        SetVNumber(arg0, System.Math.Pow(GetVNumber(arg0), (int)stod(__LastValue)));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    error(ErrorLogger.NULL_NUMBER, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (isNumber(arg0))
                                    SetVNumber(arg0, System.Math.Pow(GetVNumber(arg0), stod(arg2)));
                            }
                            else
                                SetVString(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "/=")
                    {
                        if (VExists(arg2))
                        {
                            if (isNumber(arg2))
                                SetVNumber(arg0, GetVNumber(arg0) / GetVNumber(arg2));
                            else if (isString(arg2))
                                error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else
                        {
                            if (containsParameters(arg2))
                            {
                                if (stackReady(arg2))
                                {
                                    if (isNumber(arg0))
                                        SetVNumber(arg0, GetVNumber(arg0) / getStack(arg2));
                                }
                                else if (MExists(beforeParameters(arg2)))
                                {
                                    executeTemplate(GetM(beforeParameters(arg2)), getParameters(arg2));

                                    if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVNumber(arg0, GetVNumber(arg0) / stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                                else if (OExists(beforeDot(arg2)))
                                {
                                    executeTemplate(GetM(beforeParameters(arg2)), getParameters(arg2));

                                    if (isNumber(arg0))
                                    {
                                        if (StringHelper.IsNumeric(__LastValue))
                                            SetVNumber(arg0, GetVNumber(arg0) / stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                            }
                            else if (MExists(arg2))
                            {
                                parse(arg2);

                                if (isNumber(arg0))
                                {
                                    if (StringHelper.IsNumeric(__LastValue))
                                        SetVNumber(arg0, GetVNumber(arg0) / stod(__LastValue));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    error(ErrorLogger.NULL_NUMBER, arg0, false);
                            }
                            else if (StringHelper.IsNumeric(arg2))
                            {
                                if (isNumber(arg0))
                                    SetVNumber(arg0, GetVNumber(arg0) / stod(arg2));
                            }
                            else
                                SetVString(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "++=")
                    {
                        if (VExists(arg2))
                        {
                            if (isNumber(arg2))
                            {
                                if (isString(arg0))
                                {
                                    int tempVarNumber = ((int)GetVNumber(arg2));
                                    string tempVarString = (GetVString(arg0));
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) + tempVarNumber));

                                    SetVString(arg0, cleaned);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                                error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (StringHelper.IsNumeric(arg2))
                            {
                                int tempVarNumber = (stoi(arg2));
                                string tempVarString = (GetVString(arg0));

                                if (tempVarString != __Null)
                                {
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) + tempVarNumber));

                                    SetVString(arg0, cleaned);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, tempVarString, false);
                            }
                            else
                                error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                    }
                    else if (arg1 == "--=")
                    {
                        if (VExists(arg2))
                        {
                            if (isNumber(arg2))
                            {
                                if (isString(arg0))
                                {
                                    int tempVarNumber = ((int)GetVNumber(arg2));
                                    string tempVarString = (GetVString(arg0));
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) - tempVarNumber));

                                    SetVString(arg0, cleaned);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                                error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (StringHelper.IsNumeric(arg2))
                            {
                                int tempVarNumber = (stoi(arg2));
                                string tempVarString = (GetVString(arg0));

                                if (tempVarString != __Null)
                                {
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) - tempVarNumber));

                                    SetVString(arg0, cleaned);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, tempVarString, false);
                            }
                            else
                                error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                    }
                    else if (arg1 == "?")
                    {
                        if (VExists(arg2))
                        {
                            if (isString(arg2))
                            {
                                if (isString(arg0))
                                    SetVString(arg0, getStdout(GetVString(arg2)));
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                                error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (isString(arg0))
                                SetVString(arg0, getStdout(cleanString(arg2)));
                            else
                                error(ErrorLogger.CONV_ERR, arg0, false);
                        }
                    }
                    else if (arg1 == "!")
                    {
                        if (VExists(arg2))
                        {
                            if (isString(arg2))
                            {
                                if (isString(arg0))
                                    SetVString(arg0, getParsedOutput(GetVString(arg2)));
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                                error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (isString(arg0))
                                SetVString(arg0, getParsedOutput(cleanString(arg2)));
                            else
                                error(ErrorLogger.CONV_ERR, arg0, false);
                        }
                    }
                    else
                    {
                        error(ErrorLogger.INVALID_OPERATOR, arg1, false);
                    }
                }
            }
        }

        void initializeListValues(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            string _b = (beforeDot(arg2)), _a = (afterDot(arg2)), __b = (beforeParameters(arg2));

            if (containsBrackets(arg0))
            {
                string after = (afterBrackets(arg0)), before = (beforeBrackets(arg0));
                after = subtractString(after, "]");

                if (GetLSize(before) >= stoi(after))
                {
                    if (stoi(after) == 0)
                    {
                        if (arg1 == "=")
                        {
                            if (VExists(arg2))
                            {
                                if (isString(arg2))
                                    LReplace(before, after, GetVString(arg2));
                                else if (isNumber(arg2))
                                    LReplace(before, after, dtos(GetVNumber(arg2)));
                                else
                                    error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else
                                LReplace(before, after, arg2);
                        }
                    }
                    else if (GetLLine(before, stoi(after))  == "#!=no_line")
                        error(ErrorLogger.OUT_OF_BOUNDS, arg0, false);
                    else
                    {
                        if (arg1 == "=")
                        {
                            if (VExists(arg2))
                            {
                                if (isString(arg2))
                                    LReplace(before, after, GetVString(arg2));
                                else if (isNumber(arg2))
                                    LReplace(before, after, dtos(GetVNumber(arg2)));
                                else
                                    error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else
                                LReplace(before, after, arg2);
                        }
                    }
                }
                else
                    error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
            }
            else if (containsBrackets(arg2)) // INITIALIZE LIST FROM RANGE
            {
                string listName = (beforeBrackets(arg2));

                if (LExists(listName))
                {
                    System.Collections.Generic.List<string> listRange = getBracketRange(arg2);

                    if (listRange.Count == 2)
                    {
                        string rangeBegin = (listRange[0]), rangeEnd = (listRange[1]);

                        if (rangeBegin.Length != 0 && rangeEnd.Length != 0)
                        {
                            if (StringHelper.IsNumeric(rangeBegin) && StringHelper.IsNumeric(rangeEnd))
                            {
                                if (stoi(rangeBegin) < stoi(rangeEnd))
                                {
                                    if (GetLSize(listName) >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                    {
                                        if (stoi(rangeBegin) >= 0)
                                        {
                                            if (arg1 == "+=")
                                            {
                                                for (int i = stoi(rangeBegin); i <= stoi(rangeEnd); i++)
                                                    LAddToList(arg0, GetLLine(listName, i));
                                            }
                                            else if (arg1 == "=")
                                            {
                                                LClear(arg0);

                                                for (int i = stoi(rangeBegin); i <= stoi(rangeEnd); i++)
                                                    LAddToList(arg0, GetLLine(listName, i));
                                            }
                                            else
                                                error(ErrorLogger.INVALID_OPERATOR, arg1, false);
                                        }
                                        else
                                            error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin, false);
                                    }
                                    else
                                        error(ErrorLogger.OUT_OF_BOUNDS, rangeEnd, false);
                                }
                                else if (stoi(rangeBegin) > stoi(rangeEnd))
                                {
                                    if (GetLSize(listName) >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                    {
                                        if (stoi(rangeBegin) >= 0)
                                        {
                                            if (arg1 == "+=")
                                            {
                                                for (int i = stoi(rangeBegin); i >= stoi(rangeEnd); i--)
                                                    LAddToList(arg0, GetLLine(listName, i));
                                            }
                                            else if (arg1 == "=")
                                            {
                                                LClear(arg0);

                                                for (int i = stoi(rangeBegin); i >= stoi(rangeEnd); i--)
                                                    LAddToList(arg0, GetLLine(listName, i));
                                            }
                                            else
                                                error(ErrorLogger.INVALID_OPERATOR, arg1, false);
                                        }
                                        else
                                            error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin, false);
                                    }
                                    else
                                        error(ErrorLogger.OUT_OF_BOUNDS, rangeEnd, false);
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
                        error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
                }
                else
                    error(ErrorLogger.LIST_UNDEFINED, listName, false);
            }
            else if (VExists(_b) && contains(_a, "split") && arg1 == "=")
            {
                if (isString(_b))
                {
                    System.Collections.Generic.List<string> parameters = getParameters(_a);
                    System.Collections.Generic.List<string> elements = new();

                    if (parameters[0] == "")
                        elements = split(GetVString(_b), ' ');
                    else
                    {
                        if (parameters[0][0] == ';')
                            elements = split(GetVString(_b), ';');
                        else
                            elements = split(GetVString(_b), parameters[0][0]);
                    }

                    LClear(arg0);

                    for (int i = 0; i < elements.Count; i++)
                        LAddToList(arg0, elements[i]);
                }
                else
                    error(ErrorLogger.NULL_STRING, _b, false);
            }
            else if (containsParameters(arg2)) // ADD/REMOVE ARRAY FROM LIST
            {
                System.Collections.Generic.List<string> parameters = getParameters(arg2);

                if (arg1 == "=")
                {
                    LClear(arg0);

                    setList(arg0, arg2, parameters);
                }
                else if (arg1 == "+=")
                    setList(arg0, arg2, parameters);
                else if (arg1 == "-=")
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        if (VExists(parameters[i]))
                        {
                            if (isString(parameters[i]))
                                LRemoveFromList(arg0, GetVString(parameters[i]));
                            else if (isNumber(parameters[i]))
                                LRemoveFromList(arg0, dtos(GetVNumber(parameters[i])));
                            else
                                error(ErrorLogger.IS_NULL, parameters[i], false);
                        }
                        else
                            LRemoveFromList(arg0, parameters[i]);
                    }
                }
                else
                    error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else if (VExists(arg2)) // ADD/REMOVE VARIABLE VALUE TO/FROM LIST
            {
                if (arg1 == "+=")
                {
                    if (isString(arg2))
                        LAddToList(arg0, GetVString(arg2));
                    else if (isNumber(arg2))
                        LAddToList(arg0, dtos(GetVNumber(arg2)));
                    else
                        error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else if (arg1 == "-=")
                {
                    if (isString(arg2))
                        LRemoveFromList(arg0, GetVString(arg2));
                    else if (isNumber(arg2))
                        LRemoveFromList(arg0, dtos(GetVNumber(arg2)));
                    else
                        error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                    error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else if (MExists(arg2)) // INITIALIZE LIST FROM METHOD RETURN
            {
                parse(arg2);

                System.Collections.Generic.List<string> _p = getParameters(__LastValue);

                if (arg1 == "=")
                {
                    LClear(arg0);

                    for (int i = 0; i < _p.Count; i++)
                        LAddToList(arg0, _p[i]);
                }
                else if (arg1 == "+=")
                {
                    for (int i = 0; i < _p.Count; i++)
                        LAddToList(arg0, _p[i]);
                }
                else
                    error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else // ADD/REMOVE STRING TO/FROM LIST
            {
                if (arg1 == "+=")
                {
                    if (arg2.Length != 0)
                        LAddToList(arg0, arg2);
                    else
                        error(ErrorLogger.IS_EMPTY, arg2, false);
                }
                else if (arg1 == "-=")
                {
                    if (arg2.Length != 0)
                        LRemoveFromList(arg0, arg2);
                    else
                        error(ErrorLogger.IS_EMPTY, arg2, false);
                }
            }
        }

        void createGlobalVariable(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            if (arg1 == "=")
            {
                string before = (beforeDot(arg2)), after = (afterDot(arg2));

                if (containsBrackets(arg2) && (VExists(beforeBrackets(arg2)) || LExists(beforeBrackets(arg2))))
                {
                    string beforeBracket = (beforeBrackets(arg2)), afterBracket = (afterBrackets(arg2));

                    afterBracket = subtractString(afterBracket, "]");

                    if (LExists(beforeBracket))
                    {
                        if (GetLSize(beforeBracket) >= stoi(afterBracket))
                        {
                            if (GetLLine(beforeBracket, stoi(afterBracket)) == "#!=no_line")
                                error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
                            else
                            {
                                string listValue = GetLLine(beforeBracket, stoi(afterBracket));

                                if (StringHelper.IsNumeric(listValue))
                                    CreateVNumber(arg0, stod(listValue));
                                else
                                    CreateVString(arg0, listValue);
                            }
                        }
                        else
                            error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
                    }
                    else if (VExists(beforeBracket))
                        setSubString(arg0, arg2, beforeBracket);
                    else
                        error(ErrorLogger.LIST_UNDEFINED, beforeBracket, false);
                }
                else if (LExists(before) && after == "size")
                    CreateVNumber(arg0, stod(itos(GetLSize(before))));
                else if (before == "self")
                {
                    if (OExists(__CurrentMethodObject))
                        twoSpace(arg0, arg1, (__CurrentMethodObject + "." + after), (arg0 + " " + arg1 + " " + (__CurrentMethodObject + "." + after)), command);
                    else
                        twoSpace(arg0, arg1, after, (arg0 + " " + arg1 + " " + after), command);
                }
                else if (after == "to_integer")
                {
                    if (VExists(before))
                    {
                        if (isString(before))
                            CreateVNumber(arg0, (int)GetVString(before)[0]);
                        else if (isNumber(before))
                        {
                            int i = (int)GetVNumber(before);
                            CreateVNumber(arg0, (double)i);
                        }
                        else
                            error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_double")
                {
                    if (VExists(before))
                    {
                        if (isString(before))
                            CreateVNumber(arg0, (double)GetVString(before)[0]);
                        else if (isNumber(before))
                        {
                            double i = GetVNumber(before);
                            CreateVNumber(arg0, (double)i);
                        }
                        else
                            error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_string")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVString(arg0, dtos(GetVNumber(before)));
                        else
                            error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_number")
                {
                    if (VExists(before))
                    {
                        if (isString(before))
                            CreateVNumber(arg0, stod(GetVString(before)));
                        else
                            error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (OExists(before))
                {
                    if (OMExists(before, after) && !containsParameters(after))
                    {
                        parse(arg2);

                        if (StringHelper.IsNumeric(__LastValue))
                            CreateVNumber(arg0, stod(__LastValue));
                        else
                            CreateVString(arg0, __LastValue);
                    }
                    else if (containsParameters(after))
                    {
                        if (OMExists(before, beforeParameters(after)))
                        {
                            executeTemplate(GetOM(before, beforeParameters(after)), getParameters(after));

                            if (StringHelper.IsNumeric(__LastValue))
                                CreateVNumber(arg0, stod(__LastValue));
                            else
                                CreateVString(arg0, __LastValue);
                        }
                        else
                            sysExec(s, command);
                    }
                    else if (OVExists(before, after))
                    {
                        if (GetOVString(before, after) != __Null)
                            CreateVString(arg0, GetOVString(before, after));
                        else if (GetOVNumber(before, after) != __NullNum)
                            CreateVNumber(arg0, GetOVNumber(before, after));
                        else
                            error(ErrorLogger.IS_NULL, GetOVName(before, after), false);
                    }
                }
                else if (VExists(before) && after == "read")
                {
                    if (isString(before))
                    {
                        if (System.IO.File.Exists(GetVString(before)))
                        {
                            string bigString = ("");
                            foreach (var line in System.IO.File.ReadAllLines(GetVString(before)))
                            {
                                bigString += (line + "\r\n");
                            }
                            CreateVString(arg0, bigString);
                        }
                        else
                            error(ErrorLogger.READ_FAIL, GetVString(before), false);
                    }
                    else
                        error(ErrorLogger.NULL_STRING, before, false);
                }
                else if (__DefiningObject)
                {
                    if (StringHelper.IsNumeric(arg2))
                    {
                        Variable newVariable = new(arg0, stod(arg2));

                        if (__DefiningPrivateCode)
                            newVariable.setPrivate();
                        else if (__DefiningPublicCode)
                            newVariable.setPublic();

                        CreateOV(__CurrentObject, newVariable);
                    }
                    else
                    {
                        Variable newVariable = new(arg0, arg2);

                        if (__DefiningPrivateCode)
                            newVariable.setPrivate();
                        else if (__DefiningPublicCode)
                            newVariable.setPublic();

                        CreateOV(__CurrentObject, newVariable);
                    }
                }
                else if (arg2 == "null")
                    CreateVString(arg0, arg2);
                else if (MExists(arg2))
                {
                    parse(arg2);

                    if (StringHelper.IsNumeric(__LastValue))
                        CreateVNumber(arg0, stod(__LastValue));
                    else
                        CreateVString(arg0, __LastValue);
                }
                else if (CExists(arg2))
                {
                    if (IsCNumber(arg2))
                        CreateVNumber(arg0, GetCNumber(arg2));
                    else if (IsCString(arg2))
                        CreateVString(arg0, GetCString(arg2));
                    else
                        error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else if (containsParameters(arg2))
                {
                    if (isStringStack(arg2))
                        CreateVString(arg0, getStringStack(arg2));
                    else if (stackReady(arg2))
                        CreateVNumber(arg0, getStack(arg2));
                    else if (beforeParameters(arg2) == "random")
                    {
                        if (contains(arg2, ".."))
                        {
                            System.Collections.Generic.List<string> range = getRange(arg2);
                            string s0 = (range[0]), s2 = (range[1]);

                            if (StringHelper.IsNumeric(s0) && StringHelper.IsNumeric(s2))
                            {
                                double n0 = stod(s0), n2 = stod(s2);

                                if (n0 < n2)
                                    CreateVNumber(arg0, (int)random(n0, n2));
                                else if (n0 > n2)
                                    CreateVNumber(arg0, (int)random(n2, n0));
                                else
                                    CreateVNumber(arg0, (int)random(n0, n2));
                            }
                            else if (isAlpha(s0) && isAlpha(s2))
                            {
                                if (get_alpha_num(s0[0]) < get_alpha_num(s2[0]))
                                    CreateVString(arg0, random(s0, s2));
                                else if (get_alpha_num(s0[0]) > get_alpha_num(s2[0]))
                                    CreateVString(arg0, random(s2, s0));
                                else
                                    CreateVString(arg0, random(s2, s0));
                            }
                            else if (VExists(s0) || VExists(s2))
                            {
                                if (VExists(s0))
                                {
                                    if (isNumber(s0))
                                        s0 = dtos(GetVNumber(s0));
                                    else if (isString(s0))
                                        s0 = GetVString(s0);
                                }

                                if (VExists(s2))
                                {
                                    if (isNumber(s2))
                                        s2 = dtos(GetVNumber(s2));
                                    else if (isString(s2))
                                        s2 = GetVString(s2);
                                }

                                if (StringHelper.IsNumeric(s0) && StringHelper.IsNumeric(s2))
                                {
                                    double n0 = stod(s0), n2 = stod(s2);

                                    if (n0 < n2)
                                        CreateVNumber(arg0, (int)random(n0, n2));
                                    else if (n0 > n2)
                                        CreateVNumber(arg0, (int)random(n2, n0));
                                    else
                                        CreateVNumber(arg0, (int)random(n0, n2));
                                }
                                else if (isAlpha(s0) && isAlpha(s2))
                                {
                                    if (get_alpha_num(s0[0]) < get_alpha_num(s2[0]))
                                        CreateVString(arg0, random(s0, s2));
                                    else if (get_alpha_num(s0[0]) > get_alpha_num(s2[0]))
                                        CreateVString(arg0, random(s2, s0));
                                    else
                                        CreateVString(arg0, random(s2, s0));
                                }
                            }
                            else
                                error(ErrorLogger.OUT_OF_BOUNDS, s0 + ".." + s2, false);
                        }
                        else
                            error(ErrorLogger.INVALID_RANGE_SEP, arg2, false);
                    }
                    else
                    {
                        executeTemplate(GetM(beforeParameters(arg2)), getParameters(arg2));

                        if (StringHelper.IsNumeric(__LastValue))
                            CreateVNumber(arg0, stod(__LastValue));
                        else
                            CreateVString(arg0, __LastValue);
                    }
                }
                else if (VExists(arg2))
                {
                    if (isNumber(arg2))
                        CreateVNumber(arg0, GetVNumber(arg2));
                    else if (isString(arg2))
                        CreateVString(arg0, GetVString(arg2));
                    else
                        CreateVString(arg0, __Null);
                }
                else if (arg2 == "password" || arg2 == "readline")
                {
                    string line = "";
                    if (arg2 == "password")
                    {
                        line = getSilentOutput("");

                        if (StringHelper.IsNumeric(line))
                            CreateVNumber(arg0, stod(line));
                        else
                            CreateVString(arg0, line);
                    }
                    else
                    {
                        cout = "readline: ";
                        line = Console.ReadLine();

                        if (StringHelper.IsNumeric(line))
                            CreateVNumber(arg0, stod(line));
                        else
                            CreateVString(arg0, line);
                    }
                }
                else if (arg2 == "args.size")
                    CreateVNumber(arg0, (double)__ArgumentCount);
                else if (before == "readline")
                {
                    if (VExists(after))
                    {
                        if (isString(after))
                        {
                            string line = "";
                            cout = cleanString(GetVString(after));
                            line = Console.ReadLine();

                            if (StringHelper.IsNumeric(line))
                                CreateVNumber(arg0, stod(line));
                            else
                                CreateVString(arg0, line);
                        }
                        else
                        {
                            string line = "";
                            cout = "readline: ";
                            line = Console.ReadLine();

                            if (StringHelper.IsNumeric(line))
                                CreateVNumber(arg0, stod(line));
                            else
                                CreateVString(arg0, line);
                        }
                    }
                    else
                    {
                        string line = "";
                        cout = cleanString(after);
                        line = Console.ReadLine();

                        if (StringHelper.IsNumeric(line))
                            CreateVNumber(arg0, stod(line));
                        else
                            CreateVString(arg0, line);
                    }
                }
                else if (before == "password")
                {
                    if (VExists(after))
                    {
                        if (isString(after))
                        {
                            string line = "";
                            line = getSilentOutput(GetVString(after));

                            if (StringHelper.IsNumeric(line))
                                CreateVNumber(arg0, stod(line));
                            else
                                CreateVString(arg0, line);

                            cout = System.Environment.NewLine;
                        }
                        else
                        {
                            string line = "";
                            line = getSilentOutput("password: ");

                            if (StringHelper.IsNumeric(line))
                                CreateVNumber(arg0, stod(line));
                            else
                                CreateVString(arg0, line);

                            cout = System.Environment.NewLine;
                        }
                    }
                    else
                    {
                        string line = "";
                        line = getSilentOutput(cleanString(after));

                        if (StringHelper.IsNumeric(line))
                            CreateVNumber(arg0, stod(line));
                        else
                            CreateVString(arg0, line);

                        cout = System.Environment.NewLine;
                    }
                }
                else if (after == "size")
                {
                    if (VExists(before))
                    {
                        if (isString(before))
                            CreateVNumber(arg0, (double)GetVString(before).Length);
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        CreateVNumber(arg0, (double)before.Length);
                }
                else if (after == "sin")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Sin(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "sinh")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Sinh(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "asin")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Asin(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "tan")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Tan(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "tanh")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Tanh(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "atan")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Atan(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "cos")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Cos(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "acos")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Acos(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "cosh")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Cosh(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "log")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Log(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "sqrt")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Sqrt(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "abs")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Abs(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "floor")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Floor(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "ceil")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Ceiling(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "exp")
                {
                    if (VExists(before))
                    {
                        if (isNumber(before))
                            CreateVNumber(arg0, System.Math.Exp(GetVNumber(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_upper")
                {
                    if (VExists(before))
                    {
                        if (isString(before))
                            CreateVString(arg0, getUpper(GetVString(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_lower")
                {
                    if (VExists(before))
                    {
                        if (isString(before))
                            CreateVString(arg0, getLower(GetVString(before)));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "bytes")
                {
                    if (VExists(before))
                    {
                        if (isString(before))
                        {
                            if (System.IO.File.Exists(GetVString(before)))
                                CreateVNumber(arg0, getBytes(GetVString(before)));
                            else
                                error(ErrorLogger.READ_FAIL, GetVString(before), false);
                        }
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                    {
                        if (System.IO.File.Exists(before))
                            CreateVNumber(arg0, getBytes(before));
                        else
                            error(ErrorLogger.READ_FAIL, before, false);
                    }
                }
                else if (before == "env")
                {
                    InternalGetEnv(arg0, after, 0);
                }
                else
                {
                    if (StringHelper.IsNumeric(arg2))
                        CreateVNumber(arg0, stod(arg2));
                    else
                        CreateVString(arg0, cleanString(arg2));
                }
            }
            else if (arg1 == "+=")
            {
                if (VExists(arg2))
                {
                    if (isString(arg2))
                        CreateVString(arg0, GetVString(arg2));
                    else if (isNumber(arg2))
                        CreateVNumber(arg0, GetVNumber(arg2));
                    else
                        CreateVString(arg0, __Null);
                }
                else
                {
                    if (StringHelper.IsNumeric(arg2))
                        CreateVNumber(arg0, stod(arg2));
                    else
                        CreateVString(arg0, cleanString(arg2));
                }
            }
            else if (arg1 == "-=")
            {
                if (VExists(arg2))
                {
                    if (isNumber(arg2))
                        CreateVNumber(arg0, 0 - GetVNumber(arg2));
                    else
                        CreateVString(arg0, __Null);
                }
                else
                {
                    if (StringHelper.IsNumeric(arg2))
                        CreateVNumber(arg0, stod(arg2));
                    else
                        CreateVString(arg0, cleanString(arg2));
                }
            }
            else if (arg1 == "?")
            {
                if (VExists(arg2))
                {
                    if (isString(arg2))
                        CreateVString(arg0, getStdout(GetVString(arg2)));
                    else
                        error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                    CreateVString(arg0, getStdout(cleanString(arg2)));
            }
            else if (arg1 == "!")
            {
                if (VExists(arg2))
                {
                    if (isString(arg2))
                        CreateVString(arg0, getParsedOutput(GetVString(arg2)));
                    else
                        error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                    CreateVString(arg0, getParsedOutput(cleanString(arg2)));
            }
            else
                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
        }

        void createObjectVariable(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            string before = beforeDot(arg2),
                   after = afterDot(arg2);

            if (OExists(before))
            {
                if (arg1 == "=")
                {
                    if (GetOVString(before, after) != __Null)
                        CreateVString(arg0, GetOVString(before, after));
                    else if (GetOVNumber(before, after) != __NullNum)
                        CreateVNumber(arg0, GetOVNumber(before, after));
                }
            }
        }

        void createConstant(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            if (!CExists(arg0))
            {
                if (arg1 == "=")
                {
                    if (StringHelper.IsNumeric(arg2))
                    {
                        Constant newConstant = new(arg0, stod(arg2));
                        constants.Add(arg0, newConstant);
                    }
                    else
                    {
                        Constant newConstant = new(arg0, arg2);
                        constants.Add(arg0, newConstant);
                    }
                }
                else
                    error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else
                error(ErrorLogger.CONST_UNDEFINED, arg0, false);
        }

        void executeSimpleStatement(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            if (StringHelper.IsNumeric(arg0) && StringHelper.IsNumeric(arg2))
            {
                if (arg1 == "+")
                    writeline(dtos(stod(arg0) + stod(arg2)));
                else if (arg1 == "-")
                    writeline(dtos(stod(arg0) - stod(arg2)));
                else if (arg1 == "*")
                    writeline(dtos(stod(arg0) * stod(arg2)));
                else if (arg1 == "/")
                    writeline(dtos(stod(arg0) / stod(arg2)));
                else if (arg1 == "**")
                    writeline(dtos(System.Math.Pow(stod(arg0), stod(arg2))));
                else if (arg1 == "%")
                {
                    if ((int)stod(arg2) == 0)
                        error(ErrorLogger.DIVIDED_BY_ZERO, s, false);
                    else
                        writeline(dtos((int)stod(arg0) % (int)stod(arg2)));
                }
                else
                    error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else
            {
                if (arg1 == "+")
                    writeline(arg0 + arg2);
                else if (arg1 == "-")
                    writeline(subtractString(arg0, arg2));
                else if (arg1 == "*")
                {
                    if (!zeroNumbers(arg2))
                    {
                        string bigstr = string.Empty;
                        for (int i = 0; i < stoi(arg2); i++)
                        {
                            bigstr += (arg0);
                            write(arg0);
                        }

                        setLastValue(bigstr);
                    }
                    else
                        error(ErrorLogger.INVALID_OP, s, false);
                }
                else if (arg1 == "/")
                    writeline(subtractString(arg0, arg2));
                else
                    error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
        }

        void InternalEncryptDecrypt(string arg0, string arg1)
        {
            Crypt c = new();
            string text = VExists(arg1) ? (isString(arg1) ? GetVString(arg1) : dtos(GetVNumber(arg1))) : arg1;
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
            char c = start[0] == sc[0] ? sc[0] : (char)((new System.Random().Next() % get_alpha_num(sc[0])) + start[0]);
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

            if (contains(System.Environment.GetEnvironmentVariable("HOMEPATH"), "Users"))
            {
                __GuessedOS = "win64";
                __SavedVarsPath = (System.Environment.GetEnvironmentVariable("HOMEPATH") + "\\AppData") + "\\.__SavedVarsPath";
                __SavedVars = __SavedVarsPath + "\\.__SavedVars";
            }
            else if (contains(System.Environment.GetEnvironmentVariable("HOMEPATH"), "Documents"))
            {
                __GuessedOS = "win32";
                __SavedVarsPath = System.Environment.GetEnvironmentVariable("HOMEPATH") + "\\Application Data\\.__SavedVarsPath";
                __SavedVars = __SavedVarsPath + "\\.__SavedVars";
            }
            else if (startsWith(System.Environment.GetEnvironmentVariable("HOME"), "/"))
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
