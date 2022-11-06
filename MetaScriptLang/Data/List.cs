namespace MetaScriptLang.Data
{
    public class List
    {
        private System.Collections.Generic.List<string> contents = new ();
        private System.Collections.Generic.List<string> reversion = new ();
        private string listName = String.Empty;
        private bool collectable = false;

        public List() { }

        public List(string name)
        {
            collectable = false;
            listName = name;
        }

        ~List()
        {
            clear();
        }

        public void collect()
        {
            collectable = true;
        }

        public void dontCollect()
        {
            collectable = false;
        }

        public bool garbage()
        {
            return collectable;
        }

        public void setName(string s)
        {
            listName = s;
        }

        public void listSort()
        {
            reversion = new System.Collections.Generic.List<string>(contents);
            contents.Sort();
        }

        public void listReverse()
        {
            reversion = new System.Collections.Generic.List<string>(contents);
            contents.Reverse();
        }

        public void listRevert()
        {
            contents = reversion;
        }

        public void add(string line)
        {
            contents.Add(line);
        }

        public void remove(string line)
        {
            contents.RemoveAll(s => (s ?? String.Empty).Equals(line));
        }

        public void clear()
        {
            contents.Clear();
        }

        public string at(int index)
        {
            if (index < contents.Count)
                return contents[index];

            return "#!=no_line";
        }

        public string name()
        {
            return listName;
        }

        public int size()
        {
            return contents.Count;
        }
    }
}
