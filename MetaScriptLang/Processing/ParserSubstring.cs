﻿using MetaScriptLang.Logging;

namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        string getSubString(string arg1, string arg2, string beforeBracket)
        {
            string returnValue = ("");

            if (isString(beforeBracket))
            {
                System.Collections.Generic.List<string> listRange = getBracketRange(arg2);

                string variableString = variables[indexOfVariable(beforeBracket)].getString();

                if (listRange.Count == 2)
                {
                    string rangeBegin = (listRange[0]), rangeEnd = (listRange[1]);

                    if (rangeBegin.Length != 0 && rangeEnd.Length != 0)
                    {
                        if (isNumeric(rangeBegin) && isNumeric(rangeEnd))
                        {
                            if (stoi(rangeBegin) < stoi(rangeEnd))
                            {
                                if ((int)variableString.Length - 1 >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                {
                                    string tempString = ("");

                                    for (int i = stoi(rangeBegin); i <= stoi(rangeEnd); i++)
                                        tempString += (variableString[i]);

                                    returnValue = tempString;
                                }
                                else
                                    error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                            }
                            else if (stoi(rangeBegin) > stoi(rangeEnd))
                            {
                                if ((int)variableString.Length >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                {
                                    string tempString = ("");

                                    for (int i = stoi(rangeBegin); i >= stoi(rangeEnd); i--)
                                        tempString += (variableString[i]);

                                    returnValue = tempString;
                                }
                                else
                                    error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                            }
                            else
                                error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                        }
                        else
                            error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                    }
                    else
                        error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                }
                else if (listRange.Count == 1)
                {
                    string rangeBegin = (listRange[0]);

                    if (rangeBegin.Length != 0)
                    {
                        if (isNumeric(rangeBegin))
                        {
                            if ((int)variableString.Length - 1 >= stoi(rangeBegin) && stoi(rangeBegin) >= 0)
                            {
                                string tmp_ = ("");
                                tmp_ += (variableString[stoi(rangeBegin)]);

                                returnValue = tmp_;
                            }
                        }
                    }
                }
                else
                    error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
            }
            else
                error(ErrorLogger.NULL_STRING, beforeBracket, false);

            return (returnValue);
        }

        void setSubString(string arg1, string arg2, string beforeBracket)
        {
            if (isString(beforeBracket))
            {
                System.Collections.Generic.List<string> listRange = getBracketRange(arg2);

                string variableString = variables[indexOfVariable(beforeBracket)].getString();

                if (listRange.Count == 2)
                {
                    string rangeBegin = (listRange[0]), rangeEnd = (listRange[1]);

                    if (rangeBegin.Length != 0 && rangeEnd.Length != 0)
                    {
                        if (isNumeric(rangeBegin) && isNumeric(rangeEnd))
                        {
                            if (stoi(rangeBegin) < stoi(rangeEnd))
                            {
                                if ((int)variableString.Length - 1 >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                {
                                    string tempString = ("");

                                    for (int i = stoi(rangeBegin); i <= stoi(rangeEnd); i++)
                                        tempString += (variableString[i]);

                                    if (variableExists(arg1))
                                        setVariable(arg1, tempString);
                                    else
                                        createVariable(arg1, tempString);
                                }
                                else
                                    error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                            }
                            else if (stoi(rangeBegin) > stoi(rangeEnd))
                            {
                                if ((int)variableString.Length >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                {
                                    string tempString = ("");

                                    for (int i = stoi(rangeBegin); i >= stoi(rangeEnd); i--)
                                        tempString += (variableString[i]);

                                    if (variableExists(arg1))
                                        setVariable(arg1, tempString);
                                    else
                                        createVariable(arg1, tempString);
                                }
                                else
                                    error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                            }
                            else
                                error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                        }
                        else
                            error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                    }
                    else
                        error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                }
                else if (listRange.Count == 1)
                {
                    string rangeBegin = (listRange[0]);

                    if (rangeBegin.Length != 0)
                    {
                        if (isNumeric(rangeBegin))
                        {
                            if ((int)variableString.Length - 1 >= stoi(rangeBegin) && stoi(rangeBegin) >= 0)
                            {
                                string tmp_ = ("");
                                tmp_ += (variableString[stoi(rangeBegin)]);

                                if (variableExists(arg1))
                                    setVariable(arg1, tmp_);
                                else
                                    createVariable(arg1, tmp_);
                            }
                        }
                    }
                }
                else
                    error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
            }
            else
                error(ErrorLogger.NULL_STRING, beforeBracket, false);
        }
    }
}