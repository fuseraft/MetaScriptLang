namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        void replaceElement(string before, string after, string replacement)
        {
            System.Collections.Generic.List<string> newList = new();

            for (int i = 0; i < lists[indexOfList(before)].size(); i++)
            {
                if (i == stoi(after))
                    newList.Add(replacement);
                else
                    newList.Add(lists[indexOfList(before)].at(i));
            }

            lists[indexOfList(before)].clear();

            for (int i = 0; i < (int)newList.Count; i++)
                lists[indexOfList(before)].add(newList[i]);

            newList.Clear();
        }

        List getDirectoryList(string before, bool filesOnly)
        {
            List newList = new();
            System.Collections.Generic.List<string> dirList = new();
            // TODO: filesOnly logic
            dirList.AddRange(System.IO.Directory.GetFileSystemEntries(GetVString(before)));

            for (int i = 0; i < dirList.Count; i++)
            {
                newList.add(dirList[i]);
            }

            if (newList.size() == 0)
            {
                __DefiningForLoop = false;
            }

            return newList;
        }

        void app(string arg1, string arg2)
        {
            System.IO.File.AppendAllText(arg1, arg2);
        }

        void appendText(string arg1, string arg2, bool newLine)
        {
            if (VExists(arg1))
            {
                if (isString(arg1))
                {
                    if (System.IO.File.Exists(GetVString(arg1)))
                    {
                        if (VExists(arg2))
                        {
                            if (isString(arg2))
                            {
                                if (newLine)
                                    app(GetVString(arg1), GetVString(arg2) + "\r\n");
                                else
                                    app(GetVString(arg1), GetVString(arg2));
                            }
                            else if (isNumber(arg2))
                            {
                                if (newLine)
                                    app(GetVString(arg1), dtos(GetVNumber(arg2)) + "\r\n");
                                else
                                    app(GetVString(arg1), dtos(GetVNumber(arg2)));
                            }
                            else
                                error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else
                        {
                            if (newLine)
                                app(GetVString(arg1), arg2 + "\r\n");
                            else
                                app(GetVString(arg1), arg2);
                        }
                    }
                    else
                        error(ErrorLogger.READ_FAIL, GetVString(arg1), false);
                }
                else
                    error(ErrorLogger.CONV_ERR, arg1, false);
            }
            else
            {
                if (VExists(arg2))
                {
                    if (isString(arg2))
                    {
                        if (System.IO.File.Exists(arg1))
                        {
                            if (newLine)
                                app(arg1, GetVString(arg2) + "\r\n");
                            else
                                app(arg1, GetVString(arg2));
                        }
                        else
                            error(ErrorLogger.READ_FAIL, GetVString(arg2), false);
                    }
                    else
                        error(ErrorLogger.CONV_ERR, arg2, false);
                }
                else
                {
                    if (System.IO.File.Exists(arg1))
                    {
                        if (newLine)
                            app(arg1, arg2 + "\r\n");
                        else
                            app(arg1, arg2);
                    }
                    else
                        error(ErrorLogger.READ_FAIL, arg1, false);
                }
            }
        }

        void createFile(string fileName)
        {
            try
            {
                using (var fs = System.IO.File.Create(fileName)) { }
            }
            catch { }
        }

        void __fwrite(string arg1, string arg2)
        {
            if (VExists(arg1))
            {
                if (isString(arg1))
                {
                    if (System.IO.File.Exists(GetVString(arg1)))
                    {
                        if (VExists(arg2))
                        {
                            if (isString(arg2))
                            {
                                app(GetVString(arg1), GetVString(arg2) + "\r\n");
                                __LastValue = "0";
                            }
                            else if (isNumber(arg2))
                            {
                                app(GetVString(arg1), dtos(GetVNumber(arg2)) + "\r\n");
                                __LastValue = "0";
                            }
                            else
                            {
                                error(ErrorLogger.IS_NULL, arg2, false);
                                __LastValue = "-1";
                            }
                        }
                        else
                        {
                            app(GetVString(arg1), arg2 + "\r\n");
                            __LastValue = "0";
                        }
                    }
                    else
                    {
                        createFile(GetVString(arg1));

                        if (isString(arg2))
                        {
                            app(GetVString(arg1), GetVString(arg2) + "\r\n");
                            __LastValue = "1";
                        }
                        else if (isNumber(arg2))
                        {
                            app(GetVString(arg1), dtos(GetVNumber(arg2)) + "\r\n");
                            __LastValue = "1";
                        }
                        else
                        {
                            error(ErrorLogger.IS_NULL, arg2, false);
                            __LastValue = "-1";
                        }

                        __LastValue = "1";
                    }
                }
                else
                {
                    error(ErrorLogger.CONV_ERR, arg1, false);
                    __LastValue = "-1";
                }
            }
            else
            {
                if (VExists(arg2))
                {
                    if (isString(arg2))
                    {
                        if (System.IO.File.Exists(arg1))
                        {
                            app(arg1, GetVString(arg2) + "\r\n");
                            __LastValue = "0";
                        }
                        else
                        {
                            createFile(GetVString(arg2));
                            app(arg1, GetVString(arg2) + "\r\n");
                            __LastValue = "1";
                        }
                    }
                    else
                    {
                        error(ErrorLogger.CONV_ERR, arg2, false);
                        __LastValue = "-1";
                    }
                }
                else
                {
                    if (System.IO.File.Exists(arg1))
                    {
                        System.IO.File.AppendAllText(arg1, arg2 + "\r\n");
                        __LastValue = "0";
                    }
                    else
                    {
                        System.IO.File.Create(arg1);
                        System.IO.File.AppendAllText(arg1, arg2 + "\r\n");
                        __LastValue = "1";
                    }
                }
            }
        }
    }
}
