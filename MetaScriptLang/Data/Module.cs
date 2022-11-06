namespace MetaScriptLang.Data
{
    public class Module
    {
        private System.Collections.Generic.List<string> lines = new();
        private string moduleName = string.Empty;

        public Module() { }

        public Module(string name)
        {
            moduleName = name;
        }

        ~Module()
        {
            this.clear();
        }

        public void clear()
        {
            lines.Clear();
        }

        public void add(string line)
        {
            lines.Add(line);
        }

        public System.Collections.Generic.List<string> get()
        {
            return lines;
        }

        public string at(int index)
        {
            if (index < lines.Count)
                return lines[index];

            return "[no_line]";
        }

        public int size()
        {
            return lines.Count;
        }

        public string name()
        {
            return moduleName;
        }
    }
}
