namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        void loadSavedVars(MetaScriptLang.Data.Crypt c, string bigStr)
        {
            if (System.IO.File.Exists(__SavedVars))
            {
                bigStr = System.IO.File.ReadAllText(__SavedVars);
                bigStr = c.d(bigStr);
                int bigStrLength = bigStr.Length;
                bool stop = false;
                System.Collections.Generic.List<string> varNames = new();
                System.Collections.Generic.List<string> varValues = new();

                string varName = string.Empty;
                varNames.Add("");
                varValues.Add("");

                for (int i = 0; i < bigStrLength; i++)
                {
                    switch (bigStr[i])
                    {
                        case '&':
                            stop = true;
                            break;

                        case '#':
                            stop = false;
                            varNames.Add("");
                            varValues.Add("");
                            break;

                        default:
                            if (!stop)
                                varNames[(int)varNames.Count - 1] += (bigStr[i]);
                            else
                                varValues[(int)varValues.Count - 1] += (bigStr[i]);
                            break;
                    }
                }

                for (int i = 0; i < (int)varNames.Count; i++)
                {
                    Variable newVariable = new(varNames[i], varValues[i]);
                    variables.Add(varNames[i], newVariable);
                }

                varNames.Clear();
                varValues.Clear();
            }
            else
                error(ErrorLogger.READ_FAIL, __SavedVars, false);
        }
    }
}
