using MetaScriptLang.Data;
using MetaScriptLang.Logging;
using System.Diagnostics.Metrics;
using System.Linq;

namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        void InternalInspect(string arg0, string arg1, string before, string after)
        {
            if (before.Length != 0 && after.Length != 0)
            {
                if (OExists(before))
                {
                    if (OMExists(before, after))
                    {
                        for (int i = 0; i < GetOMSize(before, after); i++)
                            write(GetOMLine(before, after, i));
                    }
                    else if (OVExists(before, after))
                    {
                        if (GetOVString(before, after) != __Null)
                            write(GetOVString(before, after));
                        else if (GetOVNumber(before, after) != __NullNum)
                            write(dtos(GetOVNumber(before, after)));
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
                if (OExists(arg1))
                {
                    for (int i = 0; i < GetOMCount(arg1); i++)
                        write(GetOMNameByIndex(arg1, i));
                    for (int i = 0; i < GetOVCount(arg1); i++)
                        write(GetOVNameByIndex(arg1, i));
                }
                else if (CExists(arg1))
                {
                    if (IsCNumber(arg1))
                        write(dtos(GetCNumber(arg1)));
                    else if (IsCString(arg1))
                        write(GetCString(arg1));
                }
                else if (MExists(arg1))
                {
                    for (int i = 0; i < GetMSize(arg1); i++)
                        write(GetMLine(arg1, i));
                }
                else if (VExists(arg1))
                {
                    if (isString(arg1))
                        write(GetVString(arg1));
                    else if (isNumber(arg1))
                        write(dtos(GetVNumber(arg1)));
                }
                else if (LExists(arg1))
                {
                    for (int i = 0; i < GetLSize(arg1); i++)
                        write(GetLLine(arg1, i));
                }
                else if (arg1 == "variables")
                {
                    foreach (var key in this.variables.Keys)
                    {
                        if (variables[key].getString() != __Null)
                            write($"{variables[key].name()}:\t{variables[key].getString()}");
                        else if (variables[key].getNumber() != __NullNum)
                            write($"{variables[key].name()}:\t{dtos(variables[key].getNumber())}");
                        else
                            write($"{variables[key].name()}:\tis_null");
                    }
                }
                else if (arg1 == "lists")
                {
                    foreach (var key in this.lists.Keys)
                    {
                        write(GetLName(key));
                    }
                }
                else if (arg1 == "methods")
                {
                    foreach (var key in this.methods.Keys)
                    {
                        write(methods[key].GetName());
                    }
                }
                else if (arg1 == "objects")
                {
                    foreach (var key in this.objects.Keys)
                    {
                        write(objects[key].name());
                    }
                }
                else if (arg1 == "constants")
                {
                    foreach (var key in this.constants.Keys)
                    {
                        write(constants[key].name());
                    }
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
            if (contains(arg1, ".") && MExists(arg1) && !MExists(afterDot(arg1)))
            {
                Method method = new(afterDot(arg1));

                System.Collections.Generic.List<string> lines = GetOM(beforeDot(arg1), afterDot(arg1)).GetLines();

                for (int i = 0; i < (int)lines.Count; i++)
                    method.AddLine(lines[i]);

                methods.Add(method.GetName(), method);
            }
            else
                error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1, false);
        }

        void InternalCallMethod(string arg0, string arg1, string before, string after)
        {
            if (__DefiningObject)
            {
                if (OMExists(__CurrentObject, arg1))
                    executeMethod(GetOM(__CurrentObject, arg1));
                else
                    error(ErrorLogger.METHOD_UNDEFINED, arg1, false);
            }
            else
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (OExists(before))
                    {
                        if (OMExists(before, after))
                            executeMethod(GetOM(before, after));
                        else
                            error(ErrorLogger.METHOD_UNDEFINED, arg1, false);
                    }
                    else
                        error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, true);
                }
                else
                {
                    if (MExists(arg1))
                        executeMethod(GetM(arg1));
                    else
                        error(ErrorLogger.METHOD_UNDEFINED, arg1, true);
                }
            }
        }


        void InternalCreateMethod(string arg0, string arg1)
        {
            bool indestructable = false;

            if (arg0 == "[method]")
                indestructable = true;

            if (__DefiningObject)
            {
                if (OMExists(__CurrentObject, arg1))
                    error(ErrorLogger.METHOD_DEFINED, arg1, false);
                else
                {
                    if (containsParameters(arg1))
                    {
                        System.Collections.Generic.List<string> parameters = getParameters(arg1);

                        Method method = new(beforeParameters(arg1));

                        if (__DefiningPublicCode)
                            method.SetPublic();
                        else if (__DefiningPrivateCode)
                            method.SetPrivate();

                        method.SetObject(__CurrentObject);

                        for (int i = 0; i < parameters.Count; i++)
                        {
                            if (VExists(parameters[i]))
                            {
                                if (!zeroDots(parameters[i]))
                                {
                                    string before = (beforeDot(parameters[i])), after = (afterDot(parameters[i]));

                                    if (OExists(before))
                                    {
                                        if (OVExists(before, after))
                                        {
                                            if (GetOVString(before, after) != __Null)
                                                method.AddVariable(GetOVString(before, after), after);
                                            else if (GetOVNumber(before, after) != __NullNum)
                                                method.AddVariable(GetOVNumber(before, after), after);
                                            else
                                                error(ErrorLogger.IS_NULL, parameters[i], false);
                                        }
                                        else
                                            error(ErrorLogger.OBJ_VAR_UNDEFINED, after, false);
                                    }
                                    else
                                        error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, false);
                                }
                                else
                                {
                                    if (isString(parameters[i]))
                                        method.AddVariable(GetVString(parameters[i]), GetVName(parameters[i]));
                                    else if (isNumber(parameters[i]))
                                        method.AddVariable(GetVNumber(parameters[i]), GetVName(parameters[i]));
                                    else
                                        error(ErrorLogger.IS_NULL, parameters[i], false);
                                }
                            }
                            else
                            {
                                if (isAlpha(parameters[i]))
                                {
                                    Variable newVariable = new("@[pm#" + itos(__ParamVarCount) + "]", parameters[i]);
                                    method.AddVariable(newVariable);
                                    __ParamVarCount++;
                                }
                                else
                                {
                                    Variable newVariable = new("@[pm#" + itos(__ParamVarCount) + "]", stod(parameters[i]));
                                    method.AddVariable(newVariable);
                                    __ParamVarCount++;
                                }
                            }
                        }

                        CreateOM(__CurrentObject, method);
                        SetOMCurrentMethod(__CurrentObject, beforeParameters(arg1));
                        __DefiningMethod = true;
                        __DefiningParameterizedMethod = true;
                        __DefiningObjectMethod = true;
                    }
                    else
                    {
                        Method method = new(arg1);

                        if (__DefiningPublicCode)
                            method.SetPublic();
                        else if (__DefiningPrivateCode)
                            method.SetPrivate();

                        method.SetObject(__CurrentObject);
                        CreateOM(__CurrentObject, method);
                        SetOMCurrentMethod(__CurrentObject, arg1);
                        __DefiningMethod = true;
                        __DefiningObjectMethod = true;
                    }
                }
            }
            else
            {
                if (MExists(arg1))
                    error(ErrorLogger.METHOD_DEFINED, arg1, false);
                else
                {
                    if (!zeroDots(arg1))
                    {
                        string before = (beforeDot(arg1)), after = (afterDot(arg1));

                        if (OExists(before))
                        {
                            Method method = new(after);

                            if (__DefiningPublicCode)
                                method.SetPublic();
                            else if (__DefiningPrivateCode)
                                method.SetPrivate();

                            method.SetObject(before);
                            CreateOM(before, method);
                            SetOMCurrentMethod(before, after);
                            __DefiningMethod = true;
                            __DefiningObjectMethod = true;
                        }
                        else
                            error(ErrorLogger.OBJ_UNDEFINED, "", false);
                    }
                    else if (containsParameters(arg1))
                    {
                        System.Collections.Generic.List<string> parameters = getParameters(arg1);

                        Method method = new(beforeParameters(arg1));

                        if (indestructable)
                            method.Lock();

                        for (int i = 0; i < parameters.Count; i++)
                        {
                            if (VExists(parameters[i]))
                            {
                                if (!zeroDots(parameters[i]))
                                {
                                    string before = beforeDot(parameters[i]), after = afterDot(parameters[i]);

                                    if (OExists(before))
                                    {
                                        if (OVExists(before, after))
                                        {
                                            if (GetOVString(before, after) != __Null)
                                                method.AddVariable(GetOVString(before, after), after);
                                            else if (GetOVNumber(before, after) != __NullNum)
                                                method.AddVariable(GetOVNumber(before, after), after);
                                            else
                                                error(ErrorLogger.IS_NULL, parameters[i], false);
                                        }
                                        else
                                            error(ErrorLogger.OBJ_VAR_UNDEFINED, after, false);
                                    }
                                    else
                                        error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, false);
                                }
                                else
                                {
                                    if (isString(parameters[i]))
                                        method.AddVariable(GetVString(parameters[i]), GetVName(parameters[i]));
                                    else if (isNumber(parameters[i]))
                                        method.AddVariable(GetVNumber(parameters[i]), GetVName(parameters[i]));
                                    else
                                        error(ErrorLogger.IS_NULL, parameters[i], false);
                                }
                            }
                            else
                            {
                                if (isAlpha(parameters[i]))
                                {
                                    Variable newVariable = new("@" + parameters[i], "");
                                    newVariable.setNull();
                                    method.AddVariable(newVariable);
                                    __ParamVarCount++;
                                }
                                else
                                {
                                    Variable newVariable = new("@" + parameters[i], 0);
                                    newVariable.setNull();
                                    method.AddVariable(newVariable);
                                    __ParamVarCount++;
                                }
                            }
                        }

                        methods.Add(method.GetName(), method);
                        __DefiningMethod = true;
                        __DefiningParameterizedMethod = true;
                    }
                    else
                    {
                        Method method = new(arg1);

                        if (indestructable)
                            method.Lock();

                        methods.Add(method.GetName(), method);
                        __DefiningMethod = true;
                    }
                }
            }
        }

        void InternalCreateModule(string s)
        {
            string moduleName = s;
            moduleName = subtractString(moduleName, "[");
            moduleName = subtractString(moduleName, "]");

            Module newModule = new(moduleName);
            modules.Add(moduleName, newModule);
            __DefiningModule = true;
            __CurrentModule = moduleName;
        }

        void InternalCreateObject(string arg0)
        {
            if (OExists(arg0))
            {
                __DefiningObject = true;
                __CurrentObject = arg0;
            }
            else
            {
                MetaScriptLang.Data.Object obj = new(arg0);
                __CurrentObject = arg0;
                obj.dontCollect();
                objects.Add(arg0, obj);
                __DefiningObject = true;
            }
        }

        void InternalForget(string arg0, string arg1)
        {
            if (System.IO.File.Exists(__SavedVars))
            {
                string bigStr = ("");
                // REFACTOR HERE
                Crypt c = new();

                bigStr += (System.IO.File.ReadAllText(__SavedVars));

                int bigStrLength = bigStr.Length;
                bool stop = false;
                string varName = string.Empty;
                bigStr = c.d(bigStr);

                System.Collections.Generic.List<string> varNames = new();
                System.Collections.Generic.List<string> varValues = new();

                varNames.Add("");
                varValues.Add("");

                for (int i = 0; i < bigStrLength; i++)
                {
                    switch (bigStr[i])
                    {
                        case '&':
                            stop = true;
                            break;

                        case '#':
                            stop = false;
                            varNames.Add("");
                            varValues.Add("");
                            break;

                        default:
                            if (!stop)
                                varNames[(int)varNames.Count - 1] += (bigStr[i]);
                            else
                                varValues[(int)varValues.Count - 1] += (bigStr[i]);
                            break;
                    }
                }

                string new_saved = ("");

                for (int i = 0; i < (int)varNames.Count; i++)
                {
                    if (varNames[i] != arg1)
                    {
                        Variable newVariable = new(varNames[i], varValues[i]);
                        variables.Add(varNames[i], newVariable);

                        if (i != (int)varNames.Count - 1)
                            new_saved += (varNames[i] + "&" + varValues[i] + "#");
                        else
                            new_saved += (varNames[i] + "&" + varValues[i]);
                    }
                }

                varNames.Clear();
                varValues.Clear();

                System.IO.File.Delete(__SavedVars);
                createFile(__SavedVars);
                app(__SavedVars, c.e(new_saved));
            }
        }

        bool InternalReturn(string arg0, string arg1, string before, string after)
        {
            __Returning = true;

            if (containsParameters(arg1))
            {
                before = (beforeParameters(arg1));

                if (MExists(before))
                {
                    executeTemplate(GetM(before), getParameters(arg1));

                    parse("return " + __LastValue);
                }
                else if (!zeroDots(arg1))
                {
                    if (OExists(before))
                    {
                        if (OMExists(before, beforeParameters(after)))
                        {
                            executeTemplate(GetOM(before, beforeParameters(after)), getParameters(arg1));
                            parse("return " + __LastValue);
                        }
                        else
                            __LastValue = arg1;
                    }
                    else
                        __LastValue = arg1;
                }
                else
                {
                    if (isStringStack(arg1))
                        __LastValue = getStringStack(arg1);
                    else if (stackReady(arg1))
                        __LastValue = dtos(getStack(arg1));
                    else
                    {
                        arg1 = subtractString(subtractString(arg1, "("), ")");
                        return true;
                    }
                }
            }
            else if (VExists(arg1))
            {
                if (OExists(beforeDot(arg1)))
                {
                    if (GetOVString(beforeDot(arg1), afterDot(arg1)) != __Null)
                        __LastValue = GetOVString(beforeDot(arg1), afterDot(arg1));
                    else if (GetOVNumber(beforeDot(arg1), afterDot(arg1)) != __NullNum)
                        __LastValue = dtos(GetOVNumber(beforeDot(arg1), afterDot(arg1)));
                    else
                        __LastValue = "null";
                }
                else
                {
                    if (isString(arg1))
                        __LastValue = GetVString(arg1);
                    else if (isNumber(arg1))
                        __LastValue = dtos(GetVNumber(arg1));
                    else
                        __LastValue = "null";

                    if (GCCanCollectV(arg1))
                        DeleteV(arg1);
                }
            }
            else if (LExists(arg1))
            {
                string bigString = "(";

                for (int i = 0; i < GetLSize(arg1); i++)
                {
                    bigString += GetLLine(arg1, i);

                    if (i != GetLSize(arg1) - 1)
                        bigString += ',';
                }

                bigString += (")");

                __LastValue = bigString;

                if (GCCanCollectL(arg1))
                    DeleteL(arg1);
            }
            else
                __LastValue = arg1;

            return false;
        }

        void InternalRemember(string arg0, string arg1)
        {
            if (VExists(arg1))
            {
                if (isString(arg1))
                    saveVariable(arg1 + "&" + GetVString(arg1));
                else if (isNumber(arg1))
                    saveVariable(arg1 + "&" + dtos(GetVNumber(arg1)));
                else
                    error(ErrorLogger.IS_NULL, arg1, false);
            }
            else
                error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
        }

        void InternalOutput(string arg0, string arg1)
        {
            string text = (arg1);
            bool is_say = (arg0 == "say");
            bool is_print = (arg0 == "print" || arg0 == "println");
            // if parameter is variable, get it's value
            if (VExists(arg1))
            {
                // set the value
                if (!zeroDots(arg1))
                {
                    if (GetOVString(beforeDot(arg1), afterDot(arg1)) != __Null)
                        text = (GetOVString(beforeDot(arg1), afterDot(arg1)));
                    else if (GetOVNumber(beforeDot(arg1), afterDot(arg1)) != __NullNum)
                        text = (dtos(GetOVNumber(beforeDot(arg1), afterDot(arg1))));
                    else
                    {
                        error(ErrorLogger.IS_NULL, arg1, false);
                        return;
                    }
                }
                else
                {
                    if (isString(arg1))
                        text = (GetVString(arg1));
                    else if (isNumber(arg1))
                        text = (dtos(GetVNumber(arg1)));
                    else
                    {
                        error(ErrorLogger.IS_NULL, arg1, false);
                        return;
                    }
                }
            }

            if (is_say)
            {
                writeline(text);
            }
            else if (is_print)
            {
                if (arg0 == "println")
                {
                    cout = text + System.Environment.NewLine;
                }
                else
                {
                    cout = text;
                }
            }
            else
            {
                write(text);
            }
        }
    }
}
