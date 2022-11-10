namespace MetaScriptLang.Data
{
    public class Script
    {
        private System.Collections.Generic.List<string> lines = new();
        private System.Collections.Generic.List<string> marks = new();
        private string scriptName = string.Empty;

        public Script(string name)
        {
            scriptName = name;
        }

        public void AddLine(string line)
        {
            lines.Add(line);
        }

        public void AddMark(string mark)
        {
            marks.Add(mark);
        }

        public string GetLineAt(int index)
        {
            if (index < lines.Count)
                return lines[index];

            return "[no_line]";
        }

        public bool MarkExists(string mark)
        {
            for (int i = 0; i < marks.Count; i++)
            {
                if (marks[i] == mark)
                    return true;
            }

            return false;
        }

        public int GetSize()
        {
            return lines.Count;
        }
    }
}
