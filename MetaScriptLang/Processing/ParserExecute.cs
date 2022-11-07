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
                if (VariableExists(strings[i]))
                {
                    if (IsStringVariable(strings[i]))
                        CreateVariableString(methodVariables[i].name(), GetVariableString(strings[i]));
                    else if (IsNumberVariable(strings[i]))
                        CreateVariableNumber(methodVariables[i].name(), GetVariableNumber(strings[i]));
                }
                else if (MethodExists(strings[i]))
                {
                    parse(strings[i]);

                    if (StringHelper.IsNumeric(__LastValue))
                        CreateVariableNumber(methodVariables[i].name(), StringHelper.StoD(__LastValue));
                    else
                        CreateVariableString(methodVariables[i].name(), __LastValue);
                }
                else
                {
                    if (StringHelper.IsNumeric(strings[i]))
                        CreateVariableNumber(methodVariables[i].name(), StringHelper.StoD(strings[i]));
                    else
                        CreateVariableString(methodVariables[i].name(), strings[i]);
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
                        variableString += (StringHelper.ItoS(a));

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

            gc.DoGarbageCollection(); // if (!__DontCollectMethodVars)
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
                            variableString += (StringHelper.ItoS(a));

                            if (words[x] == m.GetVariables()[a].name())
                            {
                                found = true;

                                if (m.GetVariables()[a].getString() != __Null)
                                    newWords.Add(m.GetVariables()[a].getString());
                                else if (m.GetVariables()[a].getNumber() != __NullNum)
                                    newWords.Add(StringHelper.DtoS(m.GetVariables()[a].getNumber()));
                            }
                            else if (words[x] == variableString)
                            {
                                found = true;

                                if (m.GetVariables()[a].getString() != __Null)
                                    newWords.Add(m.GetVariables()[a].getString());
                                else if (m.GetVariables()[a].getNumber() != __NullNum)
                                    newWords.Add(StringHelper.DtoS(m.GetVariables()[a].getNumber()));
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

            gc.DoGarbageCollection();
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
