namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        void app(string arg1, string arg2)
        {
            System.IO.File.AppendAllText(arg1, arg2);
        }

        void appendText(string arg1, string arg2, bool newLine)
        {
            if (engine.VariableExists(arg1))
            {
                if (engine.IsStringVariable(arg1))
                {
                    if (System.IO.File.Exists(engine.GetVariableString(arg1)))
                    {
                        if (engine.VariableExists(arg2))
                        {
                            if (engine.IsStringVariable(arg2))
                            {
                                if (newLine)
                                    app(engine.GetVariableString(arg1), engine.GetVariableString(arg2) + "\r\n");
                                else
                                    app(engine.GetVariableString(arg1), engine.GetVariableString(arg2));
                            }
                            else if (engine.IsNumberVariable(arg2))
                            {
                                if (newLine)
                                    app(engine.GetVariableString(arg1), StringHelper.DtoS(engine.GetVariableNumber(arg2)) + "\r\n");
                                else
                                    app(engine.GetVariableString(arg1), StringHelper.DtoS(engine.GetVariableNumber(arg2)));
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.IS_NULL, arg2, false);
                        }
                        else
                        {
                            if (newLine)
                                app(engine.GetVariableString(arg1), arg2 + "\r\n");
                            else
                                app(engine.GetVariableString(arg1), arg2);
                        }
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.READ_FAIL, engine.GetVariableString(arg1), false);
                }
                else
                    ErrorLogger.Error(ErrorLogger.CONV_ERR, arg1, false);
            }
            else
            {
                if (engine.VariableExists(arg2))
                {
                    if (engine.IsStringVariable(arg2))
                    {
                        if (System.IO.File.Exists(arg1))
                        {
                            if (newLine)
                                app(arg1, engine.GetVariableString(arg2) + "\r\n");
                            else
                                app(arg1, engine.GetVariableString(arg2));
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.READ_FAIL, engine.GetVariableString(arg2), false);
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
            if (engine.VariableExists(arg1))
            {
                if (engine.IsStringVariable(arg1))
                {
                    if (System.IO.File.Exists(engine.GetVariableString(arg1)))
                    {
                        if (engine.VariableExists(arg2))
                        {
                            if (engine.IsStringVariable(arg2))
                            {
                                app(engine.GetVariableString(arg1), engine.GetVariableString(arg2) + "\r\n");
                                __LastValue = "0";
                            }
                            else if (engine.IsNumberVariable(arg2))
                            {
                                app(engine.GetVariableString(arg1), StringHelper.DtoS(engine.GetVariableNumber(arg2)) + "\r\n");
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
                            app(engine.GetVariableString(arg1), arg2 + "\r\n");
                            __LastValue = "0";
                        }
                    }
                    else
                    {
                        createFile(engine.GetVariableString(arg1));

                        if (engine.IsStringVariable(arg2))
                        {
                            app(engine.GetVariableString(arg1), engine.GetVariableString(arg2) + "\r\n");
                            __LastValue = "1";
                        }
                        else if (engine.IsNumberVariable(arg2))
                        {
                            app(engine.GetVariableString(arg1), StringHelper.DtoS(engine.GetVariableNumber(arg2)) + "\r\n");
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
                if (engine.VariableExists(arg2))
                {
                    if (engine.IsStringVariable(arg2))
                    {
                        if (System.IO.File.Exists(arg1))
                        {
                            app(arg1, engine.GetVariableString(arg2) + "\r\n");
                            __LastValue = "0";
                        }
                        else
                        {
                            createFile(engine.GetVariableString(arg2));
                            app(arg1, engine.GetVariableString(arg2) + "\r\n");
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
