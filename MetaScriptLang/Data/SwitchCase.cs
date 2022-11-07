namespace MetaScriptLang.Data
{
    public class SwitchCase
    {
        System.Collections.Generic.List<string> lines = new();

        public SwitchCase()
        {
        }

        public string Value { get; set; } = string.Empty;

        public int Count => this.lines.Count;

        public string this[int index]
        {
            get => index < this.lines.Count ? this.lines[index] : "#!=no_line";
            set => this.lines[index] = value;
        }

        public void Add(string line)
        {
            this.lines.Add(line);
        }

        public void Clear()
        {
            this.lines.Clear();
        }
    }
}
