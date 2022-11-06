namespace MetaScriptLang.Data
{
    public class Container
    {
        System.Collections.Generic.List<string> lines = new();
        string containerName = string.Empty, value = string.Empty;
        bool isNestedIF = false;

        public Container() { }

        public Container(string name)
        {
            initialize(name);
        }

        ~Container()
        {
            clear();
        }

        public void setName(string name)
        {
            containerName = name;
        }

        public void add(string line)
        {
            lines.Add(line);
        }

        public void setValue(string val)
        {
            value = val;
        }

        public string getCase()
        {
            return value;
        }

        public string at(int index)
        {
            if (index < lines.Count)
                return lines[index];

            return "#!=no_line";
        }

        public void clear()
        {
            lines.Clear();
        }

        public List<string> getLines()
        {
            return lines;
        }

        public void initialize(string name)
        {
            containerName = name;
            isNestedIF = false;
        }

        public string name()
        {
            return containerName;
        }

        public int size()
        {
            return lines.Count;
        }

        public bool isIF()
        {
            return isNestedIF;
        }

        public void setBool(bool b)
        {
            isNestedIF = b;
        }

        public bool isBad()
        {
            return name().StartsWith("[bad_nest");
        }
    }
}
