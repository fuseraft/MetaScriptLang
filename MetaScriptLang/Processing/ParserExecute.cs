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

            for (int i = 0; i < methodVariables.Count; i++)
            {
                if (engine.VariableExists(strings[i]))
                {
                    if (engine.IsStringVariable(strings[i]))
                        engine.CreateStringVariable(methodVariables[i].Name, engine.GetVariableString(strings[i]));
                    else if (engine.IsNumberVariable(strings[i]))
                        engine.CreateNumberVariable(methodVariables[i].Name, engine.GetVariableNumber(strings[i]));
                }
                else if (engine.MethodExists(strings[i]))
                {
                    ParseString(strings[i]);

                    if (StringHelper.IsNumeric(__LastValue))
                        engine.CreateNumberVariable(methodVariables[i].Name, StringHelper.StoD(__LastValue));
                    else
                        engine.CreateStringVariable(methodVariables[i].Name, __LastValue);
                }
                else
                {
                    if (StringHelper.IsNumeric(strings[i]))
                        engine.CreateNumberVariable(methodVariables[i].Name, StringHelper.StoD(strings[i]));
                    else
                        engine.CreateStringVariable(methodVariables[i].Name, strings[i]);
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

                for (int x = 0; x < words.Count; x++)
                {
                    bool found = false;

                    for (int a = 0; a < strings.Count; a++)
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

                for (int b = 0; b<newWords.Count; b++)
                {
                    freshLine += (newWords[b]);

                    if (b !=  newWords.Count - 1)
                        freshLine += (' ');
                }

                methodLines.Add(freshLine);
            }

            for (int i = 0; i < methodLines.Count; i++)
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

                for (int i = 0; i < m.GetMethodSize(); i++)
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

                    for (int x = 0; x < words.Count; x++)
                    {
                        bool found = false;

                        for (int a = 0; a < m.GetVariables().Count; a++)
                        {
                            string variableString = ("$");
                            variableString += (StringHelper.ItoS(a));

                            if (words[x] == m.GetVariables()[a].Name)
                            {
                                found = true;

                                if (m.GetVariables()[a].StringValue != __Null)
                                    newWords.Add(m.GetVariables()[a].StringValue);
                                else if (m.GetVariables()[a].NumberValue != __NullNum)
                                    newWords.Add(StringHelper.DtoS(m.GetVariables()[a].NumberValue));
                            }
                            else if (words[x] == variableString)
                            {
                                found = true;

                                if (m.GetVariables()[a].StringValue != __Null)
                                    newWords.Add(m.GetVariables()[a].StringValue);
                                else if (m.GetVariables()[a].NumberValue != __NullNum)
                                    newWords.Add(StringHelper.DtoS(m.GetVariables()[a].NumberValue));
                            }
                        }

                        if (!found)
                            newWords.Add(words[x]);
                    }

                    string freshLine = ("");

                    for (int b = 0; b < newWords.Count; b++)
                    {
                        freshLine += (newWords[b]);

                        if (b != newWords.Count - 1)
                            freshLine += (' ');
                    }

                    methodLines.Add(freshLine);

                    for (int ii = 0; ii < methodLines.Count; ii++)
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
