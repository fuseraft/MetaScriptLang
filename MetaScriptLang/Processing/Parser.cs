namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        void whileLoop(Method m)
        {
            for (int i = 0; i < m.size(); i++)
            {
                if (m.at(i) == "leave!")
                    __Breaking = true;
                else
                    parse(m.at(i));
            }
        }

        void initializeVariable(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            string tmpObjName = beforeDot(arg0), tmpVarName = afterDot(arg0);
            bool tmpObjExists = objectExists(tmpObjName);
            if (tmpObjExists || startsWith(arg0, "@"))
            {
                if (tmpObjExists)
                {
                    if (objects[indexOfObject(tmpObjName)].getVariable(tmpVarName).getString() != __Null)
                    {
                        string tempObjectVariableName = ("@ " + tmpObjName + tmpVarName + "_string");

                        createVariable(tempObjectVariableName, objects[indexOfObject(tmpObjName)].getVariable(tmpVarName).getString());

                        twoSpace(tempObjectVariableName, arg1, arg2, "", command);

                        variables[indexOfVariable(tempObjectVariableName)].setName(tmpVarName);

                        objects[indexOfObject(tmpObjName)].removeVariable(tmpVarName);
                        objects[indexOfObject(tmpObjName)].addVariable(variables[indexOfVariable(tmpVarName)]);
                        variables = removeVariable(variables, tmpVarName);
                    }
                    else if (objects[indexOfObject(tmpObjName)].getVariable(tmpVarName).getNumber() != __NullNum)
                    {
                        string tempObjectVariableName = ("@____" + beforeDot(arg0) + "___" + afterDot(arg0) + "_number");

                        createVariable(tempObjectVariableName, objects[indexOfObject(beforeDot(arg0))].getVariable(afterDot(arg0)).getNumber());

                        twoSpace(tempObjectVariableName, arg1, arg2, tempObjectVariableName + " " + arg1 + " " + arg2, command);

                        variables[indexOfVariable(tempObjectVariableName)].setName(afterDot(arg0));

                        objects[indexOfObject(beforeDot(arg0))].removeVariable(afterDot(arg0));
                        objects[indexOfObject(beforeDot(arg0))].addVariable(variables[indexOfVariable(afterDot(arg0))]);
                        variables = removeVariable(variables, afterDot(arg0));
                    }
                }
                else if (arg1 == "=")
                {
                    string before = (beforeDot(arg2)), after = (afterDot(arg2));

                    if (containsBrackets(arg2) && (variableExists(beforeBrackets(arg2)) || listExists(beforeBrackets(arg2))))
                    {
                        string beforeBracket = (beforeBrackets(arg2)), afterBracket = (afterBrackets(arg2));

                        afterBracket = subtractString(afterBracket, "]");

                        if (listExists(beforeBracket))
                        {
                            if (lists[indexOfList(beforeBracket)].size() >= stoi(afterBracket))
                            {
                                if (lists[indexOfList(beforeBracket)].at(stoi(afterBracket)) == "#!=no_line")
                                    error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
                                else
                                {
                                    string listValue = (lists[indexOfList(beforeBracket)].at(stoi(afterBracket)));

                                    if (isNumeric(listValue))
                                    {
                                        if (isNumber(arg0))
                                            setVariable(arg0, stod(listValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                    {
                                        if (isString(arg0))
                                            setVariable(arg0, listValue);
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

                                    if (isNumeric(s0) && isNumeric(s2))
                                    {
                                        if (isNumber(arg0))
                                        {
                                            double n0 = stod(s0), n2 = stod(s2);

                                            if (n0 < n2)
                                                setVariable(arg0, (int)random(n0, n2));
                                            else if (n0 > n2)
                                                setVariable(arg0, (int)random(n2, n0));
                                            else
                                                setVariable(arg0, (int)random(n0, n2));
                                        }
                                        else if (isString(arg0))
                                        {
                                            double n0 = stod(s0), n2 = stod(s2);

                                            if (n0 < n2)
                                                setVariable(arg0, itos((int)random(n0, n2)));
                                            else if (n0 > n2)
                                                setVariable(arg0, itos((int)random(n2, n0)));
                                            else
                                                setVariable(arg0, itos((int)random(n0, n2)));
                                        }
                                    }
                                    else if (isAlpha(s0) && isAlpha(s2))
                                    {
                                        if (isString(arg0))
                                        {
                                            if (get_alpha_num(s0[0]) < get_alpha_num(s2[0]))
                                                setVariable(arg0, random(s0, s2));
                                            else if (get_alpha_num(s0[0]) > get_alpha_num(s2[0]))
                                                setVariable(arg0, random(s2, s0));
                                            else
                                                setVariable(arg0, random(s2, s0));
                                        }
                                        else
                                            error(ErrorLogger.NULL_STRING, arg0, false);
                                    }
                                    else if (variableExists(s0) || variableExists(s2))
                                    {
                                        if (variableExists(s0))
                                        {
                                            if (isNumber(s0))
                                                s0 = dtos(variables[indexOfVariable(s0)].getNumber());
                                            else if (isString(s0))
                                                s0 = variables[indexOfVariable(s0)].getString();
                                        }

                                        if (variableExists(s2))
                                        {
                                            if (isNumber(s2))
                                                s2 = dtos(variables[indexOfVariable(s2)].getNumber());
                                            else if (isString(s2))
                                                s2 = variables[indexOfVariable(s2)].getString();
                                        }

                                        if (isNumeric(s0) && isNumeric(s2))
                                        {
                                            if (isNumber(arg0))
                                            {
                                                double n0 = stod(s0), n2 = stod(s2);

                                                if (n0 < n2)
                                                    setVariable(arg0, (int)random(n0, n2));
                                                else if (n0 > n2)
                                                    setVariable(arg0, (int)random(n2, n0));
                                                else
                                                    setVariable(arg0, (int)random(n0, n2));
                                            }
                                            else if (isString(arg0))
                                            {
                                                double n0 = stod(s0), n2 = stod(s2);

                                                if (n0 < n2)
                                                    setVariable(arg0, itos((int)random(n0, n2)));
                                                else if (n0 > n2)
                                                    setVariable(arg0, itos((int)random(n2, n0)));
                                                else
                                                    setVariable(arg0, itos((int)random(n0, n2)));
                                            }
                                        }
                                        else if (isAlpha(s0) && isAlpha(s2))
                                        {
                                            if (isString(arg0))
                                            {
                                                if (get_alpha_num(s0[0]) < get_alpha_num(s2[0]))
                                                    setVariable(arg0, random(s0, s2));
                                                else if (get_alpha_num(s0[0]) > get_alpha_num(s2[0]))
                                                    setVariable(arg0, random(s2, s0));
                                                else
                                                    setVariable(arg0, random(s2, s0));
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
                        else if (listExists(before) && after == "size")
                        {
                            if (isNumber(arg0))
                                setVariable(arg0, stod(itos(lists[indexOfList(before)].size())));
                            else if (isString(arg0))
                                setVariable(arg0, itos(lists[indexOfList(before)].size()));
                            else
                                error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else if (before == "self")
                        {
                            if (objectExists(__CurrentMethodObject))
                                twoSpace(arg0, arg1, (__CurrentMethodObject + "." + after), (arg0 + " " + arg1 + " " + (__CurrentMethodObject + "." + after)), command);
                            else
                                twoSpace(arg0, arg1, after, (arg0 + " " + arg1 + " " + after), command);
                        }
                        else if (objectExists(before))
                        {
                            if (objects[indexOfObject(before)].variableExists(after))
                            {
                                if (objects[indexOfObject(before)].getVariable(after).getString() != __Null)
                                    setVariable(arg0, objects[indexOfObject(before)].getVariable(after).getString());
                                else if (objects[indexOfObject(before)].getVariable(after).getNumber() != __NullNum)
                                    setVariable(arg0, objects[indexOfObject(before)].getVariable(after).getNumber());
                                else
                                    error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else if (objects[indexOfObject(before)].methodExists(after) && !containsParameters(after))
                            {
                                parse(arg2);

                                if (isString(arg0))
                                    setVariable(arg0, __LastValue);
                                else if (isNumber(arg0))
                                    setVariable(arg0, stod(__LastValue));
                            }
                            else if (containsParameters(after))
                            {
                                if (objects[indexOfObject(before)].methodExists(beforeParameters(after)))
                                {
                                    executeTemplate(objects[indexOfObject(before)].getMethod(beforeParameters(after)), getParameters(after));

                                    if (isNumeric(__LastValue))
                                    {
                                        if (isString(arg0))
                                            setVariable(arg0, __LastValue);
                                        else if (isNumber(arg0))
                                            setVariable(arg0, stod(__LastValue));
                                        else
                                            error(ErrorLogger.IS_NULL, arg0, false);
                                    }
                                    else
                                    {
                                        if (isString(arg0))
                                            setVariable(arg0, __LastValue);
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
                            if (variableExists(before))
                            {
                                if (isString(before))
                                    setVariable(arg0, (int)variables[indexOfVariable(before)].getString()[0]);
                                else if (isNumber(before))
                                {
                                    int i = (int)variables[indexOfVariable(before)].getNumber();
                                    setVariable(arg0, (double)i);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (after == "to_double")
                        {
                            if (variableExists(before))
                            {
                                if (isString(before))
                                    setVariable(arg0, (double)variables[indexOfVariable(before)].getString()[0]);
                                else if (isNumber(before))
                                {
                                    double i = variables[indexOfVariable(before)].getNumber();
                                    setVariable(arg0, (double)i);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (after == "to_string")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(before))
                                    setVariable(arg0, dtos(variables[indexOfVariable(before)].getNumber()));
                                else
                                    error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (after == "to_number")
                        {
                            if (variableExists(before))
                            {
                                if (isString(before))
                                    setVariable(arg0, stod(variables[indexOfVariable(before)].getString()));
                                else
                                    error(ErrorLogger.IS_NULL, before, false);
                            }
                            else
                                error(ErrorLogger.VAR_UNDEFINED, before, false);
                        }
                        else if (before == "readline")
                        {
                            if (variableExists(after))
                            {
                                if (isString(after))
                                {
                                    string line = "";
                                    write(cleanString(variables[indexOfVariable(after)].getString()));
                                    line = Console.ReadLine();

                                    if (isNumber(arg0))
                                    {
                                        if (isNumeric(line))
                                            setVariable(arg0, stod(line));
                                        else
                                            error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (isString(arg0))
                                        setVariable(arg0, line);
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
                                        if (isNumeric(line))
                                            setVariable(arg0, stod(line));
                                        else
                                            error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (isString(arg0))
                                        setVariable(arg0, line);
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);
                                }
                            }
                            else
                            {
                                string line = "";
                                cout = cleanString(after);
                                line = Console.ReadLine();

                                if (isNumeric(line))
                                    setVariable(arg0, stod(line));
                                else
                                    setVariable(arg0, line);
                            }
                        }
                        else if (before == "password")
                        {
                            if (variableExists(after))
                            {
                                if (isString(after))
                                {
                                    string line = "";
                                    line = getSilentOutput(variables[indexOfVariable(after)].getString());

                                    if (isNumber(arg0))
                                    {
                                        if (isNumeric(line))
                                            setVariable(arg0, stod(line));
                                        else
                                            error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (isString(arg0))
                                        setVariable(arg0, line);
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
                                        if (isNumeric(line))
                                            setVariable(arg0, stod(line));
                                        else
                                            error(ErrorLogger.CONV_ERR, line, false);
                                    }
                                    else if (isString(arg0))
                                        setVariable(arg0, line);
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);

                                    cout = System.Environment.NewLine;
                                }
                            }
                            else
                            {
                                string line = ("");
                                line = getSilentOutput(cleanString(after));

                                if (isNumeric(line))
                                    setVariable(arg0, stod(line));
                                else
                                    setVariable(arg0, line);

                                cout = System.Environment.NewLine;
                            }
                        }
                        else if (after == "cos")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Cos(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Cos(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "acos")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Acos(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Acos(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "cosh")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Cosh(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Cosh(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "log")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Log(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Log(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "sqrt")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Sqrt(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Sqrt(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "abs")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Abs(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Abs(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "floor")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Floor(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Floor(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "ceil")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Ceiling(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Ceiling(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "exp")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Exp(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Exp(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "sin")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Sin(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Sin(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "sinh")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Sinh(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Sinh(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "asin")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Asin(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Asin(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "tan")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Tan(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Tan(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "tanh")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Tanh(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Tanh(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "atan")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, System.Math.Atan(variables[indexOfVariable(before)].getNumber()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else if (isString(arg0))
                                {
                                    if (isNumber(before))
                                        setVariable(arg0, dtos(System.Math.Atan(variables[indexOfVariable(before)].getNumber())));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "to_lower")
                        {
                            if (variableExists(before))
                            {
                                if (isString(arg0))
                                {
                                    if (isString(before))
                                        setVariable(arg0, getLower(variables[indexOfVariable(before)].getString()));
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
                                if (variableExists(before))
                                {
                                    if (isString(before))
                                    {
                                        if (System.IO.File.Exists(variables[indexOfVariable(before)].getString()))
                                        {
                                            string bigString = "";
                                            foreach (var line in System.IO.File.ReadAllLines(variables[indexOfVariable(before)].getString()))
                                            {
                                                bigString += line + System.Environment.NewLine;
                                            }
                                            setVariable(arg0, bigString);
                                        }
                                        else
                                            error(ErrorLogger.READ_FAIL, variables[indexOfVariable(before)].getString(), false);
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
                                        setVariable(arg0, bigString);
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
                            if (variableExists(before))
                            {
                                if (isString(arg0))
                                {
                                    if (isString(before))
                                        setVariable(arg0, getUpper(variables[indexOfVariable(before)].getString()));
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else if (after == "size")
                        {
                            if (variableExists(before))
                            {
                                if (isNumber(arg0))
                                {
                                    if (isString(before))
                                        setVariable(arg0, (double)variables[indexOfVariable(before)].getString().Length);
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                            {
                                if (isNumber(arg0))
                                    setVariable(arg0, (double)before.Length);
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                        }
                        else if (after == "bytes")
                        {
                            if (isNumber(arg0))
                            {
                                if (variableExists(before))
                                {
                                    if (isString(before))
                                    {
                                        if (System.IO.File.Exists(variables[indexOfVariable(before)].getString()))
                                            setVariable(arg0, getBytes(variables[indexOfVariable(before)].getString()));
                                        else
                                            error(ErrorLogger.READ_FAIL, variables[indexOfVariable(before)].getString(), false);
                                    }
                                    else
                                        error(ErrorLogger.CONV_ERR, before, false);
                                }
                                else
                                {
                                    if (System.IO.File.Exists(before))
                                        setVariable(arg0, getBytes(before));
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
                                if (isNumeric(arg2))
                                    setVariable(arg0, stod(arg2));
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else if (isString(arg0))
                                setVariable(arg0, arg2);
                            else if (variables[indexOfVariable(arg0)].waiting())
                            {
                                if (isNumeric(arg2))
                                    setVariable(arg0, stod(before + "." + after));
                                else
                                    setVariable(arg0, arg2);
                            }
                            else
                                error(ErrorLogger.IS_NULL, arg0, false);
                        }
                    }
                    else
                    {
                        if (variables[indexOfVariable(arg0)].waiting())
                        {
                            if (isNumeric(arg2))
                                setVariable(arg0, stod(arg2));
                            else
                                setVariable(arg0, arg2);
                        }
                        else if (arg2 == "null")
                        {
                            if (isString(arg0))
                                variables[indexOfVariable(arg0)].setNull();
                            else if (isNumber(arg0))
                                variables[indexOfVariable(arg0)].setNull();
                            else
                                error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else if (constantExists(arg2))
                        {
                            if (isString(arg0))
                            {
                                if (constants[indexOfConstant(arg2)].ConstNumber())
                                    setVariable(arg0, dtos(constants[indexOfConstant(arg2)].getNumber()));
                                else if (constants[indexOfConstant(arg2)].ConstString())
                                    setVariable(arg0, constants[indexOfConstant(arg2)].getString());
                            }
                            else if (isNumber(arg0))
                            {
                                if (constants[indexOfConstant(arg2)].ConstNumber())
                                    setVariable(arg0, constants[indexOfConstant(arg2)].getNumber());
                                else
                                    error(ErrorLogger.CONV_ERR, arg2, false);
                            }
                            else
                                error(ErrorLogger.IS_NULL, arg0, false);
                        }
                        else if (methodExists(arg2))
                        {
                            parse(arg2);

                            if (isString(arg0))
                                setVariable(arg0, __LastValue);
                            else if (isNumber(arg0))
                                setVariable(arg0, stod(__LastValue));
                        }
                        else if (variableExists(arg2))
                        {
                            if (isString(arg2))
                            {
                                if (isString(arg0))
                                    setVariable(arg0, variables[indexOfVariable(arg2)].getString());
                                else if (isNumber(arg0))
                                    error(ErrorLogger.CONV_ERR, arg2, false);
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else if (isNumber(arg2))
                            {
                                if (isString(arg0))
                                    setVariable(arg0, dtos(variables[indexOfVariable(arg2)].getNumber()));
                                else if (isNumber(arg0))
                                    setVariable(arg0, variables[indexOfVariable(arg2)].getNumber());
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
                                    if (isNumeric(passworder))
                                        setVariable(arg0, stod(passworder));
                                    else
                                        error(ErrorLogger.CONV_ERR, passworder, false);
                                }
                                else if (isString(arg0))
                                    setVariable(arg0, passworder);
                                else
                                    setVariable(arg0, passworder);
                            }
                            else
                            {
                                string line = "";
                                cout = "readline: ";
                                line = Console.ReadLine();

                                if (isNumeric(line))
                                    createVariable(arg0, stod(line));
                                else
                                    createVariable(arg0, line);
                            }
                        }
                        else if (containsParameters(arg2))
                        {
                            if (methodExists(beforeParameters(arg2)))
                            {
                                // execute the method
                                executeTemplate(getMethod(beforeParameters(arg2)), getParameters(arg2));
                                // set the variable = last value
                                if (isString(arg0))
                                {
                                    setVariable(arg0, __LastValue);
                                }
                                else if (isNumber(arg0))
                                {
                                    setVariable(arg0, stod(__LastValue));
                                }
                            }
                            else if (isStringStack(arg2))
                            {
                                if (isString(arg0))
                                    setVariable(arg0, getStringStack(arg2));
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else if (stackReady(arg2))
                            {
                                if (isString(arg0))
                                    setVariable(arg0, dtos(getStack(arg2)));
                                else if (isNumber(arg0))
                                    setVariable(arg0, getStack(arg2));
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                        else
                        {
                            if (isNumeric(arg2))
                            {
                                if (isNumber(arg0))
                                    setVariable(arg0, stod(arg2));
                                else if (isString(arg0))
                                    setVariable(arg0, arg2);
                            }
                            else
                            {
                                if (isNumber(arg0))
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                                else if (isString(arg0))
                                    setVariable(arg0, cleanString(arg2));
                            }
                        }
                    }
                }
                else
                {
                    if (arg1 == "+=")
                    {
                        if (variableExists(arg2))
                        {
                            if (isString(arg0))
                            {
                                if (isString(arg2))
                                    setVariable(arg0, variables[indexOfVariable(arg0)].getString() + variables[indexOfVariable(arg2)].getString());
                                else if (isNumber(arg2))
                                    setVariable(arg0, variables[indexOfVariable(arg0)].getString() + dtos(variables[indexOfVariable(arg2)].getNumber()));
                                else
                                    error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else if (isNumber(arg0))
                            {
                                if (isString(arg2))
                                    error(ErrorLogger.CONV_ERR, arg2, false);
                                else if (isNumber(arg2))
                                    setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() + variables[indexOfVariable(arg2)].getNumber());
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
                                        setVariable(arg0, variables[indexOfVariable(arg0)].getString() + getStringStack(arg2));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else if (stackReady(arg2))
                                {
                                    if (isNumber(arg0))
                                        setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() + getStack(arg2));
                                }
                                else if (methodExists(beforeParameters(arg2)))
                                {
                                    executeTemplate(getMethod(beforeParameters(arg2)), getParameters(arg2));

                                    if (isString(arg0))
                                        setVariable(arg0, variables[indexOfVariable(arg0)].getString() + __LastValue);
                                    else if (isNumber(arg0))
                                    {
                                        if (isNumeric(__LastValue))
                                            setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() + stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);
                                }
                                else if (objectExists(beforeDot(arg2)))
                                {
                                    executeTemplate(getMethod(beforeParameters(arg2)), getParameters(arg2));

                                    if (isString(arg0))
                                        setVariable(arg0, variables[indexOfVariable(arg0)].getString() + __LastValue);
                                    else if (isNumber(arg0))
                                    {
                                        if (isNumeric(__LastValue))
                                            setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() + stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);
                                }
                            }
                            else if (methodExists(arg2))
                            {
                                parse(arg2);

                                if (isString(arg0))
                                    setVariable(arg0, variables[indexOfVariable(arg0)].getString() + __LastValue);
                                else if (isNumber(arg0))
                                {
                                    if (isNumeric(__LastValue))
                                        setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() + stod(__LastValue));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else if (isNumeric(arg2))
                            {
                                if (isString(arg0))
                                    setVariable(arg0, variables[indexOfVariable(arg0)].getString() + arg2);
                                else if (isNumber(arg0))
                                    setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() + stod(arg2));
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                            {
                                if (isString(arg0))
                                    setVariable(arg0, variables[indexOfVariable(arg0)].getString() + cleanString(arg2));
                                else if (isNumber(arg0))
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                        }
                    }
                    else if (arg1 == "-=")
                    {
                        if (variableExists(arg2))
                        {
                            if (isString(arg0))
                            {
                                if (isString(arg2))
                                {
                                    if (variables[indexOfVariable(arg2)].getString().Length == 1)
                                        setVariable(arg0, subtractChar(variables[indexOfVariable(arg0)].getString(), variables[indexOfVariable(arg2)].getString()));
                                    else
                                        setVariable(arg0, subtractString(variables[indexOfVariable(arg0)].getString(), variables[indexOfVariable(arg2)].getString()));
                                }
                                else if (isNumber(arg2))
                                    setVariable(arg0, subtractString(variables[indexOfVariable(arg0)].getString(), dtos(variables[indexOfVariable(arg2)].getNumber())));
                                else
                                    error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else if (isNumber(arg0))
                            {
                                if (isString(arg2))
                                    error(ErrorLogger.CONV_ERR, arg2, false);
                                else if (isNumber(arg2))
                                    setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() - variables[indexOfVariable(arg2)].getNumber());
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
                                        setVariable(arg0, subtractString(variables[indexOfVariable(arg0)].getString(), getStringStack(arg2)));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else if (stackReady(arg2))
                                {
                                    if (isNumber(arg0))
                                        setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() - getStack(arg2));
                                }
                                else if (methodExists(beforeParameters(arg2)))
                                {
                                    executeTemplate(getMethod(beforeParameters(arg2)), getParameters(arg2));

                                    if (isString(arg0))
                                        setVariable(arg0, subtractString(variables[indexOfVariable(arg0)].getString(), __LastValue));
                                    else if (isNumber(arg0))
                                    {
                                        if (isNumeric(__LastValue))
                                            setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() - stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);
                                }
                                else if (objectExists(beforeDot(arg2)))
                                {
                                    executeTemplate(getMethod(beforeParameters(arg2)), getParameters(arg2));

                                    if (isString(arg0))
                                        setVariable(arg0, subtractString(variables[indexOfVariable(arg0)].getString(), __LastValue));
                                    else if (isNumber(arg0))
                                    {
                                        if (isNumeric(__LastValue))
                                            setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() - stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.IS_NULL, arg0, false);
                                }
                            }
                            else if (methodExists(arg2))
                            {
                                parse(arg2);

                                if (isString(arg0))
                                    setVariable(arg0, subtractString(variables[indexOfVariable(arg0)].getString(), __LastValue));
                                else if (isNumber(arg0))
                                {
                                    if (isNumeric(__LastValue))
                                        setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() - stod(__LastValue));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else if (isNumeric(arg2))
                            {
                                if (isString(arg0))
                                {
                                    if (arg2.Length == 1)
                                        setVariable(arg0, subtractChar(variables[indexOfVariable(arg0)].getString(), arg2));
                                    else
                                        setVariable(arg0, subtractString(variables[indexOfVariable(arg0)].getString(), arg2));
                                }
                                else if (isNumber(arg0))
                                    setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() - stod(arg2));
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                            {
                                if (isString(arg0))
                                {
                                    if (arg2.Length == 1)
                                        setVariable(arg0, subtractChar(variables[indexOfVariable(arg0)].getString(), arg2));
                                    else
                                        setVariable(arg0, subtractString(variables[indexOfVariable(arg0)].getString(), cleanString(arg2)));
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
                        if (variableExists(arg2))
                        {
                            if (isNumber(arg2))
                                setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() * variables[indexOfVariable(arg2)].getNumber());
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
                                        setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() * getStack(arg2));
                                }
                                else if (methodExists(beforeParameters(arg2)))
                                {
                                    executeTemplate(getMethod(beforeParameters(arg2)), getParameters(arg2));

                                    if (isNumber(arg0))
                                    {
                                        if (isNumeric(__LastValue))
                                            setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() * stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                                else if (objectExists(beforeDot(arg2)))
                                {
                                    executeTemplate(getMethod(beforeParameters(arg2)), getParameters(arg2));

                                    if (isNumber(arg0))
                                    {
                                        if (isNumeric(__LastValue))
                                            setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() * stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                            }
                            else if (methodExists(arg2))
                            {
                                parse(arg2);

                                if (isNumber(arg0))
                                {
                                    if (isNumeric(__LastValue))
                                        setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() * stod(__LastValue));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    error(ErrorLogger.NULL_NUMBER, arg0, false);
                            }
                            else if (isNumeric(arg2))
                            {
                                if (isNumber(arg0))
                                    setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() * stod(arg2));
                            }
                            else
                                setVariable(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "%=")
                    {
                        if (variableExists(arg2))
                        {
                            if (isNumber(arg2))
                                setVariable(arg0, (int)variables[indexOfVariable(arg0)].getNumber() % (int)variables[indexOfVariable(arg2)].getNumber());
                            else if (isString(arg2))
                                error(ErrorLogger.CONV_ERR, arg2, false);
                            else
                                error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else if (methodExists(arg2))
                        {
                            parse(arg2);

                            if (isNumber(arg0))
                            {
                                if (isNumeric(__LastValue))
                                    setVariable(arg0, (int)variables[indexOfVariable(arg0)].getNumber() % (int)stod(__LastValue));
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                                error(ErrorLogger.NULL_NUMBER, arg0, false);
                        }
                        else
                        {
                            if (isNumeric(arg2))
                            {
                                if (isNumber(arg0))
                                    setVariable(arg0, (int)variables[indexOfVariable(arg0)].getNumber() % (int)stod(arg2));
                            }
                            else
                                setVariable(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "**=")
                    {
                        if (variableExists(arg2))
                        {
                            if (isNumber(arg2))
                                setVariable(arg0, System.Math.Pow(variables[indexOfVariable(arg0)].getNumber(), variables[indexOfVariable(arg2)].getNumber()));
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
                                        setVariable(arg0, System.Math.Pow(variables[indexOfVariable(arg0)].getNumber(), (int)getStack(arg2)));
                                }
                                else if (methodExists(beforeParameters(arg2)))
                                {
                                    executeTemplate(getMethod(beforeParameters(arg2)), getParameters(arg2));

                                    if (isNumber(arg0))
                                    {
                                        if (isNumeric(__LastValue))
                                            setVariable(arg0, System.Math.Pow(variables[indexOfVariable(arg0)].getNumber(), (int)stod(__LastValue)));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                                else if (objectExists(beforeDot(arg2)))
                                {
                                    executeTemplate(getMethod(beforeParameters(arg2)), getParameters(arg2));

                                    if (isNumber(arg0))
                                    {
                                        if (isNumeric(__LastValue))
                                            setVariable(arg0, System.Math.Pow(variables[indexOfVariable(arg0)].getNumber(), (int)stod(__LastValue)));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                            }
                            else if (methodExists(arg2))
                            {
                                parse(arg2);

                                if (isNumber(arg0))
                                {
                                    if (isNumeric(__LastValue))
                                        setVariable(arg0, System.Math.Pow(variables[indexOfVariable(arg0)].getNumber(), (int)stod(__LastValue)));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    error(ErrorLogger.NULL_NUMBER, arg0, false);
                            }
                            else if (isNumeric(arg2))
                            {
                                if (isNumber(arg0))
                                    setVariable(arg0, System.Math.Pow(variables[indexOfVariable(arg0)].getNumber(), stod(arg2)));
                            }
                            else
                                setVariable(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "/=")
                    {
                        if (variableExists(arg2))
                        {
                            if (isNumber(arg2))
                                setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() / variables[indexOfVariable(arg2)].getNumber());
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
                                        setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() / getStack(arg2));
                                }
                                else if (methodExists(beforeParameters(arg2)))
                                {
                                    executeTemplate(getMethod(beforeParameters(arg2)), getParameters(arg2));

                                    if (isNumber(arg0))
                                    {
                                        if (isNumeric(__LastValue))
                                            setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() / stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                                else if (objectExists(beforeDot(arg2)))
                                {
                                    executeTemplate(getMethod(beforeParameters(arg2)), getParameters(arg2));

                                    if (isNumber(arg0))
                                    {
                                        if (isNumeric(__LastValue))
                                            setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() / stod(__LastValue));
                                        else
                                            error(ErrorLogger.CONV_ERR, arg0, false);
                                    }
                                    else
                                        error(ErrorLogger.NULL_NUMBER, arg0, false);
                                }
                            }
                            else if (methodExists(arg2))
                            {
                                parse(arg2);

                                if (isNumber(arg0))
                                {
                                    if (isNumeric(__LastValue))
                                        setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() / stod(__LastValue));
                                    else
                                        error(ErrorLogger.CONV_ERR, arg0, false);
                                }
                                else
                                    error(ErrorLogger.NULL_NUMBER, arg0, false);
                            }
                            else if (isNumeric(arg2))
                            {
                                if (isNumber(arg0))
                                    setVariable(arg0, variables[indexOfVariable(arg0)].getNumber() / stod(arg2));
                            }
                            else
                                setVariable(arg0, cleanString(arg2));
                        }
                    }
                    else if (arg1 == "++=")
                    {
                        if (variableExists(arg2))
                        {
                            if (isNumber(arg2))
                            {
                                if (isString(arg0))
                                {
                                    int tempVarNumber = ((int)variables[indexOfVariable(arg2)].getNumber());
                                    string tempVarString = (variables[indexOfVariable(arg0)].getString());
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) + tempVarNumber));

                                    setVariable(arg0, cleaned);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                                error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (isNumeric(arg2))
                            {
                                int tempVarNumber = (stoi(arg2));
                                string tempVarString = (variables[indexOfVariable(arg0)].getString());

                                if (tempVarString != __Null)
                                {
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) + tempVarNumber));

                                    setVariable(arg0, cleaned);
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
                        if (variableExists(arg2))
                        {
                            if (isNumber(arg2))
                            {
                                if (isString(arg0))
                                {
                                    int tempVarNumber = ((int)variables[indexOfVariable(arg2)].getNumber());
                                    string tempVarString = (variables[indexOfVariable(arg0)].getString());
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) - tempVarNumber));

                                    setVariable(arg0, cleaned);
                                }
                                else
                                    error(ErrorLogger.IS_NULL, arg0, false);
                            }
                            else
                                error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (isNumeric(arg2))
                            {
                                int tempVarNumber = (stoi(arg2));
                                string tempVarString = (variables[indexOfVariable(arg0)].getString());

                                if (tempVarString != __Null)
                                {
                                    int len = (tempVarString.Length);
                                    string cleaned = ("");

                                    for (int i = 0; i < len; i++)
                                        cleaned += ((char)(((int)tempVarString[i]) - tempVarNumber));

                                    setVariable(arg0, cleaned);
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
                        if (variableExists(arg2))
                        {
                            if (isString(arg2))
                            {
                                if (isString(arg0))
                                    setVariable(arg0, getStdout(variables[indexOfVariable(arg2)].getString()));
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                                error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (isString(arg0))
                                setVariable(arg0, getStdout(cleanString(arg2)));
                            else
                                error(ErrorLogger.CONV_ERR, arg0, false);
                        }
                    }
                    else if (arg1 == "!")
                    {
                        if (variableExists(arg2))
                        {
                            if (isString(arg2))
                            {
                                if (isString(arg0))
                                    setVariable(arg0, getParsedOutput(variables[indexOfVariable(arg2)].getString()));
                                else
                                    error(ErrorLogger.CONV_ERR, arg0, false);
                            }
                            else
                                error(ErrorLogger.CONV_ERR, arg2, false);
                        }
                        else
                        {
                            if (isString(arg0))
                                setVariable(arg0, getParsedOutput(cleanString(arg2)));
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

                if (lists[indexOfList(before)].size() >= stoi(after))
                {
                    if (stoi(after) == 0)
                    {
                        if (arg1 == "=")
                        {
                            if (variableExists(arg2))
                            {
                                if (isString(arg2))
                                    replaceElement(before, after, variables[indexOfVariable(arg2)].getString());
                                else if (isNumber(arg2))
                                    replaceElement(before, after, dtos(variables[indexOfVariable(arg2)].getNumber()));
                                else
                                    error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else
                                replaceElement(before, after, arg2);
                        }
                    }
                    else if (lists[indexOfList(before)].at(stoi(after)) == "#!=no_line")
                        error(ErrorLogger.OUT_OF_BOUNDS, arg0, false);
                    else
                    {
                        if (arg1 == "=")
                        {
                            if (variableExists(arg2))
                            {
                                if (isString(arg2))
                                    replaceElement(before, after, variables[indexOfVariable(arg2)].getString());
                                else if (isNumber(arg2))
                                    replaceElement(before, after, dtos(variables[indexOfVariable(arg2)].getNumber()));
                                else
                                    error(ErrorLogger.IS_NULL, arg2, false);
                            }
                            else
                                replaceElement(before, after, arg2);
                        }
                    }
                }
                else
                    error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
            }
            else if (containsBrackets(arg2)) // INITIALIZE LIST FROM RANGE
            {
                string listName = (beforeBrackets(arg2));

                if (listExists(listName))
                {
                    System.Collections.Generic.List<string> listRange = getBracketRange(arg2);

                    if (listRange.Count == 2)
                    {
                        string rangeBegin = (listRange[0]), rangeEnd = (listRange[1]);

                        if (rangeBegin.Length != 0 && rangeEnd.Length != 0)
                        {
                            if (isNumeric(rangeBegin) && isNumeric(rangeEnd))
                            {
                                if (stoi(rangeBegin) < stoi(rangeEnd))
                                {
                                    if (lists[indexOfList(listName)].size() >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                    {
                                        if (stoi(rangeBegin) >= 0)
                                        {
                                            if (arg1 == "+=")
                                            {
                                                for (int i = stoi(rangeBegin); i <= stoi(rangeEnd); i++)
                                                    lists[indexOfList(arg0)].add(lists[indexOfList(listName)].at(i));
                                            }
                                            else if (arg1 == "=")
                                            {
                                                lists[indexOfList(arg0)].clear();

                                                for (int i = stoi(rangeBegin); i <= stoi(rangeEnd); i++)
                                                    lists[indexOfList(arg0)].add(lists[indexOfList(listName)].at(i));
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
                                    if (lists[indexOfList(listName)].size() >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                    {
                                        if (stoi(rangeBegin) >= 0)
                                        {
                                            if (arg1 == "+=")
                                            {
                                                for (int i = stoi(rangeBegin); i >= stoi(rangeEnd); i--)
                                                    lists[indexOfList(arg0)].add(lists[indexOfList(listName)].at(i));
                                            }
                                            else if (arg1 == "=")
                                            {
                                                lists[indexOfList(arg0)].clear();

                                                for (int i = stoi(rangeBegin); i >= stoi(rangeEnd); i--)
                                                    lists[indexOfList(arg0)].add(lists[indexOfList(listName)].at(i));
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
            else if (variableExists(_b) && contains(_a, "split") && arg1 == "=")
            {
                if (isString(_b))
                {
                    System.Collections.Generic.List<string> parameters = getParameters(_a);
                    System.Collections.Generic.List<string> elements = new();

                    if (parameters[0] == "")
                        elements = split(variables[indexOfVariable(_b)].getString(), ' ');
                    else
                    {
                        if (parameters[0][0] == ';')
                            elements = split(variables[indexOfVariable(_b)].getString(), ';');
                        else
                            elements = split(variables[indexOfVariable(_b)].getString(), parameters[0][0]);
                    }

                    lists[indexOfList(arg0)].clear();

                    for (int i = 0; i < (int)elements.Count; i++)
                        lists[indexOfList(arg0)].add(elements[i]);
                }
                else
                    error(ErrorLogger.NULL_STRING, _b, false);
            }
            else if (containsParameters(arg2)) // ADD/REMOVE ARRAY FROM LIST
            {
                System.Collections.Generic.List<string> parameters = getParameters(arg2);

                if (arg1 == "=")
                {
                    lists[indexOfList(arg0)].clear();

                    setList(arg0, arg2, parameters);
                }
                else if (arg1 == "+=")
                    setList(arg0, arg2, parameters);
                else if (arg1 == "-=")
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        if (variableExists(parameters[i]))
                        {
                            if (isString(parameters[i]))
                                lists[indexOfList(arg0)].remove(variables[indexOfVariable(parameters[i])].getString());
                            else if (isNumber(parameters[i]))
                                lists[indexOfList(arg0)].remove(dtos(variables[indexOfVariable(parameters[i])].getNumber()));
                            else
                                error(ErrorLogger.IS_NULL, parameters[i], false);
                        }
                        else
                            lists[indexOfList(arg0)].remove(parameters[i]);
                    }
                }
                else
                    error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else if (variableExists(arg2)) // ADD/REMOVE VARIABLE VALUE TO/FROM LIST
            {
                if (arg1 == "+=")
                {
                    if (isString(arg2))
                        lists[indexOfList(arg0)].add(variables[indexOfVariable(arg2)].getString());
                    else if (isNumber(arg2))
                        lists[indexOfList(arg0)].add(dtos(variables[indexOfVariable(arg2)].getNumber()));
                    else
                        error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else if (arg1 == "-=")
                {
                    if (isString(arg2))
                        lists[indexOfList(arg0)].remove(variables[indexOfVariable(arg2)].getString());
                    else if (isNumber(arg2))
                        lists[indexOfList(arg0)].remove(dtos(variables[indexOfVariable(arg2)].getNumber()));
                    else
                        error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                    error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else if (methodExists(arg2)) // INITIALIZE LIST FROM METHOD RETURN
            {
                parse(arg2);

                System.Collections.Generic.List<string> _p = getParameters(__LastValue);

                if (arg1 == "=")
                {
                    lists[indexOfList(arg0)].clear();

                    for (int i = 0; i < (int)_p.Count; i++)
                        lists[indexOfList(arg0)].add(_p[i]);
                }
                else if (arg1 == "+=")
                {
                    for (int i = 0; i < (int)_p.Count; i++)
                        lists[indexOfList(arg0)].add(_p[i]);
                }
                else
                    error(ErrorLogger.INVALID_OPERATOR, arg1, false);
            }
            else // ADD/REMOVE STRING TO/FROM LIST
            {
                if (arg1 == "+=")
                {
                    if (arg2.Length != 0)
                        lists[indexOfList(arg0)].add(arg2);
                    else
                        error(ErrorLogger.IS_EMPTY, arg2, false);
                }
                else if (arg1 == "-=")
                {
                    if (arg2.Length != 0)
                        lists[indexOfList(arg0)].remove(arg2);
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

                if (containsBrackets(arg2) && (variableExists(beforeBrackets(arg2)) || listExists(beforeBrackets(arg2))))
                {
                    string beforeBracket = (beforeBrackets(arg2)), afterBracket = (afterBrackets(arg2));

                    afterBracket = subtractString(afterBracket, "]");

                    if (listExists(beforeBracket))
                    {
                        if (lists[indexOfList(beforeBracket)].size() >= stoi(afterBracket))
                        {
                            if (lists[indexOfList(beforeBracket)].at(stoi(afterBracket)) == "#!=no_line")
                                error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
                            else
                            {
                                string listValue = (lists[indexOfList(beforeBracket)].at(stoi(afterBracket)));

                                if (isNumeric(listValue))
                                    createVariable(arg0, stod(listValue));
                                else
                                    createVariable(arg0, listValue);
                            }
                        }
                        else
                            error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
                    }
                    else if (variableExists(beforeBracket))
                        setSubString(arg0, arg2, beforeBracket);
                    else
                        error(ErrorLogger.LIST_UNDEFINED, beforeBracket, false);
                }
                else if (listExists(before) && after == "size")
                    createVariable(arg0, stod(itos(lists[indexOfList(before)].size())));
                else if (before == "self")
                {
                    if (objectExists(__CurrentMethodObject))
                        twoSpace(arg0, arg1, (__CurrentMethodObject + "." + after), (arg0 + " " + arg1 + " " + (__CurrentMethodObject + "." + after)), command);
                    else
                        twoSpace(arg0, arg1, after, (arg0 + " " + arg1 + " " + after), command);
                }
                else if (after == "to_integer")
                {
                    if (variableExists(before))
                    {
                        if (isString(before))
                            createVariable(arg0, (int)variables[indexOfVariable(before)].getString()[0]);
                        else if (isNumber(before))
                        {
                            int i = (int)variables[indexOfVariable(before)].getNumber();
                            createVariable(arg0, (double)i);
                        }
                        else
                            error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_double")
                {
                    if (variableExists(before))
                    {
                        if (isString(before))
                            createVariable(arg0, (double)variables[indexOfVariable(before)].getString()[0]);
                        else if (isNumber(before))
                        {
                            double i = variables[indexOfVariable(before)].getNumber();
                            createVariable(arg0, (double)i);
                        }
                        else
                            error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_string")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, dtos(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_number")
                {
                    if (variableExists(before))
                    {
                        if (isString(before))
                            createVariable(arg0, stod(variables[indexOfVariable(before)].getString()));
                        else
                            error(ErrorLogger.IS_NULL, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (objectExists(before))
                {
                    if (objects[indexOfObject(before)].methodExists(after) && !containsParameters(after))
                    {
                        parse(arg2);

                        if (isNumeric(__LastValue))
                            createVariable(arg0, stod(__LastValue));
                        else
                            createVariable(arg0, __LastValue);
                    }
                    else if (containsParameters(after))
                    {
                        if (objects[indexOfObject(before)].methodExists(beforeParameters(after)))
                        {
                            executeTemplate(objects[indexOfObject(before)].getMethod(beforeParameters(after)), getParameters(after));

                            if (isNumeric(__LastValue))
                                createVariable(arg0, stod(__LastValue));
                            else
                                createVariable(arg0, __LastValue);
                        }
                        else
                            sysExec(s, command);
                    }
                    else if (objects[indexOfObject(before)].variableExists(after))
                    {
                        if (objects[indexOfObject(before)].getVariable(after).getString() != __Null)
                            createVariable(arg0, objects[indexOfObject(before)].getVariable(after).getString());
                        else if (objects[indexOfObject(before)].getVariable(after).getNumber() != __NullNum)
                            createVariable(arg0, objects[indexOfObject(before)].getVariable(after).getNumber());
                        else
                            error(ErrorLogger.IS_NULL, objects[indexOfObject(before)].getVariable(after).name(), false);
                    }
                }
                else if (variableExists(before) && after == "read")
                {
                    if (isString(before))
                    {
                        if (System.IO.File.Exists(variables[indexOfVariable(before)].getString()))
                        {
                            string bigString = ("");
                            foreach (var line in System.IO.File.ReadAllLines(variables[indexOfVariable(before)].getString()))
                            {
                                bigString += (line + "\r\n");
                            }
                            createVariable(arg0, bigString);
                        }
                        else
                            error(ErrorLogger.READ_FAIL, variables[indexOfVariable(before)].getString(), false);
                    }
                    else
                        error(ErrorLogger.NULL_STRING, before, false);
                }
                else if (__DefiningObject)
                {
                    if (isNumeric(arg2))
                    {
                        Variable newVariable = new(arg0, stod(arg2));

                        if (__DefiningPrivateCode)
                            newVariable.setPrivate();
                        else if (__DefiningPublicCode)
                            newVariable.setPublic();

                        objects[indexOfObject(__CurrentObject)].addVariable(newVariable);
                    }
                    else
                    {
                        Variable newVariable = new(arg0, arg2);

                        if (__DefiningPrivateCode)
                            newVariable.setPrivate();
                        else if (__DefiningPublicCode)
                            newVariable.setPublic();

                        objects[indexOfObject(__CurrentObject)].addVariable(newVariable);
                    }
                }
                else if (arg2 == "null")
                    createVariable(arg0, arg2);
                else if (methodExists(arg2))
                {
                    parse(arg2);

                    if (isNumeric(__LastValue))
                        createVariable(arg0, stod(__LastValue));
                    else
                        createVariable(arg0, __LastValue);
                }
                else if (constantExists(arg2))
                {
                    if (constants[indexOfConstant(arg2)].ConstNumber())
                        createVariable(arg0, constants[indexOfConstant(arg2)].getNumber());
                    else if (constants[indexOfConstant(arg2)].ConstString())
                        createVariable(arg0, constants[indexOfConstant(arg2)].getString());
                    else
                        error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else if (containsParameters(arg2))
                {
                    if (isStringStack(arg2))
                        createVariable(arg0, getStringStack(arg2));
                    else if (stackReady(arg2))
                        createVariable(arg0, getStack(arg2));
                    else if (beforeParameters(arg2) == "random")
                    {
                        if (contains(arg2, ".."))
                        {
                            System.Collections.Generic.List<string> range = getRange(arg2);
                            string s0 = (range[0]), s2 = (range[1]);

                            if (isNumeric(s0) && isNumeric(s2))
                            {
                                double n0 = stod(s0), n2 = stod(s2);

                                if (n0 < n2)
                                    createVariable(arg0, (int)random(n0, n2));
                                else if (n0 > n2)
                                    createVariable(arg0, (int)random(n2, n0));
                                else
                                    createVariable(arg0, (int)random(n0, n2));
                            }
                            else if (isAlpha(s0) && isAlpha(s2))
                            {
                                if (get_alpha_num(s0[0]) < get_alpha_num(s2[0]))
                                    createVariable(arg0, random(s0, s2));
                                else if (get_alpha_num(s0[0]) > get_alpha_num(s2[0]))
                                    createVariable(arg0, random(s2, s0));
                                else
                                    createVariable(arg0, random(s2, s0));
                            }
                            else if (variableExists(s0) || variableExists(s2))
                            {
                                if (variableExists(s0))
                                {
                                    if (isNumber(s0))
                                        s0 = dtos(variables[indexOfVariable(s0)].getNumber());
                                    else if (isString(s0))
                                        s0 = variables[indexOfVariable(s0)].getString();
                                }

                                if (variableExists(s2))
                                {
                                    if (isNumber(s2))
                                        s2 = dtos(variables[indexOfVariable(s2)].getNumber());
                                    else if (isString(s2))
                                        s2 = variables[indexOfVariable(s2)].getString();
                                }

                                if (isNumeric(s0) && isNumeric(s2))
                                {
                                    double n0 = stod(s0), n2 = stod(s2);

                                    if (n0 < n2)
                                        createVariable(arg0, (int)random(n0, n2));
                                    else if (n0 > n2)
                                        createVariable(arg0, (int)random(n2, n0));
                                    else
                                        createVariable(arg0, (int)random(n0, n2));
                                }
                                else if (isAlpha(s0) && isAlpha(s2))
                                {
                                    if (get_alpha_num(s0[0]) < get_alpha_num(s2[0]))
                                        createVariable(arg0, random(s0, s2));
                                    else if (get_alpha_num(s0[0]) > get_alpha_num(s2[0]))
                                        createVariable(arg0, random(s2, s0));
                                    else
                                        createVariable(arg0, random(s2, s0));
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
                        executeTemplate(getMethod(beforeParameters(arg2)), getParameters(arg2));

                        if (isNumeric(__LastValue))
                            createVariable(arg0, stod(__LastValue));
                        else
                            createVariable(arg0, __LastValue);
                    }
                }
                else if (variableExists(arg2))
                {
                    if (isNumber(arg2))
                        createVariable(arg0, variables[indexOfVariable(arg2)].getNumber());
                    else if (isString(arg2))
                        createVariable(arg0, variables[indexOfVariable(arg2)].getString());
                    else
                        createVariable(arg0, __Null);
                }
                else if (arg2 == "password" || arg2 == "readline")
                {
                    string line = "";
                    if (arg2 == "password")
                    {
                        line = getSilentOutput("");

                        if (isNumeric(line))
                            createVariable(arg0, stod(line));
                        else
                            createVariable(arg0, line);
                    }
                    else
                    {
                        cout = "readline: ";
                        line = Console.ReadLine();

                        if (isNumeric(line))
                            createVariable(arg0, stod(line));
                        else
                            createVariable(arg0, line);
                    }
                }
                else if (arg2 == "args.size")
                    createVariable(arg0, (double)__ArgumentCount);
                else if (before == "readline")
                {
                    if (variableExists(after))
                    {
                        if (isString(after))
                        {
                            string line = "";
                            cout = cleanString(variables[indexOfVariable(after)].getString());
                            line = Console.ReadLine();

                            if (isNumeric(line))
                                createVariable(arg0, stod(line));
                            else
                                createVariable(arg0, line);
                        }
                        else
                        {
                            string line = "";
                            cout = "readline: ";
                            line = Console.ReadLine();

                            if (isNumeric(line))
                                createVariable(arg0, stod(line));
                            else
                                createVariable(arg0, line);
                        }
                    }
                    else
                    {
                        string line = "";
                        cout = cleanString(after);
                        line = Console.ReadLine();

                        if (isNumeric(line))
                            createVariable(arg0, stod(line));
                        else
                            createVariable(arg0, line);
                    }
                }
                else if (before == "password")
                {
                    if (variableExists(after))
                    {
                        if (isString(after))
                        {
                            string line = "";
                            line = getSilentOutput(variables[indexOfVariable(after)].getString());

                            if (isNumeric(line))
                                createVariable(arg0, stod(line));
                            else
                                createVariable(arg0, line);

                            cout = System.Environment.NewLine;
                        }
                        else
                        {
                            string line = "";
                            line = getSilentOutput("password: ");

                            if (isNumeric(line))
                                createVariable(arg0, stod(line));
                            else
                                createVariable(arg0, line);

                            cout = System.Environment.NewLine;
                        }
                    }
                    else
                    {
                        string line = "";
                        line = getSilentOutput(cleanString(after));

                        if (isNumeric(line))
                            createVariable(arg0, stod(line));
                        else
                            createVariable(arg0, line);

                        cout = System.Environment.NewLine;
                    }
                }
                else if (after == "size")
                {
                    if (variableExists(before))
                    {
                        if (isString(before))
                            createVariable(arg0, (double)variables[indexOfVariable(before)].getString().Length);
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        createVariable(arg0, (double)before.Length);
                }
                else if (after == "sin")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Sin(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "sinh")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Sinh(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "asin")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Asin(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "tan")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Tan(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "tanh")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Tanh(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "atan")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Atan(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "cos")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Cos(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "acos")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Acos(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "cosh")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Cosh(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "log")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Log(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "sqrt")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Sqrt(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "abs")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Abs(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "floor")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Floor(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "ceil")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Ceiling(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "exp")
                {
                    if (variableExists(before))
                    {
                        if (isNumber(before))
                            createVariable(arg0, System.Math.Exp(variables[indexOfVariable(before)].getNumber()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_upper")
                {
                    if (variableExists(before))
                    {
                        if (isString(before))
                            createVariable(arg0, getUpper(variables[indexOfVariable(before)].getString()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "to_lower")
                {
                    if (variableExists(before))
                    {
                        if (isString(before))
                            createVariable(arg0, getLower(variables[indexOfVariable(before)].getString()));
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                        error(ErrorLogger.VAR_UNDEFINED, before, false);
                }
                else if (after == "bytes")
                {
                    if (variableExists(before))
                    {
                        if (isString(before))
                        {
                            if (System.IO.File.Exists(variables[indexOfVariable(before)].getString()))
                                createVariable(arg0, getBytes(variables[indexOfVariable(before)].getString()));
                            else
                                error(ErrorLogger.READ_FAIL, variables[indexOfVariable(before)].getString(), false);
                        }
                        else
                            error(ErrorLogger.CONV_ERR, before, false);
                    }
                    else
                    {
                        if (System.IO.File.Exists(before))
                            createVariable(arg0, getBytes(before));
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
                    if (isNumeric(arg2))
                        createVariable(arg0, stod(arg2));
                    else
                        createVariable(arg0, cleanString(arg2));
                }
            }
            else if (arg1 == "+=")
            {
                if (variableExists(arg2))
                {
                    if (isString(arg2))
                        createVariable(arg0, variables[indexOfVariable(arg2)].getString());
                    else if (isNumber(arg2))
                        createVariable(arg0, variables[indexOfVariable(arg2)].getNumber());
                    else
                        createVariable(arg0, __Null);
                }
                else
                {
                    if (isNumeric(arg2))
                        createVariable(arg0, stod(arg2));
                    else
                        createVariable(arg0, cleanString(arg2));
                }
            }
            else if (arg1 == "-=")
            {
                if (variableExists(arg2))
                {
                    if (isNumber(arg2))
                        createVariable(arg0, 0 - variables[indexOfVariable(arg2)].getNumber());
                    else
                        createVariable(arg0, __Null);
                }
                else
                {
                    if (isNumeric(arg2))
                        createVariable(arg0, stod(arg2));
                    else
                        createVariable(arg0, cleanString(arg2));
                }
            }
            else if (arg1 == "?")
            {
                if (variableExists(arg2))
                {
                    if (isString(arg2))
                        createVariable(arg0, getStdout(variables[indexOfVariable(arg2)].getString()));
                    else
                        error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                    createVariable(arg0, getStdout(cleanString(arg2)));
            }
            else if (arg1 == "!")
            {
                if (variableExists(arg2))
                {
                    if (isString(arg2))
                        createVariable(arg0, getParsedOutput(variables[indexOfVariable(arg2)].getString()));
                    else
                        error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                    createVariable(arg0, getParsedOutput(cleanString(arg2)));
            }
            else
                error(ErrorLogger.INVALID_OPERATOR, arg2, false);
        }

        void createObjectVariable(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            string before = beforeDot(arg2),
                   after = afterDot(arg2);

            if (objectExists(before))
            {
                if (arg1 == "=")
                {
                    if (objects[indexOfObject(before)].getVariable(after).getString() != __Null)
                        createVariable(arg0, objects[indexOfObject(before)].getVariable(after).getString());
                    else if (objects[indexOfObject(before)].getVariable(after).getNumber() != __NullNum)
                        createVariable(arg0, objects[indexOfObject(before)].getVariable(after).getNumber());
                }
            }
        }

        void copyObject(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            if (arg1 == "=")
            {
                System.Collections.Generic.List<Method> objectMethods = objects[indexOfObject(arg2)].getMethods();
                Object newObject = new(arg0);

                for (int i = 0; i < (int)objectMethods.Count; i++)
                    newObject.addMethod(objectMethods[i]);


                System.Collections.Generic.List<Variable> objectVariables = objects[indexOfObject(arg2)].getVariables();

                for (int i = 0; i < (int)objectVariables.Count; i++)
                    newObject.addVariable(objectVariables[i]);

                if (__ExecutedMethod)
                    newObject.collect();
                else
                    newObject.dontCollect();

                objects.Add(newObject);
                __CurrentObject = arg1;
                __DefiningObject = false;

                newObject.clear();
                objectMethods.Clear();
            }
            else
                error(ErrorLogger.INVALID_OPERATOR, arg1, false);
        }

        void createConstant(string arg0, string arg1, string arg2, string s, System.Collections.Generic.List<string> command)
        {
            if (!constantExists(arg0))
            {
                if (arg1 == "=")
                {
                    if (isNumeric(arg2))
                    {
                        Constant newConstant = new(arg0, stod(arg2));
                        constants.Add(newConstant);
                    }
                    else
                    {
                        Constant newConstant = new(arg0, arg2);
                        constants.Add(newConstant);
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
            if (isNumeric(arg0) && isNumeric(arg2))
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
            string text = variableExists(arg1) ? (isString(arg1) ? getVariable(arg1).getString() : dtos(getVariable(arg1).getNumber())) : arg1;
            write(arg0 == "encrypt" ? c.e(text) : c.d(text));
        }

        void InternalInspect(string arg0, string arg1, string before, string after)
        {
            if (before.Length != 0 && after.Length != 0)
            {
                if (objectExists(before))
                {
                    if (objects[indexOfObject(before)].methodExists(after))
                    {
                        for (int i = 0; i < objects[indexOfObject(before)].getMethod(after).size(); i++)
                            write(objects[indexOfObject(before)].getMethod(after).at(i));
                    }
                    else if (objects[indexOfObject(before)].variableExists(after))
                    {
                        if (objects[indexOfObject(before)].getVariable(after).getString() != __Null)
                            write(objects[indexOfObject(before)].getVariable(after).getString());
                        else if (objects[indexOfObject(before)].getVariable(after).getNumber() != __NullNum)
                            write(dtos(objects[indexOfObject(before)].getVariable(after).getNumber()));
                        else
                            write(__Null);
                    }
                    else
                        error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                    error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, false);
            }
            else
            {
                if (objectExists(arg1))
                {
                    for (int i = 0; i < objects[indexOfObject(arg1)].methodSize(); i++)
                        write(objects[indexOfObject(arg1)].getMethod(objects[indexOfObject(arg1)].getMethodName(i)).name());
                    for (int i = 0; i < objects[indexOfObject(arg1)].variableSize(); i++)
                        write(objects[indexOfObject(arg1)].getVariable(objects[indexOfObject(arg1)].getVariableName(i)).name());
                }
                else if (constantExists(arg1))
                {
                    if (constants[indexOfConstant(arg1)].ConstNumber())
                        write(dtos(constants[indexOfConstant(arg1)].getNumber()));
                    else if (constants[indexOfConstant(arg1)].ConstString())
                        write(constants[indexOfConstant(arg1)].getString());
                }
                else if (methodExists(arg1))
                {
                    for (int i = 0; i < methods[indexOfMethod(arg1)].size(); i++)
                        write(methods[indexOfMethod(arg1)].at(i));
                }
                else if (variableExists(arg1))
                {
                    if (isString(arg1))
                        write(variables[indexOfVariable(arg1)].getString());
                    else if (isNumber(arg1))
                        write(dtos(variables[indexOfVariable(arg1)].getNumber()));
                }
                else if (listExists(arg1))
                {
                    for (int i = 0; i < lists[indexOfList(arg1)].size(); i++)
                        write(lists[indexOfList(arg1)].at(i));
                }
                else if (arg1 == "variables")
                {
                    for (int i = 0; i < (int)variables.Count; i++)
                    {
                        if (variables[i].getString() != __Null)
                            write(variables[i].name() + ":\t" + variables[i].getString());
                        else if (variables[i].getNumber() != __NullNum)
                            write(variables[i].name() + ":\t" + dtos(variables[i].getNumber()));
                        else
                            write(variables[i].name() + ":\tis_null");
                    }
                }
                else if (arg1 == "lists")
                {
                    for (int i = 0; i < (int)lists.Count; i++)
                        write(lists[i].name());
                }
                else if (arg1 == "methods")
                {
                    for (int i = 0; i < (int)methods.Count; i++)
                        write(methods[i].name());
                }
                else if (arg1 == "objects")
                {
                    for (int i = 0; i < (int)objects.Count; i++)
                        write(objects[i].name());
                }
                else if (arg1 == "constants")
                {
                    for (int i = 0; i < (int)constants.Count; i++)
                        write(constants[i].name());
                }
                else if (arg1 == "os?")
                    write("windows?");
                else if (arg1 == "last")
                    write(__LastValue);
                else
                    error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
            }
        }

        void InternalGlobalize(string arg0, string arg1)
        {
            if (contains(arg1, ".") && methodExists(arg1) && !methodExists(afterDot(arg1)))
            {
                Method method = new(afterDot(arg1));

                System.Collections.Generic.List<string> lines = getObject(beforeDot(arg1)).getMethod(afterDot(arg1)).getLines();

                for (int i = 0; i < (int)lines.Count; i++)
                    method.add(lines[i]);

                methods.Add(method);
            }
            else
                error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1, false);
        }

        void InternalCallMethod(string arg0, string arg1, string before, string after)
        {
            if (__DefiningObject)
            {
                if (objects[indexOfObject(__CurrentObject)].methodExists(arg1))
                    executeMethod(objects[indexOfObject(__CurrentObject)].getMethod(arg1));
                else
                    error(ErrorLogger.METHOD_UNDEFINED, arg1, false);
            }
            else
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (objectExists(before))
                    {
                        if (objects[indexOfObject(before)].methodExists(after))
                            executeMethod(objects[indexOfObject(before)].getMethod(after));
                        else
                            error(ErrorLogger.METHOD_UNDEFINED, arg1, false);
                    }
                    else
                        error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, true);
                }
                else
                {
                    if (methodExists(arg1))
                        executeMethod(methods[indexOfMethod(arg1)]);
                    else
                        error(ErrorLogger.METHOD_UNDEFINED, arg1, true);
                }
            }
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
