namespace MetaScriptLang.Data
{
    public class Method
    {
        private List<Variable> methodVariables = new ();
        private List<string> lines = new ();

        private string logicOperatorValue= string.Empty,
            methodName = string.Empty,
            objectName = string.Empty,
            symbolString = string.Empty,
            valueOne_ = string.Empty,
            valueTwo_ = string.Empty;

        private bool isForLoop_ = false,
            isIF_ = false,
            isIndestructible = false,
            isInfinite_ = false,
            isListLoop_ = false,
            isPrivate_ = false,
            isPublic_ = false,
            isTemplate_ = false,
            isWhileLoop_ = false;

        private int startValue = 0,
            stopValue = 0,
            templateObjects = 0;

        private List list = new ();

        private SwitchCase nest = new ();

        private char defaultSymbol;

        public Method() { }

        public Method(string name)
        {
            Initialize(name);
        }

        public Method(string name, bool isTemplate)
        {
            Initialize(name);
            isTemplate_ = isTemplate;
        }

        public void SetObject(string name)
        {
            objectName = name;
        }

        public void Lock()
        {
            isIndestructible = true;
        }

        public bool IsLocked()
        {
            return isIndestructible;
        }

        public void Unlock()
        {
            isIndestructible = false;
        }

        public string GetObject()
        {
            return objectName;
        }

        /**
         * symbol is the variable containing the current iteration value.
         */
        public void SetSymbol(string symbol)
        {
            symbolString = symbol;
        }

        /**
         * symbol is the variable containing the current iteration value.
         */
        public void SetDefaultSymbol(string symbol)
        {
            defaultSymbol = symbol[0];
        }

        public string GetSymbol()
        {
            return symbolString;
        }

        public char GetDefaultSymbol()
        {
            return defaultSymbol;
        }

        public void SetPrivate()
        {
            isPrivate_ = true;
            isPublic_ = false;
        }

        public void SetPublic()
        {
            isPublic_ = true;
            isPrivate_ = false;
        }

        public bool IsPublic()
        {
            return isPublic_;
        }

        public List<Variable> GetVariables()
        {
            return methodVariables;
        }

        public void SetName(string name)
        {
            methodName = name;
        }

        public void AddLine(string line)
        {
            lines.Add(line);
        }

        public void AddVariable(string value, string variableName)
        {
            Variable newVariable = new (variableName, value);
            methodVariables.Add(newVariable);
        }

        public void AddVariable(double value, string variableName)
        {
            Variable newVariable = new (variableName, value);
            methodVariables.Add(newVariable);
        }

        public void AddVariable(Variable variable)
        {
            methodVariables.Add(variable);
        }

        public string GetLine(int index)
        {
            if (index < lines.Count)
                return lines[index];
            return "#!=no_line";
        }

        public void BuildNest()
        {
            SwitchCase newNest = new ();
            nest = newNest;
        }

        public SwitchCase GetNest()
        {
            return (nest);
        }

        public void AddToNest(string line)
        {
            nest.Add(line);
        }

        public string GetNestLine(int index)
        {
            if (index < nest.Count)
                return (nest[index]);
            else
                return "nothing!!!";
        }

        public List<string> GetLines()
        {
            return lines;
        }

        public void Initialize(string name)
        {
            defaultSymbol = '$';
            logicOperatorValue = string.Empty;
            methodName = name;
            objectName = string.Empty;
            symbolString = "$";
            valueOne_ = string.Empty;
            valueTwo_ = string.Empty;
            isForLoop_ = false;
            isIF_ = false;
            isIndestructible = false;
            isInfinite_ = false;
            isListLoop_ = false;
            isPublic_ = false;
            isPrivate_ = false;
            isTemplate_ = false;
            isWhileLoop_ = false;
            startValue = 0;
            stopValue = 0;
            templateObjects = 0;
        }

        public bool IsBad()
        {
            return methodName.StartsWith("[bad_meth");
        }

        public string GetName()
        {
            return methodName;
        }

        public void SetTemplateSize(int size)
        {
            templateObjects = size;
        }

        public int GetTemplateSize()
        {
            return templateObjects;
        }

        public int GetMethodSize()
        {
            return lines.Count;
        }

        public bool IsIfStatement()
        {
            return isIF_;
        }

        public bool IsForLoop()
        {
            return isForLoop_;
        }

        public bool IsWhileLoop()
        {
            return isWhileLoop_;
        }

        public bool IsInfinite()
        {
            return isInfinite_;
        }

        public int Start()
        {
            return startValue;
        }

        public int Stop()
        {
            return stopValue;
        }

        public string FirstValue()
        {
            return valueOne_;
        }

        public string SecondValue()
        {
            return valueTwo_;
        }

        public string LogicalOperator()
        {
            return logicOperatorValue;
        }

        public void SetInfinite()
        {
            isInfinite_ = true;
        }

        public void SetIsIfStatement(bool b)
        {
            isIF_ = b;
        }

        public void SetIsForLoop(bool b)
        {
            isForLoop_ = b;
        }

        public void SetIsWhileLoop(bool b)
        {
            isWhileLoop_ = b;
        }

        public void SetWhileLoopValues(string v1, string op, string v2)
        {
            valueOne_ = v1;
            logicOperatorValue = op;
            valueTwo_ = v2;
        }

        public void SetForLoopValues(int a, int b)
        {
            startValue = a;
            stopValue = b;
        }

        public void SetForListLoop(List l)
        {
            list = l;
        }

        public bool IsListLoop()
        {
            return isListLoop_;
        }

        public void SetListLoop()
        {
            isListLoop_ = true;
        }

        public List GetList()
        {
            return list;
        }
    }
}
