namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Logging;
    using System.Xml.Linq;

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
                        lists[indexOfList(arg1)].add(last_parameters[i]);
                }
                else
                    lists[indexOfList(arg1)].add(__LastValue);
            }
            else if (objectExists(beforeDot(beforeParameters(arg2))))
            {
                executeTemplate(objects[indexOfObject(beforeDot(beforeParameters(arg2)))].getMethod(afterDot(beforeParameters(arg2))), parameters);

                if (containsParameters(__LastValue))
                {
                    System.Collections.Generic.List<string> last_parameters = getParameters(__LastValue);

                    for (int i = 0; i < (int)last_parameters.Count; i++)
                        lists[indexOfList(arg1)].add(last_parameters[i]);
                }
                else
                    lists[indexOfList(arg1)].add(__LastValue);
            }
            else
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (VExists(parameters[i]))
                    {
                        if (isString(parameters[i]))
                            lists[indexOfList(arg1)].add(GetVString(parameters[i]));
                        else if (isNumber(parameters[i]))
                            lists[indexOfList(arg1)].add(dtos(GetVNumber(parameters[i])));
                        else
                            error(ErrorLogger.IS_NULL, parameters[i], false);
                    }
                    else
                        lists[indexOfList(arg1)].add(parameters[i]);
                }
            }
        }
        #endregion

        #region Indexing
        int indexOfObject(string s)
        {
            for (int i = 0; i < (int)objects.Count; i++)
            {
                if (objects[i].name() == s)
                    return (i);
            }

            return (-1);
        }

        int indexOfList(string s)
        {
            for (int i = 0; i < (int)lists.Count; i++)
            {
                if (lists[i].name() == s)
                    return (i);
            }

            return (-1);
        }

        int indexOfModule(string s)
        {
            for (int i = 0; i < (int)modules.Count; i++)
            {
                if (modules[i].name() == s)
                    return (i);
            }

            return (-1);
        }

        int indexOfScript(string s)
        {
            for (int i = 0; i < (int)scripts.Count; i++)
            {
                if (scripts[i].name() == s)
                    return (i);
            }

            return (-1);
        }

        int indexOfConstant(string s)
        {
            for (int i = 0; i < (int)constants.Count; i++)
            {
                if (constants[i].name() == s)
                    return (i);
            }

            return (-1);
        }
        #endregion

        #region Existence Checking
        bool listExists(string s)
        {
            for (int i = 0; i < (int)lists.Count; i++)
                if (lists[i].name() == s)
                    return (true);

            return (false);
        }

        bool objectExists(string s)
        {
            for (int i = 0; i < (int)objects.Count; i++)
                if (objects[i].name() == s)
                    return (true);

            return (false);
        }

        bool moduleExists(string s)
        {
            for (int i = 0; i < (int)modules.Count; i++)
                if (modules[i].name() == s)
                    return (true);

            return (false);
        }

        bool constantExists(string s)
        {
            for (int i = 0; i < (int)constants.Count; i++)
                if (constants[i].name() == s)
                    return (true);

            return (false);
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

                if (objectExists(before))
                    return (false);
                else
                    return (true);
            }
        }
        #endregion

        #region Retrieval
        Object getObject(string s)
        {
            Object bad_obj = new("[bad_obj#" + itos(__BadObjectCount) + "]");

            if (objectExists(s))
            {
                for (int i = 0; i < (int)objects.Count; i++)
                {
                    if (objects[i].name() == s)
                    {
                        return (objects[i]);
                    }
                }
            }
            __BadObjectCount++;

            return (bad_obj);
        }
        #endregion

        #region Removal
        System.Collections.Generic.List<MetaScriptLang.Data.Object> removeObject(System.Collections.Generic.List<MetaScriptLang.Data.Object> v, string target)
        {
            System.Collections.Generic.List<MetaScriptLang.Data.Object> cleanedVector = new();

            for (int i = 0; i < (int)v.Count; i++)
                if (v[i].name() != target)
                    cleanedVector.Add(v[i]);

            return (cleanedVector);
        }

        System.Collections.Generic.List<List> removeList(System.Collections.Generic.List<List> v, string target)
        {
            System.Collections.Generic.List<List> cleanedVector = new();

            for (int i = 0; i < (int)v.Count; i++)
                if (v[i].name() != target)
                    cleanedVector.Add(v[i]);

            return (cleanedVector);
        }

        System.Collections.Generic.List<Module> removeModule(System.Collections.Generic.List<Module> v, string target)
        {
            System.Collections.Generic.List<Module> cleanedVector = new();

            for (int i = 0; i < (int)v.Count; i++)
                if (v[i].name() != target)
                    cleanedVector.Add(v[i]);

            return (cleanedVector);
        }

        System.Collections.Generic.List<Constant> removeConstant(System.Collections.Generic.List<Constant> v, string target)
        {
            System.Collections.Generic.List<Constant> cleanedVector = new();

            for (int i = 0; i < (int)v.Count; i++)
                if (v[i].name() != target)
                    cleanedVector.Add(v[i]);

            return (cleanedVector);
        }
        #endregion

        #region Redefinition
        void rename(string first, string second)
        {
            // rename file or directory.
        }

        /**
            Give a new name to anything.
        **/
        void redefine(string target, string name)
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
            else if (listExists(target))
            {
                if (!listExists(name))
                    lists[indexOfList(target)].setName(name);
                else
                    error(ErrorLogger.LIST_UNDEFINED, name, false);
            }
            else if (objectExists(target))
            {
                if (!objectExists(name))
                    objects[indexOfObject(target)].setName(name);
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
