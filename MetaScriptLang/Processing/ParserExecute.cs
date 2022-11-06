namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    
    public partial class Parser
    {
        void executeTemplate(MetaScriptLang.Data.Method m, System.Collections.Generic.List<string> strings)
        {
            System.Collections.Generic.List<string> methodLines = new();

            __ExecutedTemplate = true;
            __DontCollectMethodVars = true;
            __CurrentMethodObject = m.getObject();

            System.Collections.Generic.List<Variable> methodVariables = m.getMethodVariables();

            for (int i = 0; i < (int)methodVariables.Count; i++)
            {
                if (variableExists(strings[i]))
                {
                    if (isString(strings[i]))
                        createVariable(methodVariables[i].name(), variables[indexOfVariable(strings[i])].getString());
                    else if (isNumber(strings[i]))
                        createVariable(methodVariables[i].name(), variables[indexOfVariable(strings[i])].getNumber());
                }
                else if (methodExists(strings[i]))
                {
                    parse(strings[i]);

                    if (isNumeric(__LastValue))
                        createVariable(methodVariables[i].name(), stod(__LastValue));
                    else
                        createVariable(methodVariables[i].name(), __LastValue);
                }
                else
                {
                    if (isNumeric(strings[i]))
                        createVariable(methodVariables[i].name(), stod(strings[i]));
                    else
                        createVariable(methodVariables[i].name(), strings[i]);
                }
            }

            for (int i = 0; i < m.size(); i++)
            {
                string line = m.at(i);
                string word = string.Empty;
                int len = line.Length;
                System.Collections.Generic.List<string> words = new();

                for (int x = 0; x < len; x++)
                {
                    if (line[x] == ' ')
                    {
                        words.Add(word);
                        word = string.Empty;
                    }
                    else
                        word += line[x];
                }

                words.Add(word);

                System.Collections.Generic.List<string> newWords = new();

                for (int x = 0; x < (int)words.Count; x++)
                {
                    bool found = false;

                    for (int a = 0; a < (int)strings.Count; a++)
                    {
                        string variableString = ("$");
                        variableString += (itos(a));

                        if (words[x] == variableString)
                        {
                            found = true;

                            newWords.Add(strings[a]);
                        }
                    }

                    if (!found)
                        newWords.Add(words[x]);
                }

                string freshLine = ("");

                for (int b = 0; b<(int)newWords.Count; b++)
                {
                    freshLine += (newWords[b]);

                    if (b != (int) newWords.Count - 1)
                        freshLine += (' ');
                }

                methodLines.Add(freshLine);
            }

            for (int i = 0; i < (int)methodLines.Count; i++)
                parse(methodLines[i]);

            __ExecutedTemplate = false;
            __DontCollectMethodVars = false;

            collectGarbage(); // if (!__DontCollectMethodVars)
        }

        void executeMethod(Method m)
        {
            __ExecutedMethod = true;
            __CurrentMethodObject = m.getObject();

            if (__DefiningParameterizedMethod)
            {
                System.Collections.Generic.List<string> methodLines = new();

                for (int i = 0; i < (int)m.size(); i++)
                {
                    string line = m.at(i);
                    string word = ("");
                    int len = line.Length;
                    System.Collections.Generic.List<string> words = new();

                    for (int x = 0; x < len; x++)
                    {
                        if (line[x] == ' ')
                        {
                            words.Add(word);
                            word = string.Empty;
                        }
                        else
                            word += (line[x]);
                    }

                    words.Add(word);

                    System.Collections.Generic.List<string> newWords = new();

                    for (int x = 0; x < (int)words.Count; x++)
                    {
                        bool found = false;

                        for (int a = 0; a < (int)m.getMethodVariables().Count; a++)
                        {
                            string variableString = ("$");
                            variableString += (itos(a));

                            if (words[x] == m.getMethodVariables()[a].name())
                            {
                                found = true;

                                if (m.getMethodVariables()[a].getString() != __Null)
                                    newWords.Add(m.getMethodVariables()[a].getString());
                                else if (m.getMethodVariables()[a].getNumber() != __NullNum)
                                    newWords.Add(dtos(m.getMethodVariables()[a].getNumber()));
                            }
                            else if (words[x] == variableString)
                            {
                                found = true;

                                if (m.getMethodVariables()[a].getString() != __Null)
                                    newWords.Add(m.getMethodVariables()[a].getString());
                                else if (m.getMethodVariables()[a].getNumber() != __NullNum)
                                    newWords.Add(dtos(m.getMethodVariables()[a].getNumber()));
                            }
                        }

                        if (!found)
                            newWords.Add(words[x]);
                    }

                    string freshLine = ("");

                    for (int b = 0; b < (int)newWords.Count; b++)
                    {
                        freshLine += (newWords[b]);

                        if (b != (int)newWords.Count - 1)
                            freshLine += (' ');
                    }

                    methodLines.Add(freshLine);

                    for (int ii = 0; ii < (int)methodLines.Count; ii++)
                        parse(methodLines[ii]);
                }
            }
            else
                for (int ii = 0; ii < m.size(); ii++)
                    parse(m.at(ii));

            __ExecutedMethod = false;

            collectGarbage();
        }

        void executeNest(MetaScriptLang.Data.Container n)
        {
            __DefiningNest = false;
            __DefiningIfStatement = false;

            for (int i = 0; i < n.size(); i++)
            {
                if (__FailedNest == false)
                    parse(n.at(i));
                else
                    break;
            }

            __DefiningIfStatement = true;
        }

    }
}
