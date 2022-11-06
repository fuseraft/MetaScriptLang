namespace MetaScriptLang.Data
{
    public class StringContainer
    {
        private System.Collections.Generic.List<string> strings = new();

        public StringContainer() { }

        ~StringContainer()
        {
            clear();
        }

        public void clear()
        {
            strings.Clear();
        }

        public void add(string line)
        {
            strings.Add(line);
        }

        System.Collections.Generic.List<string> get()
        {
            return strings;
        }

        public int size()
        {
            return strings.Count;
        }

        public string at(int index)
        {
            if (index < strings.Count)
                return strings[index];

            return "[no_line]";
        }
    };

}
