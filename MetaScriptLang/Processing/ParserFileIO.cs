namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        List getDirectoryList(string before, bool filesOnly)
        {
            List newList = new();
            System.Collections.Generic.List<string> dirList = new();
            // TODO: filesOnly logic
            dirList.AddRange(System.IO.Directory.GetFileSystemEntries(GetVariableString(before)));

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
            if (VariableExists(arg1))
            {
                if (IsStringVariable(arg1))
                {
                    if (System.IO.File.Exists(GetVariableString(arg1)))
                    {
                        if (VariableExists(arg2))
                        {
                            if (IsStringVariable(arg2))
                            {
                                if (newLine)
                                    app(GetVariableString(arg1), GetVariableString(arg2) + "\r\n");
                                else
                                    app(GetVariableString(arg1), GetVariableString(arg2));
                            }
                            else if (IsNumberVariable(arg2))
                            {
                                if (newLine)
                                    app(GetVariableString(arg1), StringHelper.DtoS(GetVariableNumber(arg2)) + "\r\n");
                                else
                                    app(GetVariableString(arg1), StringHelper.DtoS(GetVariableNumber(arg2)));
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else
                        {
                            if (newLine)
                                app(GetVariableString(arg1), arg2 + "\r\n");
                            else
                                app(GetVariableString(arg1), arg2);
                        }
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(arg1), false);
                }
                else
                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg1, false);
            }
            else
            {
                if (VariableExists(arg2))
                {
                    if (IsStringVariable(arg2))
                    {
                        if (System.IO.File.Exists(arg1))
                        {
                            if (newLine)
                                app(arg1, GetVariableString(arg2) + "\r\n");
                            else
                                app(arg1, GetVariableString(arg2));
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.READ_FAIL, GetVariableString(arg2), false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
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
                        ErrorLogger.Error(ErrorLogger.READ_FAIL, arg1, false);
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
            if (VariableExists(arg1))
            {
                if (IsStringVariable(arg1))
                {
                    if (System.IO.File.Exists(GetVariableString(arg1)))
                    {
                        if (VariableExists(arg2))
                        {
                            if (IsStringVariable(arg2))
                            {
                                app(GetVariableString(arg1), GetVariableString(arg2) + "\r\n");
                                __LastValue = "0";
                            }
                            else if (IsNumberVariable(arg2))
                            {
                                app(GetVariableString(arg1), StringHelper.DtoS(GetVariableNumber(arg2)) + "\r\n");
                                __LastValue = "0";
                            }
                            else
                            {
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                                __LastValue = "-1";
                            }
                        }
                        else
                        {
                            app(GetVariableString(arg1), arg2 + "\r\n");
                            __LastValue = "0";
                        }
                    }
                    else
                    {
                        createFile(GetVariableString(arg1));

                        if (IsStringVariable(arg2))
                        {
                            app(GetVariableString(arg1), GetVariableString(arg2) + "\r\n");
                            __LastValue = "1";
                        }
                        else if (IsNumberVariable(arg2))
                        {
                            app(GetVariableString(arg1), StringHelper.DtoS(GetVariableNumber(arg2)) + "\r\n");
                            __LastValue = "1";
                        }
                        else
                        {
                            ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                            __LastValue = "-1";
                        }

                        __LastValue = "1";
                    }
                }
                else
                {
                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg1, false);
                    __LastValue = "-1";
                }
            }
            else
            {
                if (VariableExists(arg2))
                {
                    if (IsStringVariable(arg2))
                    {
                        if (System.IO.File.Exists(arg1))
                        {
                            app(arg1, GetVariableString(arg2) + "\r\n");
                            __LastValue = "0";
                        }
                        else
                        {
                            createFile(GetVariableString(arg2));
                            app(arg1, GetVariableString(arg2) + "\r\n");
                            __LastValue = "1";
                        }
                    }
                    else
                    {
                        ErrorLogger.Error(ErrorLogger.CONV_ERR, arg2, false);
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
