namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Helpers;

    public partial class Parser
    {
        void SaveVariable(string variableName)
        {
            System.IO.File.AppendAllText(__SavedVars, CryptoHelper.Encrypt(variableName) + Environment.NewLine);
        }
    }
}
