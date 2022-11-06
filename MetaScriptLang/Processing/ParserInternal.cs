using MetaScriptLang.Data;
using MetaScriptLang.Logging;
using System.Diagnostics.Metrics;
using System.Linq;

namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        void InternalCreateMethod(string arg0, string arg1)
        {
            bool indestructable = false;

            if (arg0 == "[method]")
                indestructable = true;

            if (__DefiningObject)
            {
                if (objects[indexOfObject(__CurrentObject)].methodExists(arg1))
                    error(ErrorLogger.METHOD_DEFINED, arg1, false);
                else
                {
                    if (containsParameters(arg1))
                    {
                        System.Collections.Generic.List<string> parameters = getParameters(arg1);

                        Method method = new(beforeParameters(arg1));

                        if (__DefiningPublicCode)
                            method.setPublic();
                        else if (__DefiningPrivateCode)
                            method.setPrivate();

                        method.setObject(__CurrentObject);

                        for (int i = 0; i < parameters.Count; i++)
                        {
                            if (variableExists(parameters[i]))
                            {
                                if (!zeroDots(parameters[i]))
                                {
                                    string before = (beforeDot(parameters[i])), after = (afterDot(parameters[i]));

                                    if (objectExists(before))
                                    {
                                        if (objects[indexOfObject(before)].variableExists(after))
                                        {
                                            if (objects[indexOfObject(before)].getVariable(after).getString() != __Null)
                                                method.addMethodVariable(objects[indexOfObject(before)].getVariable(after).getString(), after);
                                            else if (objects[indexOfObject(before)].getVariable(after).getNumber() != __NullNum)
                                                method.addMethodVariable(objects[indexOfObject(before)].getVariable(after).getNumber(), after);
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
                                        method.addMethodVariable(variables[indexOfVariable(parameters[i])].getString(), variables[indexOfVariable(parameters[i])].name());
                                    else if (isNumber(parameters[i]))
                                        method.addMethodVariable(variables[indexOfVariable(parameters[i])].getNumber(), variables[indexOfVariable(parameters[i])].name());
                                    else
                                        error(ErrorLogger.IS_NULL, parameters[i], false);
                                }
                            }
                            else
                            {
                                if (isAlpha(parameters[i]))
                                {
                                    Variable newVariable = new("@[pm#" + itos(__ParamVarCount) + "]", parameters[i]);
                                    method.addMethodVariable(newVariable);
                                    __ParamVarCount++;
                                }
                                else
                                {
                                    Variable newVariable = new("@[pm#" + itos(__ParamVarCount) + "]", stod(parameters[i]));
                                    method.addMethodVariable(newVariable);
                                    __ParamVarCount++;
                                }
                            }
                        }

                        objects[indexOfObject(__CurrentObject)].addMethod(method);
                        objects[indexOfObject(__CurrentObject)].setCurrentMethod(beforeParameters(arg1));
                        __DefiningMethod = true;
                        __DefiningParameterizedMethod = true;
                        __DefiningObjectMethod = true;
                    }
                    else
                    {
                        Method method = new(arg1);

                        if (__DefiningPublicCode)
                            method.setPublic();
                        else if (__DefiningPrivateCode)
                            method.setPrivate();

                        method.setObject(__CurrentObject);
                        objects[indexOfObject(__CurrentObject)].addMethod(method);
                        objects[indexOfObject(__CurrentObject)].setCurrentMethod(arg1);
                        __DefiningMethod = true;
                        __DefiningObjectMethod = true;
                    }
                }
            }
            else
            {
                if (methodExists(arg1))
                    error(ErrorLogger.METHOD_DEFINED, arg1, false);
                else
                {
                    if (!zeroDots(arg1))
                    {
                        string before = (beforeDot(arg1)), after = (afterDot(arg1));

                        if (objectExists(before))
                        {
                            Method method = new(after);

                            if (__DefiningPublicCode)
                                method.setPublic();
                            else if (__DefiningPrivateCode)
                                method.setPrivate();

                            method.setObject(before);
                            objects[indexOfObject(before)].addMethod(method);
                            objects[indexOfObject(before)].setCurrentMethod(after);
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
                            method.setIndestructible();

                        for (int i = 0; i < parameters.Count; i++)
                        {
                            if (variableExists(parameters[i]))
                            {
                                if (!zeroDots(parameters[i]))
                                {
                                    string before = (beforeDot(parameters[i])), after = (afterDot(parameters[i]));

                                    if (objectExists(before))
                                    {
                                        if (objects[indexOfObject(before)].variableExists(after))
                                        {
                                            if (objects[indexOfObject(before)].getVariable(after).getString() != __Null)
                                                method.addMethodVariable(objects[indexOfObject(before)].getVariable(after).getString(), after);
                                            else if (objects[indexOfObject(before)].getVariable(after).getNumber() != __NullNum)
                                                method.addMethodVariable(objects[indexOfObject(before)].getVariable(after).getNumber(), after);
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
                                        method.addMethodVariable(variables[indexOfVariable(parameters[i])].getString(), variables[indexOfVariable(parameters[i])].name());
                                    else if (isNumber(parameters[i]))
                                        method.addMethodVariable(variables[indexOfVariable(parameters[i])].getNumber(), variables[indexOfVariable(parameters[i])].name());
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
                                    method.addMethodVariable(newVariable);
                                    __ParamVarCount++;
                                }
                                else
                                {
                                    Variable newVariable = new("@" + parameters[i], 0);
                                    newVariable.setNull();
                                    method.addMethodVariable(newVariable);
                                    __ParamVarCount++;
                                }
                            }
                        }

                        methods.Add(method);
                        __DefiningMethod = true;
                        __DefiningParameterizedMethod = true;
                    }
                    else
                    {
                        Method method = new(arg1);

                        if (indestructable)
                            method.setIndestructible();

                        methods.Add(method);
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
            modules.Add(newModule);
            __DefiningModule = true;
            __CurrentModule = moduleName;
        }

        void InternalCreateObject(string arg0)
        {
            if (objectExists(arg0))
            {
                __DefiningObject = true;
                __CurrentObject = arg0;
            }
            else
            {
                MetaScriptLang.Data.Object obj = new(arg0);
                __CurrentObject = arg0;
                obj.dontCollect();
                objects.Add(obj);
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
                        variables.Add(newVariable);

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

                if (methodExists(before))
                {
                    executeTemplate(getMethod(before), getParameters(arg1));

                    parse("return " + __LastValue);
                }
                else if (!zeroDots(arg1))
                {
                    if (objectExists(before))
                    {
                        if (objects[indexOfObject(before)].methodExists(beforeParameters(after)))
                        {
                            executeTemplate(objects[indexOfObject(before)].getMethod(beforeParameters(after)), getParameters(arg1));
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
            else if (variableExists(arg1))
            {
                if (objectExists(beforeDot(arg1)))
                {
                    if (objects[indexOfObject(beforeDot(arg1))].getVariable(afterDot(arg1)).getString() != __Null)
                        __LastValue = objects[indexOfObject(beforeDot(arg1))].getVariable(afterDot(arg1)).getString();
                    else if (objects[indexOfObject(beforeDot(arg1))].getVariable(afterDot(arg1)).getNumber() != __NullNum)
                        __LastValue = dtos(objects[indexOfObject(beforeDot(arg1))].getVariable(afterDot(arg1)).getNumber());
                    else
                        __LastValue = "null";
                }
                else
                {
                    if (isString(arg1))
                        __LastValue = variables[indexOfVariable(arg1)].getString();
                    else if (isNumber(arg1))
                        __LastValue = dtos(variables[indexOfVariable(arg1)].getNumber());
                    else
                        __LastValue = "null";

                    if (variables[indexOfVariable(arg1)].garbage())
                        variables = removeVariable(variables, arg1);
                }
            }
            else if (listExists(arg1))
            {
                string bigString = "(";

                for (int i = 0; i < (int)lists[indexOfList(arg1)].size(); i++)
                {
                    bigString += lists[indexOfList(arg1)].at(i);

                    if (i != (int)lists[indexOfList(arg1)].size() - 1)
                        bigString += ',';
                }

                bigString += (")");

                __LastValue = bigString;

                if (lists[indexOfList(arg1)].garbage())
                    lists = removeList(lists, arg1);
            }
            else
                __LastValue = arg1;

            return false;
        }

        void InternalRemember(string arg0, string arg1)
        {
            if (variableExists(arg1))
            {
                if (isString(arg1))
                    saveVariable(arg1 + "&" + variables[indexOfVariable(arg1)].getString());
                else if (isNumber(arg1))
                    saveVariable(arg1 + "&" + dtos(variables[indexOfVariable(arg1)].getNumber()));
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
            if (variableExists(arg1))
            {
                // set the value
                if (!zeroDots(arg1))
                {
                    if (objects[indexOfObject(beforeDot(arg1))].getVariable(afterDot(arg1)).getString() != __Null)
                        text = (objects[indexOfObject(beforeDot(arg1))].getVariable(afterDot(arg1)).getString());
                    else if (objects[indexOfObject(beforeDot(arg1))].getVariable(afterDot(arg1)).getNumber() != __NullNum)
                        text = (dtos(objects[indexOfObject(beforeDot(arg1))].getVariable(afterDot(arg1)).getNumber()));
                    else
                    {
                        error(ErrorLogger.IS_NULL, arg1, false);
                        return;
                    }
                }
                else
                {
                    if (isString(arg1))
                        text = (variables[indexOfVariable(arg1)].getString());
                    else if (isNumber(arg1))
                        text = (dtos(variables[indexOfVariable(arg1)].getNumber()));
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
