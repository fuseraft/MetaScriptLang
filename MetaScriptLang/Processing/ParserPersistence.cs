namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    public partial class Parser
    {
        void SaveVariable(string variableName)
        {
            Crypt c = new();

            System.IO.File.AppendAllText(__SavedVars, c.e(variableName) + Environment.NewLine);
        }
    }
}
