namespace MetaScriptLang.Processing
{
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
                        builder.Append(subtractChar(builder.ToString(), "{"));

                        if (variableExists(builder.ToString()) && zeroDots(builder.ToString()))
                        {
                            if (isString(builder.ToString()))
                                cleaned.Append(variables[indexOfVariable(builder.ToString())].getString());
                            else if (isNumber(builder.ToString()))
                                cleaned.Append(dtos(variables[indexOfVariable(builder.ToString())].getNumber()));
                            else
                                cleaned.Append("null");
                        }
                        else if (methodExists(builder.ToString()))
                        {
                            parse(builder.ToString());

                            cleaned.Append(__LastValue);
                        }
                        else if (containsParameters(builder.ToString()))
                        {
                            if (stackReady(builder.ToString()))
                            {
                                if (isStringStack(builder.ToString()))
                                    cleaned.Append(getStringStack(builder.ToString()));
                                else
                                    cleaned.Append(dtos(getStack(builder.ToString())));
                            }
                            else if (!zeroDots(builder.ToString()))
                            {
                                string before = (beforeDot(builder.ToString())), after = (afterDot(builder.ToString()));

                                if (objectExists(before))
                                {
                                    if (objects[indexOfObject(before)].methodExists(beforeParameters(after)))
                                    {
                                        executeTemplate(objects[indexOfObject(before)].getMethod(beforeParameters(after)), getParameters(after));

                                        cleaned.Append(__LastValue);
                                    }
                                    else
                                        error(ErrorLogger.METHOD_UNDEFINED, before + "." + beforeParameters(after), false);
                                }
                                else
                                    error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, false);
                            }
                            else if (methodExists(beforeParameters(builder.ToString())))
                            {
                                executeTemplate(methods[indexOfMethod(beforeParameters(builder.ToString()))], getParameters(builder.ToString()));

                                cleaned.Append(__LastValue);
                            }
                            else
                                cleaned.Append("null");
                        }
                        else if (containsBrackets(builder.ToString()))
                        {
                            string _beforeBrackets = beforeBrackets(builder.ToString()), afterBrackets = builder.ToString();
                            string rangeBegin = "", rangeEnd = "", _build = "";

                            System.Collections.Generic.List<string> listRange = getBracketRange(afterBrackets);

                            if (variableExists(_beforeBrackets))
                            {
                                if (isString(_beforeBrackets))
                                {
                                    string tempString = variables[indexOfVariable(_beforeBrackets)].getString();

                                    if (listRange.Count == 2)
                                    {
                                        rangeBegin = listRange[0];
                                        rangeEnd = listRange[1];

                                        if (isNumeric(rangeBegin) && isNumeric(rangeEnd))
                                        {
                                            if (stoi(rangeBegin) < stoi(rangeEnd))
                                            {
                                                if ((int)tempString.Length - 1 >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                                {
                                                    for (int z = stoi(rangeBegin); z <= stoi(rangeEnd); z++)
                                                        _build += (tempString[z]);

                                                    cleaned.Append(_build);
                                                }
                                                else
                                                    error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                            }
                                            else if (stoi(rangeBegin) > stoi(rangeEnd))
                                            {
                                                if ((int)tempString.Length - 1 >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                                {
                                                    for (int z = stoi(rangeBegin); z >= stoi(rangeEnd); z--)
                                                        _build += (tempString[z]);

                                                    cleaned.Append(_build);
                                                }
                                                else
                                                    error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                            }
                                            else
                                                error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                        }
                                    }
                                    else if (listRange.Count == 1)
                                    {
                                        rangeBegin = listRange[0];

                                        if (isNumeric(rangeBegin))
                                        {
                                            if (stoi(rangeBegin) <= (int)tempString.Length - 1 && stoi(rangeBegin) >= 0)
                                            {
                                                cleaned.Append(tempString[stoi(rangeBegin)]);
                                            }
                                            else error(ErrorLogger.OUT_OF_BOUNDS, afterBrackets, false);
                                        }
                                        else error(ErrorLogger.OUT_OF_BOUNDS, afterBrackets, false);
                                    }
                                    else error(ErrorLogger.OUT_OF_BOUNDS, afterBrackets, false);
                                }
                            }
                            else if (listExists(_beforeBrackets))
                            {
                                if (listRange.Count == 2)
                                {
                                    rangeBegin = listRange[0]; rangeEnd = listRange[1];

                                    if (isNumeric(rangeBegin) && isNumeric(rangeEnd))
                                    {
                                        if (stoi(rangeBegin) < stoi(rangeEnd))
                                        {
                                            if (lists[indexOfList(_beforeBrackets)].size() - 1 >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                            {
                                                System.Text.StringBuilder bigString = new();
                                                bigString.Append("(");

                                                for (int z = stoi(rangeBegin); z <= stoi(rangeEnd); z++)
                                                {
                                                    bigString.Append("\"" + lists[indexOfList(_beforeBrackets)].at(z) + "\"");

                                                    if (z < stoi(rangeEnd))
                                                        bigString.Append(',');
                                                }

                                                bigString.Append(')');

                                                cleaned.Append(bigString);
                                            }
                                            else
                                                error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                                        }
                                        else if (stoi(rangeBegin) > stoi(rangeEnd))
                                        {
                                            if (lists[indexOfList(_beforeBrackets)].size() - 1 >= stoi(rangeEnd) && stoi(rangeBegin) >= 0)
                                            {
                                                System.Text.StringBuilder bigString = new();
                                                bigString.Append('(');

                                                for (int z = stoi(rangeBegin); z >= stoi(rangeEnd); z--)
                                                {
                                                    bigString.Append("\"" + lists[indexOfList(_beforeBrackets)].at(z) + "\"");

                                                    if (z > stoi(rangeEnd))
                                                        bigString.Append(',');
                                                }

                                                bigString.Append(')');

                                                cleaned.Append(bigString);
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
                                    rangeBegin = listRange[0];

                                    if (isNumeric(rangeBegin))
                                    {
                                        if (stoi(rangeBegin) <= (int)lists[indexOfList(_beforeBrackets)].size() - 1 && stoi(rangeBegin) >= 0)
                                            cleaned.Append(lists[indexOfList(_beforeBrackets)].at(stoi(rangeBegin)));
                                        else
                                            error(ErrorLogger.OUT_OF_BOUNDS, afterBrackets, false);
                                    }
                                    else
                                        error(ErrorLogger.OUT_OF_BOUNDS, afterBrackets, false);
                                }
                                else
                                    error(ErrorLogger.OUT_OF_BOUNDS, afterBrackets, false);
                            }
                            else
                                cleaned.Append("null");
                        }
                        else if (!zeroDots(builder.ToString()))
                        {
                            string before = (beforeDot(builder.ToString())), after = (afterDot(builder.ToString()));

                            if (objectExists(before))
                            {
                                if (objects[indexOfObject(before)].methodExists(after))
                                {
                                    parse(before + "." + after);
                                    cleaned.Append(__LastValue);
                                }
                                else if (objects[indexOfObject(before)].variableExists(after))
                                {
                                    if (objects[indexOfObject(before)].getVariable(after).getString() != __Null)
                                        cleaned.Append(objects[indexOfObject(before)].getVariable(after).getString());
                                    else if (objects[indexOfObject(before)].getVariable(after).getNumber() != __NullNum)
                                        cleaned.Append(dtos(objects[indexOfObject(before)].getVariable(after).getNumber()));
                                    else
                                        cleaned.Append("null");
                                }
                                else
                                    error(ErrorLogger.VAR_UNDEFINED, before + "." + after, false);
                            }
                            else
                                error(ErrorLogger.OBJ_METHOD_UNDEFINED, before, false);
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
