namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;
    
    public partial class Parser
    {
        void executeTemplate(MetaScriptLang.Data.Method m, System.Collections.Generic.List<string> strings)
        {
            System.Collections.Generic.List<string> methodLines = new();

            __ExecutedTemplate = true;
            __DontCollectMethodVars = true;
            __CurrentMethodObject = m.GetObject();

            System.Collections.Generic.List<Variable> methodVariables = m.GetVariables();

            for (int i = 0; i < (int)methodVariables.Count; i++)
            {
                if (VExists(strings[i]))
                {
                    if (isString(strings[i]))
                        CreateVString(methodVariables[i].name(), GetVString(strings[i]));
                    else if (isNumber(strings[i]))
                        CreateVNumber(methodVariables[i].name(), GetVNumber(strings[i]));
                }
                else if (MExists(strings[i]))
                {
                    parse(strings[i]);

                    if (StringHelper.IsNumeric(__LastValue))
                        CreateVNumber(methodVariables[i].name(), stod(__LastValue));
                    else
                        CreateVString(methodVariables[i].name(), __LastValue);
                }
                else
                {
                    if (StringHelper.IsNumeric(strings[i]))
                        CreateVNumber(methodVariables[i].name(), stod(strings[i]));
                    else
                        CreateVString(methodVariables[i].name(), strings[i]);
                }
            }

            for (int i = 0; i < m.GetMethodSize(); i++)
            {
                string line = m.GetLine(i);
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
            __CurrentMethodObject = m.GetObject();

            if (__DefiningParameterizedMethod)
            {
                System.Collections.Generic.List<string> methodLines = new();

                for (int i = 0; i < (int)m.GetMethodSize(); i++)
                {
                    string line = m.GetLine(i);
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

                        for (int a = 0; a < (int)m.GetVariables().Count; a++)
                        {
                            string variableString = ("$");
                            variableString += (itos(a));

                            if (words[x] == m.GetVariables()[a].name())
                            {
                                found = true;

                                if (m.GetVariables()[a].getString() != __Null)
                                    newWords.Add(m.GetVariables()[a].getString());
                                else if (m.GetVariables()[a].getNumber() != __NullNum)
                                    newWords.Add(dtos(m.GetVariables()[a].getNumber()));
                            }
                            else if (words[x] == variableString)
                            {
                                found = true;

                                if (m.GetVariables()[a].getString() != __Null)
                                    newWords.Add(m.GetVariables()[a].getString());
                                else if (m.GetVariables()[a].getNumber() != __NullNum)
                                    newWords.Add(dtos(m.GetVariables()[a].getNumber()));
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
                for (int ii = 0; ii < m.GetMethodSize(); ii++)
                    parse(m.GetLine(ii));

            __ExecutedMethod = false;

            collectGarbage();
        }

        void executeNest(MetaScriptLang.Data.SwitchCase n)
        {
            __DefiningNest = false;
            __DefiningIfStatement = false;

            for (int i = 0; i < n.Count; i++)
            {
                if (__FailedNest == false)
                    parse(n[i]);
                else
                    break;
            }

            __DefiningIfStatement = true;
        }

    }
}
