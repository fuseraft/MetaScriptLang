namespace MetaScriptLang.Data
{
    public class Script
    {
        private System.Collections.Generic.List<string> lines = new();
        private System.Collections.Generic.List<string> marks = new();
        private string scriptName = string.Empty;

        public Script() { }

        public Script(string name)
        {
            scriptName = name;
        }

        ~Script()
        {
            clear();
        }

        public void clear()
        {
            lines.Clear();
            marks.Clear();
        }

        public void add(string line)
        {
            lines.Add(line);
        }

        public void addMark(string mark)
        {
            marks.Add(mark);
        }

        System.Collections.Generic.List<string> get()
        {
            return lines;
        }

        public string at(int index)
        {
            if (index < lines.Count)
                return lines[index];

            return "[no_line]";
        }

        public string markAt(int index)
        {
            if (index < marks.Count)
                return marks[index];

            return "[no_line]";
        }

        public bool markExists(string mark)
        {
            for (int i = 0; i < marks.Count; i++)
            {
                if (marks[i] == mark)
                    return true;
            }

            return false;
        }

        public int markSize()
        {
            return marks.Count;
        }

        public int size()
        {
            return lines.Count;
        }

        public string name()
        {
            return scriptName;
        }
    }
}
