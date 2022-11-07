namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        bool ModExists(string moduleName)
        {
            return this.modules.ContainsKey(moduleName);
        }

        void ModAddLine(string moduleName, string line)
        {
            this.modules[moduleName].add(line);
        }

        System.Collections.Generic.List<string> ModGetLines(string moduleName)
        {
            return this.modules[moduleName].get();
        }
    }
}
