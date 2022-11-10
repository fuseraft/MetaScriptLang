namespace MetaScriptLang.Data
{
    public class List
    {
        private System.Collections.Generic.List<string> contents = new ();
        private System.Collections.Generic.List<string> reversion = new ();
        private string listName = String.Empty;
        private bool autoCollect = false;

        public List() { }

        public List(string name)
        {
            autoCollect = false;
            listName = name;
        }

        ~List()
        {
            Clear();
        }

        public bool CanCollect => this.autoCollect;

        public void SetAutoCollect(bool autoCollect)
        {
            this.autoCollect = autoCollect;
        }

        public void SetName(string s)
        {
            listName = s;
        }

        public void Sort()
        {
            reversion = new System.Collections.Generic.List<string>(contents);
            contents.Sort();
        }

        public void Reverse()
        {
            reversion = new System.Collections.Generic.List<string>(contents);
            contents.Reverse();
        }

        public void Revert()
        {
            contents = reversion;
        }

        public void Add(string line)
        {
            contents.Add(line);
        }

        public void RemoveAll(string line)
        {
            contents.RemoveAll(s => (s ?? String.Empty).Equals(line));
        }

        public void Clear()
        {
            contents.Clear();
        }

        public string GetItemAt(int index)
        {
            if (index < contents.Count)
                return contents[index];

            return "#!=no_line";
        }

        public string GetName()
        {
            return listName;
        }

        public int GetSize()
        {
            return contents.Count;
        }
    }
}
