namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;
    
    public partial class Parser
    {
        void executeTemplate(MetaScriptLang.Data.Method m, System.Collections.Generic.List<string> strings)
        {
            System.Collections.Generic.List<string> methodLines = new();

            engine.__ExecutedTemplate = true;
            engine.__DontCollectMethodVars = true;
            __CurrentMethodObject = m.GetObject();

            System.Collections.Generic.List<Variable> methodVariables = m.GetVariables();

            for (int i = 0; i < (int)methodVariables.Count; i++)
            {
                if (engine.VariableExists(strings[i]))
                {
                    if (engine.IsStringVariable(strings[i]))
                        engine.CreateVariableString(methodVariables[i].SetName(), engine.GetVariableString(strings[i]));
                    else if (engine.IsNumberVariable(strings[i]))
                        engine.CreateVariableNumber(methodVariables[i].SetName(), engine.GetVariableNumber(strings[i]));
                }
                else if (engine.MethodExists(strings[i]))
                {
                    ParseString(strings[i]);

                    if (StringHelper.IsNumeric(__LastValue))
                        engine.CreateVariableNumber(methodVariables[i].SetName(), StringHelper.StoD(__LastValue));
                    else
                        engine.CreateVariableString(methodVariables[i].SetName(), __LastValue);
                }
                else
                {
                    if (StringHelper.IsNumeric(strings[i]))
                        engine.CreateVariableNumber(methodVariables[i].SetName(), StringHelper.StoD(strings[i]));
                    else
                        engine.CreateVariableString(methodVariables[i].SetName(), strings[i]);
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
                ParseString(methodLines[i]);

            engine.__ExecutedTemplate = false;
            engine.__DontCollectMethodVars = false;

            gc.DoGarbageCollection(); // if (!engine.__DontCollectMethodVars)
        }

        void executeMethod(Method m)
        {
            engine.__ExecutedMethod = true;
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

                            if (words[x] == m.GetVariables()[a].SetName())
                            {
                                found = true;

                                if (m.GetVariables()[a].GetStringValue() != __Null)
                                    newWords.Add(m.GetVariables()[a].GetStringValue());
                                else if (m.GetVariables()[a].GetNumberValue() != __NullNum)
                                    newWords.Add(StringHelper.DtoS(m.GetVariables()[a].GetNumberValue()));
                            }
                            else if (words[x] == variableString)
                            {
                                found = true;

                                if (m.GetVariables()[a].GetStringValue() != __Null)
                                    newWords.Add(m.GetVariables()[a].GetStringValue());
                                else if (m.GetVariables()[a].GetNumberValue() != __NullNum)
                                    newWords.Add(StringHelper.DtoS(m.GetVariables()[a].GetNumberValue()));
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
                        ParseString(methodLines[ii]);
                }
            }
            else
                for (int ii = 0; ii < m.GetMethodSize(); ii++)
                    ParseString(m.GetLine(ii));

            engine.__ExecutedMethod = false;

            gc.DoGarbageCollection();
        }

        void executeNest(MetaScriptLang.Data.SwitchCase n)
        {
            __DefiningNest = false;
            __DefiningIfStatement = false;

            for (int i = 0; i < n.Count; i++)
            {
                if (engine.__FailedNest == false)
                    ParseString(n[i]);
                else
                    break;
            }

            __DefiningIfStatement = true;
        }

    }
}
