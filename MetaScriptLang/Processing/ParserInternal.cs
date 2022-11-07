﻿namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        void InternalInspect(string arg0, string arg1, string before, string after)
        {
            if (before.Length != 0 && after.Length != 0)
            {
                if (engine.ObjectExists(before))
                {
                    if (ObjectMethodExists(before, after))
                    {
                        for (int i = 0; i < GetObjectMethodSize(before, after); i++)
                            write(GetObjectMethodLine(before, after, i));
                    }
                    else if (ObjectVariableExists(before, after))
                    {
                        if (GetObjectVariableString(before, after) != __Null)
                            write(GetObjectVariableString(before, after));
                        else if (GetObjectVariableNumber(before, after) != __NullNum)
                            write(StringHelper.DtoS(GetObjectVariableNumber(before, after)));
                        else
                            write(__Null);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
                }
                else
                    ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, false);
            }
            else
            {
                if (engine.ObjectExists(arg1))
                {
                    for (int i = 0; i < GetObjectMethodCount(arg1); i++)
                        write(GetObjectMethodNameByIndex(arg1, i));
                    for (int i = 0; i < GetObjectVariableSize(arg1); i++)
                        write(GetObjectVariableNameByIndex(arg1, i));
                }
                else if (engine.ConstantExists(arg1))
                {
                    if (engine.IsNumberConstant(arg1))
                        write(StringHelper.DtoS(engine.GetConstantNumber(arg1)));
                    else if (engine.IsStringConstant(arg1))
                        write(engine.GetConstantString(arg1));
                }
                else if (MethodExists(arg1))
                {
                    for (int i = 0; i < GetMethodSize(arg1); i++)
                        write(GetMethodLine(arg1, i));
                }
                else if (VariableExists(arg1))
                {
                    if (IsStringVariable(arg1))
                        write(GetVariableString(arg1));
                    else if (IsNumberVariable(arg1))
                        write(StringHelper.DtoS(GetVariableNumber(arg1)));
                }
                else if (engine.ListExists(arg1))
                {
                    for (int i = 0; i < engine.GetListSize(arg1); i++)
                        write(engine.GetListLine(arg1, i));
                }
                else if (arg1 == "variables")
                {
                    foreach (var key in this.variables.Keys)
                    {
                        if (variables[key].getString() != __Null)
                            write($"{variables[key].name()}:\t{variables[key].getString()}");
                        else if (variables[key].getNumber() != __NullNum)
                            write($"{variables[key].name()}:\t{StringHelper.DtoS(variables[key].getNumber())}");
                        else
                            write($"{variables[key].name()}:\tis_null");
                    }
                }
                else if (arg1 == "lists")
                {
                    foreach (var key in this.lists.Keys)
                    {
                        write(engine.GetListName(key));
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
                    ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
            }
        }

        void InternalGlobalize(string arg0, string arg1)
        {
            if (StringHelper.ContainsString(arg1, ".") && MethodExists(arg1) && !MethodExists(StringHelper.AfterDot(arg1)))
            {
                Method method = new(StringHelper.AfterDot(arg1));

                System.Collections.Generic.List<string> lines = GetObjectMethod(StringHelper.BeforeDot(arg1), StringHelper.AfterDot(arg1)).GetLines();

                for (int i = 0; i < (int)lines.Count; i++)
                    method.AddLine(lines[i]);

                methods.Add(method.GetName(), method);
            }
            else
                ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, arg1, false);
        }

        void InternalCallMethod(string arg0, string arg1, string before, string after)
        {
            if (__DefiningObject)
            {
                if (ObjectMethodExists(__CurrentObject, arg1))
                    executeMethod(GetObjectMethod(__CurrentObject, arg1));
                else
                    ErrorLogger.Error(ErrorLogger.METHOD_UNDEFINED, arg1, false);
            }
            else
            {
                if (before.Length != 0 && after.Length != 0)
                {
                    if (engine.ObjectExists(before))
                    {
                        if (ObjectMethodExists(before, after))
                            executeMethod(GetObjectMethod(before, after));
                        else
                            ErrorLogger.Error(ErrorLogger.METHOD_UNDEFINED, arg1, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, true);
                }
                else
                {
                    if (MethodExists(arg1))
                        executeMethod(GetMethod(arg1));
                    else
                        ErrorLogger.Error(ErrorLogger.METHOD_UNDEFINED, arg1, true);
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
                if (ObjectMethodExists(__CurrentObject, arg1))
                    ErrorLogger.Error(ErrorLogger.METHOD_DEFINED, arg1, false);
                else
                {
                    if (StringHelper.ContainsParameters(arg1))
                    {
                        System.Collections.Generic.List<string> parameters = StringHelper.GetParameters(arg1);

                        Method method = new(StringHelper.BeforeParameters(arg1));

                        if (__DefiningPublicCode)
                            method.SetPublic();
                        else if (__DefiningPrivateCode)
                            method.SetPrivate();

                        method.SetObject(__CurrentObject);

                        for (int i = 0; i < parameters.Count; i++)
                        {
                            if (VariableExists(parameters[i]))
                            {
                                if (!StringHelper.ZeroDots(parameters[i]))
                                {
                                    string before = (StringHelper.BeforeDot(parameters[i])), after = (StringHelper.AfterDot(parameters[i]));

                                    if (engine.ObjectExists(before))
                                    {
                                        if (ObjectVariableExists(before, after))
                                        {
                                            if (GetObjectVariableString(before, after) != __Null)
                                                method.AddVariable(GetObjectVariableString(before, after), after);
                                            else if (GetObjectVariableNumber(before, after) != __NullNum)
                                                method.AddVariable(GetObjectVariableNumber(before, after), after);
                                            else
                                                ErrorLogger.Error(ErrorLogger.IS_NULL, parameters[i], false);
                                        }
                                        else
                                            ErrorLogger.Error(ErrorLogger.OBJ_VAR_UNDEFINED, after, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, false);
                                }
                                else
                                {
                                    if (IsStringVariable(parameters[i]))
                                        method.AddVariable(GetVariableString(parameters[i]), GetVariableName(parameters[i]));
                                    else if (IsNumberVariable(parameters[i]))
                                        method.AddVariable(GetVariableNumber(parameters[i]), GetVariableName(parameters[i]));
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, parameters[i], false);
                                }
                            }
                            else
                            {
                                if (StringHelper.IsAlphabetical(parameters[i]))
                                {
                                    Variable newVariable = new("@[pm#" + StringHelper.ItoS(__ParamVarCount) + "]", parameters[i]);
                                    method.AddVariable(newVariable);
                                    __ParamVarCount++;
                                }
                                else
                                {
                                    Variable newVariable = new("@[pm#" + StringHelper.ItoS(__ParamVarCount) + "]", StringHelper.StoD(parameters[i]));
                                    method.AddVariable(newVariable);
                                    __ParamVarCount++;
                                }
                            }
                        }

                        CreateObjectMethod(__CurrentObject, method);
                        SetObjectCurrentMethod(__CurrentObject, StringHelper.BeforeParameters(arg1));
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
                        CreateObjectMethod(__CurrentObject, method);
                        SetObjectCurrentMethod(__CurrentObject, arg1);
                        __DefiningMethod = true;
                        __DefiningObjectMethod = true;
                    }
                }
            }
            else
            {
                if (MethodExists(arg1))
                    ErrorLogger.Error(ErrorLogger.METHOD_DEFINED, arg1, false);
                else
                {
                    if (!StringHelper.ZeroDots(arg1))
                    {
                        string before = (StringHelper.BeforeDot(arg1)), after = (StringHelper.AfterDot(arg1));

                        if (engine.ObjectExists(before))
                        {
                            Method method = new(after);

                            if (__DefiningPublicCode)
                                method.SetPublic();
                            else if (__DefiningPrivateCode)
                                method.SetPrivate();

                            method.SetObject(before);
                            CreateObjectMethod(before, method);
                            SetObjectCurrentMethod(before, after);
                            __DefiningMethod = true;
                            __DefiningObjectMethod = true;
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.OBJ_UNDEFINED, "", false);
                    }
                    else if (StringHelper.ContainsParameters(arg1))
                    {
                        System.Collections.Generic.List<string> parameters = StringHelper.GetParameters(arg1);

                        Method method = new(StringHelper.BeforeParameters(arg1));

                        if (indestructable)
                            method.Lock();

                        for (int i = 0; i < parameters.Count; i++)
                        {
                            if (VariableExists(parameters[i]))
                            {
                                if (!StringHelper.ZeroDots(parameters[i]))
                                {
                                    string before = StringHelper.BeforeDot(parameters[i]), after = StringHelper.AfterDot(parameters[i]);

                                    if (engine.ObjectExists(before))
                                    {
                                        if (ObjectVariableExists(before, after))
                                        {
                                            if (GetObjectVariableString(before, after) != __Null)
                                                method.AddVariable(GetObjectVariableString(before, after), after);
                                            else if (GetObjectVariableNumber(before, after) != __NullNum)
                                                method.AddVariable(GetObjectVariableNumber(before, after), after);
                                            else
                                                ErrorLogger.Error(ErrorLogger.IS_NULL, parameters[i], false);
                                        }
                                        else
                                            ErrorLogger.Error(ErrorLogger.OBJ_VAR_UNDEFINED, after, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, false);
                                }
                                else
                                {
                                    if (IsStringVariable(parameters[i]))
                                        method.AddVariable(GetVariableString(parameters[i]), GetVariableName(parameters[i]));
                                    else if (IsNumberVariable(parameters[i]))
                                        method.AddVariable(GetVariableNumber(parameters[i]), GetVariableName(parameters[i]));
                                    else
                                        ErrorLogger.Error(ErrorLogger.IS_NULL, parameters[i], false);
                                }
                            }
                            else
                            {
                                if (StringHelper.IsAlphabetical(parameters[i]))
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
            moduleName = StringHelper.SubtractString(moduleName, "[");
            moduleName = StringHelper.SubtractString(moduleName, "]");

            Module newModule = new(moduleName);
            modules.Add(moduleName, newModule);
            __DefiningModule = true;
            __CurrentModule = moduleName;
        }

        void InternalCreateObject(string arg0)
        {
            if (engine.ObjectExists(arg0))
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

            if (StringHelper.ContainsParameters(arg1))
            {
                before = (StringHelper.BeforeParameters(arg1));

                if (MethodExists(before))
                {
                    executeTemplate(GetMethod(before), StringHelper.GetParameters(arg1));

                    parse("return " + __LastValue);
                }
                else if (!StringHelper.ZeroDots(arg1))
                {
                    if (engine.ObjectExists(before))
                    {
                        if (ObjectMethodExists(before, StringHelper.BeforeParameters(after)))
                        {
                            executeTemplate(GetObjectMethod(before, StringHelper.BeforeParameters(after)), StringHelper.GetParameters(arg1));
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
                        __LastValue = StringHelper.DtoS(getStack(arg1));
                    else
                    {
                        arg1 = StringHelper.SubtractString(StringHelper.SubtractString(arg1, "("), ")");
                        return true;
                    }
                }
            }
            else if (VariableExists(arg1))
            {
                if (engine.ObjectExists(StringHelper.BeforeDot(arg1)))
                {
                    if (GetObjectVariableString(StringHelper.BeforeDot(arg1), StringHelper.AfterDot(arg1)) != __Null)
                        __LastValue = GetObjectVariableString(StringHelper.BeforeDot(arg1), StringHelper.AfterDot(arg1));
                    else if (GetObjectVariableNumber(StringHelper.BeforeDot(arg1), StringHelper.AfterDot(arg1)) != __NullNum)
                        __LastValue = StringHelper.DtoS(GetObjectVariableNumber(StringHelper.BeforeDot(arg1), StringHelper.AfterDot(arg1)));
                    else
                        __LastValue = "null";
                }
                else
                {
                    if (IsStringVariable(arg1))
                        __LastValue = GetVariableString(arg1);
                    else if (IsNumberVariable(arg1))
                        __LastValue = StringHelper.DtoS(GetVariableNumber(arg1));
                    else
                        __LastValue = "null";

                    if (GCCanCollectVariable(arg1))
                        DeleteVariable(arg1);
                }
            }
            else if (engine.ListExists(arg1))
            {
                string bigString = "(";

                for (int i = 0; i < engine.GetListSize(arg1); i++)
                {
                    bigString += engine.GetListLine(arg1, i);

                    if (i != engine.GetListSize(arg1) - 1)
                        bigString += ',';
                }

                bigString += (")");

                __LastValue = bigString;

                if (engine.GCCanCollectList(arg1))
                    engine.DeleteList(arg1);
            }
            else
                __LastValue = arg1;

            return false;
        }

        void InternalRemember(string arg0, string arg1)
        {
            if (VariableExists(arg1))
            {
                if (IsStringVariable(arg1))
                    saveVariable(arg1 + "&" + GetVariableString(arg1));
                else if (IsNumberVariable(arg1))
                    saveVariable(arg1 + "&" + StringHelper.DtoS(GetVariableNumber(arg1)));
                else
                    ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
            }
            else
                ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, arg1, false);
        }

        void InternalOutput(string arg0, string arg1)
        {
            string text = (arg1);
            bool is_say = (arg0 == "say");
            bool is_print = (arg0 == "print" || arg0 == "println");
            // if parameter is variable, get it's value
            if (VariableExists(arg1))
            {
                // set the value
                if (!StringHelper.ZeroDots(arg1))
                {
                    if (GetObjectVariableString(StringHelper.BeforeDot(arg1), StringHelper.AfterDot(arg1)) != __Null)
                        text = (GetObjectVariableString(StringHelper.BeforeDot(arg1), StringHelper.AfterDot(arg1)));
                    else if (GetObjectVariableNumber(StringHelper.BeforeDot(arg1), StringHelper.AfterDot(arg1)) != __NullNum)
                        text = (StringHelper.DtoS(GetObjectVariableNumber(StringHelper.BeforeDot(arg1), StringHelper.AfterDot(arg1))));
                    else
                    {
                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
                        return;
                    }
                }
                else
                {
                    if (IsStringVariable(arg1))
                        text = (GetVariableString(arg1));
                    else if (IsNumberVariable(arg1))
                        text = (StringHelper.DtoS(GetVariableNumber(arg1)));
                    else
                    {
                        ErrorLogger.Error(ErrorLogger.IS_NULL, arg1, false);
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
