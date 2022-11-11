namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        void LoadSavedVariables()
        {
            if (!System.IO.File.Exists(__SavedVars))
            {
                ErrorLogger.Error(ErrorLogger.READ_FAIL, __SavedVars, false);
                return;
            }

            string bigStr = CryptoHelper.Decrypt(System.IO.File.ReadAllText(__SavedVars));
            bool stop = false;
            System.Collections.Generic.List<string> varNames = new();
            System.Collections.Generic.List<string> varValues = new();

            varNames.Add("");
            varValues.Add("");

            for (int i = 0; i < bigStr.Length; i++)
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
                            varNames[varNames.Count - 1] += (bigStr[i]);
                        else
                            varValues[varValues.Count - 1] += (bigStr[i]);
                        break;
                }
            }

            for (int i = 0; i < varNames.Count; i++)
            {
                Variable newVariable = Variable.Create(varNames[i], varValues[i]);
                variables.Add(varNames[i], newVariable);
            }

            varNames.Clear();
            varValues.Clear();
        }
    }
}
