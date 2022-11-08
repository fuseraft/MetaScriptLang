namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Helpers;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        string cleanString(string st)
        {
            System.Text.StringBuilder cleaned = new();
            System.Text.StringBuilder builder = new();
            int l = st.Length;
            bool buildSymbol = false;

            for (int i = 0; i < l; i++)
            {
                if (buildSymbol)
                {
                    if (st[i] == '}')
                    {
                        builder.Clear();
                        builder.Append(StringHelper.SubtractChars(builder.ToString(), "{"));

                        if (VariableExists(builder.ToString()) && StringHelper.ZeroDots(builder.ToString()))
                        {
                            if (IsStringVariable(builder.ToString()))
                                cleaned.Append(GetVariableString(builder.ToString()));
                            else if (IsNumberVariable(builder.ToString()))
                                cleaned.Append(StringHelper.DtoS(GetVariableNumber(builder.ToString())));
                            else
                                cleaned.Append("null");
                        }
                        else if (engine.MethodExists(builder.ToString()))
                        {
                            parse(builder.ToString());

                            cleaned.Append(__LastValue);
                        }
                        else if (StringHelper.ContainsParameters(builder.ToString()))
                        {
                            if (stackReady(builder.ToString()))
                            {
                                if (isStringStack(builder.ToString()))
                                    cleaned.Append(getStringStack(builder.ToString()));
                                else
                                    cleaned.Append(StringHelper.DtoS(getStack(builder.ToString())));
                            }
                            else if (!StringHelper.ZeroDots(builder.ToString()))
                            {
                                string before = (StringHelper.BeforeDot(builder.ToString())), after = (StringHelper.AfterDot(builder.ToString()));

                                if (engine.ObjectExists(before))
                                {
                                    if (engine.ObjectMethodExists(before, StringHelper.BeforeParameters(after)))
                                    {
                                        executeTemplate(engine.GetObjectMethod(before, StringHelper.BeforeParameters(after)), StringHelper.GetParameters(after));

                                        cleaned.Append(__LastValue);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.METHOD_UNDEFINED, before + "." + StringHelper.BeforeParameters(after), false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, false);
                            }
                            else if (engine.MethodExists(StringHelper.BeforeParameters(builder.ToString())))
                            {
                                executeTemplate(engine.GetMethod(StringHelper.BeforeParameters(builder.ToString())), StringHelper.GetParameters(builder.ToString()));

                                cleaned.Append(__LastValue);
                            }
                            else
                                cleaned.Append("null");
                        }
                        else if (StringHelper.ContainsBrackets(builder.ToString()))
                        {
                            // TODO revisit this logic.
                            string beforeBrackets = StringHelper.BeforeBrackets(builder.ToString()), afterBrackets = builder.ToString();
                            string rangeBegin = "", rangeEnd = "", _build = "";

                            System.Collections.Generic.List<string> listRange = StringHelper.GetBracketRange(afterBrackets);

                            if (VariableExists(beforeBrackets))
                            {
                                if (IsStringVariable(beforeBrackets))
                                {
                                    string tempString = GetVariableString(beforeBrackets);

                                    if (listRange.Count == 2)
                                    {
                                        rangeBegin = listRange[0];
                                        rangeEnd = listRange[1];

                                        if (StringHelper.IsNumeric(rangeBegin) && StringHelper.IsNumeric(rangeEnd))
                                        {
                                            if (StringHelper.StoI(rangeBegin) < StringHelper.StoI(rangeEnd))
                                            {
                                                if ((int)tempString.Length - 1 >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0)
                                                {
                                                    for (int z = StringHelper.StoI(rangeBegin); z <= StringHelper.StoI(rangeEnd); z++)
                                                        _build += (tempString[z]);

                                                    cleaned.Append(_build);
                                                }
                                                else
                                                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                            }
                                            else if (StringHelper.StoI(rangeBegin) > StringHelper.StoI(rangeEnd))
                                            {
                                                if ((int)tempString.Length - 1 >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0)
                                                {
                                                    for (int z = StringHelper.StoI(rangeBegin); z >= StringHelper.StoI(rangeEnd); z--)
                                                        _build += (tempString[z]);

                                                    cleaned.Append(_build);
                                                }
                                                else
                                                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                            }
                                            else
                                                ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                        }
                                    }
                                    else if (listRange.Count == 1)
                                    {
                                        rangeBegin = listRange[0];

                                        if (StringHelper.IsNumeric(rangeBegin))
                                        {
                                            if (StringHelper.StoI(rangeBegin) <= (int)tempString.Length - 1 && StringHelper.StoI(rangeBegin) >= 0)
                                            {
                                                cleaned.Append(tempString[StringHelper.StoI(rangeBegin)]);
                                            }
                                            else ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, afterBrackets, false);
                                        }
                                        else ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, afterBrackets, false);
                                    }
                                    else ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, afterBrackets, false);
                                }
                            }
                            else if (engine.ListExists(beforeBrackets))
                            {
                                if (listRange.Count == 2)
                                {
                                    rangeBegin = listRange[0]; rangeEnd = listRange[1];

                                    if (StringHelper.IsNumeric(rangeBegin) && StringHelper.IsNumeric(rangeEnd))
                                    {
                                        if (StringHelper.StoI(rangeBegin) < StringHelper.StoI(rangeEnd))
                                        {
                                            if (engine.GetListSize(beforeBrackets) - 1 >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0)
                                            {
                                                System.Text.StringBuilder bigString = new();
                                                bigString.Append("(");

                                                for (int z = StringHelper.StoI(rangeBegin); z <= StringHelper.StoI(rangeEnd); z++)
                                                {
                                                    bigString.Append("\"" + engine.GetListLine(beforeBrackets, z) + "\"");

                                                    if (z < StringHelper.StoI(rangeEnd))
                                                        bigString.Append(',');
                                                }

                                                bigString.Append(')');

                                                cleaned.Append(bigString);
                                            }
                                            else
                                                ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                        }
                                        else if (StringHelper.StoI(rangeBegin) > StringHelper.StoI(rangeEnd))
                                        {
                                            if (engine.GetListSize(beforeBrackets) - 1 >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0)
                                            {
                                                System.Text.StringBuilder bigString = new();
                                                bigString.Append('(');

                                                for (int z = StringHelper.StoI(rangeBegin); z >= StringHelper.StoI(rangeEnd); z--)
                                                {
                                                    bigString.Append("\"" + engine.GetListLine(beforeBrackets, z) + "\"");

                                                    if (z > StringHelper.StoI(rangeEnd))
                                                        bigString.Append(',');
                                                }

                                                bigString.Append(')');

                                                cleaned.Append(bigString);
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
                                    rangeBegin = listRange[0];

                                    if (StringHelper.IsNumeric(rangeBegin))
                                    {
                                        if (StringHelper.StoI(rangeBegin) <= (int)engine.GetListSize(beforeBrackets) - 1 && StringHelper.StoI(rangeBegin) >= 0)
                                            cleaned.Append(engine.GetListLine(beforeBrackets, StringHelper.StoI(rangeBegin)));
                                        else
                                            ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, afterBrackets, false);
                                    }
                                    else
                                        ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, afterBrackets, false);
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, afterBrackets, false);
                            }
                            else
                                cleaned.Append("null");
                        }
                        else if (!StringHelper.ZeroDots(builder.ToString()))
                        {
                            string before = StringHelper.BeforeDot(builder.ToString()), after = StringHelper.AfterDot(builder.ToString());

                            if (engine.ObjectExists(before))
                            {
                                if (engine.ObjectMethodExists(before, after))
                                {
                                    parse(before + "." + after);
                                    cleaned.Append(__LastValue);
                                }
                                else if (engine.ObjectVariableExists(before, after))
                                {
                                    if (engine.GetObjectVariableString(before, after) != __Null)
                                        cleaned.Append(engine.GetObjectVariableString(before, after));
                                    else if (engine.GetObjectVariableNumber(before, after) != __NullNum)
                                        cleaned.Append(StringHelper.DtoS(engine.GetObjectVariableNumber(before, after)));
                                    else
                                        cleaned.Append("null");
                                }
                                else
                                    ErrorLogger.Error(ErrorLogger.VAR_UNDEFINED, before + "." + after, false);
                            }
                            else
                                ErrorLogger.Error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, false);
                        }
                        else
                            cleaned.Append(builder.ToString());

                        builder.Clear();

                        buildSymbol = false;
                    }
                    else
                        builder.Append(st[i]);
                }
                else
                {
                    // REFACTOR HERE
                    if (st[i] == '\\' && st[i + 1] == 'n') // begin new-line
                        cleaned.Append('\r');
                    else if (st[i] == 'n' && st[i - 1] == '\\') // end new-line
                        cleaned.Append('\n');
                    else if (st[i] == '\\' && st[i + 1] == 't') // begin tab
                        doNothing();
                    else if (st[i] == 't' && st[i - 1] == '\\') // end tab
                        cleaned.Append('\t');
                    else if (st[i] == '\\' && st[i + 1] == ';') // begin semi-colon
                        doNothing();
                    else if (st[i] == ';' && st[i - 1] == '\\') // end semi-colon
                        cleaned.Append(';');
                    else if (st[i] == '\\' && st[i + 1] == '\'') // begin apostrophe
                        doNothing();
                    else if (st[i] == '\'' && st[i - 1] == '\\') // end apostrophe
                        cleaned.Append('\'');
                    else if (st[i] == '\\' && st[i + 1] == '{') // begin symbol
                        buildSymbol = true;
                    else
                        cleaned.Append(st[i]);
                }
            }

            return cleaned.ToString();
        }
    }
}
