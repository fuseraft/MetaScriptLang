namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Helpers;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        string getSubString(string arg1, string arg2, string beforeBracket)
        {
            string returnValue = ("");

            if (engine.IsStringVariable(beforeBracket))
            {
                System.Collections.Generic.List<string> listRange = StringHelper.GetBracketRange(arg2);

                string variableString = engine.GetVariableString(beforeBracket);

                if (listRange.Count == 2)
                {
                    string rangeBegin = (listRange[0]), rangeEnd = (listRange[1]);

                    if (rangeBegin.Length != 0 && rangeEnd.Length != 0)
                    {
                        if (StringHelper.IsNumeric(rangeBegin) && StringHelper.IsNumeric(rangeEnd))
                        {
                            if (StringHelper.StoI(rangeBegin) < StringHelper.StoI(rangeEnd))
                            {
                                if (variableString.Length - 1 >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0)
                                {
                                    string tempString = ("");

                                    for (int i = StringHelper.StoI(rangeBegin); i <= StringHelper.StoI(rangeEnd); i++)
                                        tempString += (variableString[i]);

                                    returnValue = tempString;
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                            }
                            else if (StringHelper.StoI(rangeBegin) > StringHelper.StoI(rangeEnd))
                            {
                                if (variableString.Length >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0)
                                {
                                    string tempString = ("");

                                    for (int i = StringHelper.StoI(rangeBegin); i >= StringHelper.StoI(rangeEnd); i--)
                                        tempString += (variableString[i]);

                                    returnValue = tempString;
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                }
                else if (listRange.Count == 1)
                {
                    string rangeBegin = (listRange[0]);

                    if (rangeBegin.Length != 0)
                    {
                        if (StringHelper.IsNumeric(rangeBegin))
                        {
                            if (variableString.Length - 1 >= StringHelper.StoI(rangeBegin) && StringHelper.StoI(rangeBegin) >= 0)
                            {
                                string tmp_ = ("");
                                tmp_ += (variableString[StringHelper.StoI(rangeBegin)]);

                                returnValue = tmp_;
                            }
                        }
                    }
                }
                else
                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
            }
            else
                ErrorLogger.Error(ErrorLogger.NULL_STRING, beforeBracket, false);

            return (returnValue);
        }

        void setSubString(string arg1, string arg2, string beforeBracket)
        {
            if (engine.IsStringVariable(beforeBracket))
            {
                System.Collections.Generic.List<string> listRange = StringHelper.GetBracketRange(arg2);

                string variableString = engine.GetVariableString(beforeBracket);

                if (listRange.Count == 2)
                {
                    string rangeBegin = (listRange[0]), rangeEnd = (listRange[1]);

                    if (rangeBegin.Length != 0 && rangeEnd.Length != 0)
                    {
                        if (StringHelper.IsNumeric(rangeBegin) && StringHelper.IsNumeric(rangeEnd))
                        {
                            if (StringHelper.StoI(rangeBegin) < StringHelper.StoI(rangeEnd))
                            {
                                if (variableString.Length - 1 >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0)
                                {
                                    string tempString = ("");

                                    for (int i = StringHelper.StoI(rangeBegin); i <= StringHelper.StoI(rangeEnd); i++)
                                        tempString += (variableString[i]);

                                    if (engine.VariableExists(arg1))
                                        engine.SetVariableString(arg1, tempString);
                                    else
                                        engine.CreateStringVariable(arg1, tempString);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                            }
                            else if (StringHelper.StoI(rangeBegin) > StringHelper.StoI(rangeEnd))
                            {
                                if (variableString.Length >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0)
                                {
                                    string tempString = ("");

                                    for (int i = StringHelper.StoI(rangeBegin); i >= StringHelper.StoI(rangeEnd); i--)
                                        tempString += (variableString[i]);

                                    if (engine.VariableExists(arg1))
                                        engine.SetVariableString(arg1, tempString);
                                    else
                                        engine.CreateStringVariable(arg1, tempString);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                        }
                        else
                            ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                }
                else if (listRange.Count == 1)
                {
                    string rangeBegin = (listRange[0]);

                    if (rangeBegin.Length != 0)
                    {
                        if (StringHelper.IsNumeric(rangeBegin))
                        {
                            if (variableString.Length - 1 >= StringHelper.StoI(rangeBegin) && StringHelper.StoI(rangeBegin) >= 0)
                            {
                                string tmp_ = ("");
                                tmp_ += (variableString[StringHelper.StoI(rangeBegin)]);

                                if (engine.VariableExists(arg1))
                                    engine.SetVariableString(arg1, tmp_);
                                else
                                    engine.CreateStringVariable(arg1, tmp_);
                            }
                        }
                    }
                }
                else
                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
            }
            else
                ErrorLogger.Error(ErrorLogger.NULL_STRING, beforeBracket, false);
        }
    }
}
