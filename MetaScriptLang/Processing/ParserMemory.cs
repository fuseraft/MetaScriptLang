namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Logging;

    public partial class Parser
    {

        #region Memory Stuff
        void setLastValue(string s)
        {
            __LastValue = s;
        }

        void setList(string arg1, string arg2, System.Collections.Generic.List<string> parameters)
        {
            if (MExists(beforeParameters(arg2)))
            {
                executeTemplate(GetM(beforeParameters(arg2)), parameters);

                if (containsParameters(__LastValue))
                {
                    System.Collections.Generic.List<string> last_parameters = getParameters(__LastValue);

                    for (int i = 0; i < last_parameters.Count; i++)
                        LAddToList(arg1, last_parameters[i]);
                }
                else
                    LAddToList(arg1, __LastValue);
            }
            else if (OExists(beforeDot(beforeParameters(arg2))))
            {
                executeTemplate(GetOM(beforeDot(beforeParameters(arg2)), afterDot(beforeParameters(arg2))), parameters);

                if (containsParameters(__LastValue))
                {
                    System.Collections.Generic.List<string> last_parameters = getParameters(__LastValue);

                    for (int i = 0; i < (int)last_parameters.Count; i++)
                        LAddToList(arg1, last_parameters[i]);
                }
                else
                    LAddToList(arg1, __LastValue);
            }
            else
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (VExists(parameters[i]))
                    {
                        if (isString(parameters[i]))
                            LAddToList(arg1, GetVString(parameters[i]));
                        else if (isNumber(parameters[i]))
                            LAddToList(arg1, dtos(GetVNumber(parameters[i])));
                        else
                            error(ErrorLogger.IS_NULL, parameters[i], false);
                    }
                    else
                        LAddToList(arg1, parameters[i]);
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
            if (zeroDots(s))
                return (true);
            else
            {
                string before = beforeDot(s);

                if (OExists(before))
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
            if (VExists(target))
            {
                if (System.IO.File.Exists(GetVString(target)) || System.IO.Directory.Exists(GetVString(target)))
                {
                    string old_name = (GetVString(target)), new_name = string.Empty;

                    if (VExists(name))
                    {
                        if (isString(name))
                        {
                            new_name = GetVString(name);

                            if (System.IO.File.Exists(old_name))
                            {
                                if (!System.IO.File.Exists(new_name))
                                {
                                    if (System.IO.File.Exists(old_name))
                                        rename(old_name, new_name);
                                    else
                                        error(ErrorLogger.FILE_NOT_FOUND, old_name, false);
                                }
                                else
                                    error(ErrorLogger.FILE_EXISTS, new_name, false);
                            }
                            else if (System.IO.Directory.Exists(old_name))
                            {
                                if (!System.IO.Directory.Exists(new_name))
                                {
                                    if (System.IO.Directory.Exists(old_name))
                                        rename(old_name, new_name);
                                    else
                                        error(ErrorLogger.DIR_NOT_FOUND, old_name, false);
                                }
                                else
                                    error(ErrorLogger.DIR_EXISTS, new_name, false);
                            }
                            else
                                error(ErrorLogger.TARGET_UNDEFINED, old_name, false);
                        }
                        else
                            error(ErrorLogger.NULL_STRING, name, false);
                    }
                    else
                    {
                        if (System.IO.File.Exists(old_name))
                        {
                            if (!System.IO.File.Exists(name))
                                rename(old_name, name);
                            else
                                error(ErrorLogger.FILE_EXISTS, name, false);
                        }
                        else if (System.IO.Directory.Exists(old_name))
                        {
                            if (!System.IO.Directory.Exists(name))
                                rename(old_name, name);
                            else
                                error(ErrorLogger.DIR_EXISTS, name, false);
                        }
                        else
                            error(ErrorLogger.TARGET_UNDEFINED, old_name, false);
                    }
                }
                else
                {
                    if (startsWith(name, "@"))
                    {
                        if (!VExists(name))
                            SetVName(target, name);
                        else
                            error(ErrorLogger.VAR_DEFINED, name, false);
                    }
                    else
                        error(ErrorLogger.INVALID_VAR_DECL, name, false);
                }
            }
            else if (LExists(target))
            {
                if (!LExists(name))
                    SetLName(target, name);
                else
                    error(ErrorLogger.LIST_UNDEFINED, name, false);
            }
            else if (OExists(target))
            {
                if (!OExists(name))
                    SetOName(target, name);
                else
                    error(ErrorLogger.OBJ_METHOD_UNDEFINED, name, false);
            }
            else if (MExists(target))
            {
                if (!MExists(name))
                    SetMName(target, name);
                else
                    error(ErrorLogger.METHOD_UNDEFINED, name, false);
            }
            else if (System.IO.File.Exists(target) || System.IO.Directory.Exists(target))
                rename(target, name);
            else
                error(ErrorLogger.TARGET_UNDEFINED, target, false);
        }
        #endregion
    }
}
