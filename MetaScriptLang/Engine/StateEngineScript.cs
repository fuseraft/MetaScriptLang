namespace MetaScriptLang.Engine
{
    public partial class StateEngine
    {
        public bool ScriptMarkExists(string scriptName, string mark)
        {
            return this.scripts[scriptName].MarkExists(mark);
        }

        public void SetScriptMark(string scriptName, string mark)
        {
            this.scripts[scriptName].AddMark(mark);
        }

        public string GetLineFromScript(string scriptName, int lineNumber)
        {
            return this.scripts[scriptName].GetLineAt(lineNumber);
        }

        public int GetScriptSize(string scriptName)
        {
            return this.scripts[scriptName].GetSize();
        }
    }
}
