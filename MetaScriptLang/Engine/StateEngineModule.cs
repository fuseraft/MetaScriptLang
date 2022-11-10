namespace MetaScriptLang.Engine
{
    public partial class StateEngine
    {
        public bool ModuleExists(string moduleName)
        {
            return this.modules.ContainsKey(moduleName);
        }

        public void AddLineToModule(string moduleName, string line)
        {
            this.modules[moduleName].add(line);
        }

        public System.Collections.Generic.List<string> GetModuleLines(string moduleName)
        {
            return this.modules[moduleName].get();
        }
    }
}
