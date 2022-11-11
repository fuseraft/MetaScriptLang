namespace MetaScriptLang.Data
{
    public class StringContainer
    {
        private System.Collections.Generic.List<string> strings = new();

        public StringContainer() { }

        ~StringContainer()
        {
            Clear();
        }

        public void Clear()
        {
            strings.Clear();
        }

        public void AddLine(string line)
        {
            strings.Add(line);
        }

        System.Collections.Generic.List<string> GetLines()
        {
            return strings;
        }

        public int GetSize()
        {
            return strings.Count;
        }

        public string GetLineAt(int index)
        {
            if (index < strings.Count)
                return strings[index];

            return "[no_line]";
        }
    };

}
