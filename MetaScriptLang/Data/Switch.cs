namespace MetaScriptLang.Data
{
    public class Switch
    {
        private System.Collections.Generic.List<Container> cases = new();
        private Container defaultCase = new();
        private int count = 0;

        public Switch()
        {
            count = 0;
        }

        ~Switch()
        {
            this.clear();
        }

        public void clear()
        {
            cases.Clear();
            defaultCase.clear();
            count = 0;
        }

        public Container rightCase(string value)
        {
            for (int i = 0; i < cases.Count; i++)
            {
                if (cases[i].getCase() == value)
                    return cases[i];
            }

            return defaultCase;
        }

        public void addCase(string value)
        {
            Container newCase = new ($"[case#{count}]");
            newCase.setValue(value);
            cases.Add(newCase);
            count++;
        }

        public void addToCase(string line)
        {
            cases[count - 1].add(line);
        }

        public void addToDefault(string line)
        {
            defaultCase.add(line);
        }
    }
}
