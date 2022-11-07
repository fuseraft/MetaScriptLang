namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Helpers;
    using MetaScriptLang.Logging;

    public partial class Parser
    {

        #region Memory Stuff
        void SetLastValue(string s)
        {
            __LastValue = s;
        }

        void setList(string arg1, string arg2, System.Collections.Generic.List<string> parameters)
        {
            if (MethodExists(StringHelper.BeforeParameters(arg2)))
            {
                executeTemplate(GetMethod(StringHelper.BeforeParameters(arg2)), parameters);

                if (StringHelper.ContainsParameters(__LastValue))
                {
                    System.Collections.Generic.List<string> last_parameters = StringHelper.GetParameters(__LastValue);

                    for (int i = 0; i < last_parameters.Count; i++)
                        engine.AddToList(arg1, last_parameters[i]);
                }
                else
                    engine.AddToList(arg1, __LastValue);
            }
            else if (engine.ObjectExists(StringHelper.BeforeDot(StringHelper.BeforeParameters(arg2))))
            {
                executeTemplate(GetObjectMethod(StringHelper.BeforeDot(StringHelper.BeforeParameters(arg2)), StringHelper.AfterDot(StringHelper.BeforeParameters(arg2))), parameters);

                if (StringHelper.ContainsParameters(__LastValue))
                {
                    System.Collections.Generic.List<string> last_parameters = StringHelper.GetParameters(__LastValue);

                    for (int i = 0; i < (int)last_parameters.Count; i++)
                        engine.AddToList(arg1, last_parameters[i]);
                }
                else
                    engine.AddToList(arg1, __LastValue);
            }
            else
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (VariableExists(parameters[i]))
                    {
                        if (IsStringVariable(parameters[i]))
                            engine.AddToList(arg1, GetVariableString(parameters[i]));
                        else if (IsNumberVariable(parameters[i]))
                            engine.AddToList(arg1, StringHelper.DtoS(GetVariableNumber(parameters[i])));
                        else
                            ErrorLogger.Error(ErrorLogger.IS_NULL, parameters[i], false);
                    }
                    else
                        engine.AddToList(arg1, parameters[i]);
                }
            }
        }
        #endregion

        #region Existence Checking Pt 2
        bool noLists()
        {
            return lists.Count == 0;
        }

        bool noMethods()
        {
            return methods.Count == 0;
        }

        bool noObjects()
        {
            return objects.Count == 0;
        }

        bool noVariables()
        {
            return variables.Count == 0;
        }

        bool notObjectMethod(string s)
        {
            if (StringHelper.ZeroDots(s))
                return (true);
            else
            {
                string before = StringHelper.BeforeDot(s);

                if (engine.ObjectExists(before))
                    return (false);
                else
                    return (true);
            }
        }
        #endregion

        #region Removal
        //System.Collections.Generic.List<Module> removeModule(System.Collections.Generic.List<Module> v, string target)
        //System.Collections.Generic.List<Constant> removeConstant(System.Collections.Generic.List<Constant> v, string target)
        #endregion

        #region Redefinition
        void rename(string first, string second)
        {
            // rename file or directory.
            if (System.IO.Directory.Exists(first) && !System.IO.Directory.Exists(second))
            {
            }
            else if (System.IO.File.Exists(first) && !System.IO.File.Exists(second))
            {
            }
            else
            {
            }
        }

        /**
            Give a new name to anything.
        **/
        void MemRedefine(string target, string name)
        {
            if (VariableExists(target))
            {
                if (System.IO.File.Exists(GetVariableString(target)) || System.IO.Directory.Exists(GetVariableString(target)))
                {
                    string old_name = (GetVariableString(target)), new_name = string.Empty;

                    if (VariableExists(name))
                    {
                        if (IsStringVariable(name))
                        {
                            new_name = GetVariableString(name);

                            if (System.IO.File.Exists(old_name))
                            {
                                if (!System.IO.File.Exists(new_name))
                                {
                                    if (System.IO.File.Exists(old_name))
                                        rename(old_name, new_name);
                                    else
                                        ErrorLogger.Error(ErrorLogger.FILE_NOT_FOUND, old_name, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.FILE_EXISTS, new_name, false);
                            }
                            else if (System.IO.Directory.Exists(old_name))
                            {
                                if (!System.IO.Directory.Exists(new_name))
                                {
                                    if (System.IO.Directory.Exists(old_name))
                                        rename(old_name, new_name);
                                    else
                                        ErrorLogger.Error(ErrorLogger.DIR_NOT_FOUND, old_name, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.DIR_EXISTS, new_name, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, old_name, false);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.NULL_STRING, name, false);
                    }
                    else
                    {
                        if (System.IO.File.Exists(old_name))
                        {
                            if (!System.IO.File.Exists(name))
                                rename(old_name, name);
                            else
                                ErrorLogger.Error(ErrorLogger.FILE_EXISTS, name, false);
                        }
                        else if (System.IO.Directory.Exists(old_name))
                        {
                            if (!System.IO.Directory.Exists(name))
                                rename(old_name, name);
                            else
                                ErrorLogger.Error(ErrorLogger.DIR_EXISTS, name, false);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, old_name, false);
                    }
                }
                else
                {
                    if (StringHelper.StringStartsWith(name, "@"))
                    {
                        if (!VariableExists(name))
                            SetVariableName(target, name);
                        else
                            ErrorLogger.Error(ErrorLogger.VAR_DEFINED, name, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.INVALID_VAR_DECL, name, false);
                }
            }
            else if (engine.ListExists(target))
            {
                if (!engine.ListExists(name))
                    engine.SetListName(target, name);
                else
                    ErrorLogger.Error(ErrorLogger.LIST_UNDEFINED, name, false);
            }
            else if (engine.ObjectExists(target))
            {
                if (!engine.ObjectExists(name))
                    SetObjectName(target, name);
                else
                    ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, name, false);
            }
            else if (MethodExists(target))
            {
                if (!MethodExists(name))
                    SetMethodName(target, name);
                else
                    ErrorLogger.Error(ErrorLogger.METHOD_UNDEFINED, name, false);
            }
            else if (System.IO.File.Exists(target) || System.IO.Directory.Exists(target))
                rename(target, name);
            else
                ErrorLogger.Error(ErrorLogger.TARGET_UNDEFINED, target, false);
        }
        #endregion
    }
}
