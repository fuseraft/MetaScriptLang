namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        bool GetSMarkExists(string scriptName, string mark)
        {
            return this.scripts[scriptName].markExists(mark);
        }

        void SetSMark(string scriptName, string mark)
        {
            this.scripts[scriptName].addMark(mark);
        }

        string GetSLine(string scriptName, int lineNumber)
        {
            return this.scripts[scriptName].at(lineNumber);
        }

        int GetSSize(string scriptName)
        {
            return this.scripts[scriptName].size();
        }
    }
}
