namespace MetaScriptLang.Data
{
    public class Switch
    {
        private System.Collections.Generic.Dictionary<string, SwitchCase> cases = new();
        private SwitchCase defaultCase = new();
        private string currentCaseKey = string.Empty;

        public Switch()
        {
        }

        public void Clear()
        {
            cases.Clear();
            defaultCase.Clear();
        }

        public SwitchCase FindSwitchCase(string value)
        {
            return cases.ContainsKey(value) ? cases[value] : defaultCase;
        }

        public void AddToCurrentSwitchCase(string line)
        {
            cases[currentCaseKey].Add(line);
        }

        public void AddToDefaultSwitchCase(string line)
        {
            defaultCase.Add(line);
        }

        public void CreateSwitchCase(string value)
        {
            currentCaseKey = value;
            cases.Add(value, new()
            {
                Value = value,
            });
        }
    }
}
