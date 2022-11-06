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

        private Container nest = new ();

        private char defaultSymbol;

        public Method() { }

        public Method(string name)
        {
            initialize(name);
        }

        public Method(string name, bool isTemplate)
        {
            initialize(name);
            isTemplate_ = isTemplate;
        }

        ~Method()
        {
            clear();
        }

        public void setObject(string name)
        {
            objectName = name;
        }

        public void setIndestructible()
        {
            isIndestructible = true;
        }

        public bool indestructible()
        {
            return isIndestructible;
        }

        public void setDestructible()
        {
            isIndestructible = false;
        }

        public string getObject()
        {
            return objectName;
        }

        /**
         * symbol is the variable containing the current iteration value.
         */
        public void setSymbol(string symbol)
        {
            symbolString = symbol;
        }

        /**
         * symbol is the variable containing the current iteration value.
         */
        public void setDefaultSymbol(string symbol)
        {
            defaultSymbol = symbol[0];
        }

        public string getSymbolString()
        {
            return symbolString;
        }

        public char getDefaultSymbol()
        {
            return defaultSymbol;
        }

        public void setPrivate()
        {
            isPrivate_ = true;
            isPublic_ = false;
        }

        public void setPublic()
        {
            isPublic_ = true;
            isPrivate_ = false;
        }

        public bool isPublic()
        {
            return isPublic_;
        }

        public bool isPrivate()
        {
            return isPrivate_;
        }

        public bool isTemplate()
        {
            return isTemplate_;
        }

        public List<Variable> getMethodVariables()
        {
            return methodVariables;
        }

        public void setName(string name)
        {
            methodName = name;
        }

        public void add(string line)
        {
            lines.Add(line);
        }

        public void addMethodVariable(string value, string variableName)
        {
            Variable newVariable = new (variableName, value);
            methodVariables.Add(newVariable);
        }

        public void addMethodVariable(double value, string variableName)
        {
            Variable newVariable = new (variableName, value);
            methodVariables.Add(newVariable);
        }

        public void addMethodVariable(Variable variable)
        {
            methodVariables.Add(variable);
        }

        public string at(int index)
        {
            if (index < lines.Count)
                return lines[index];
            return "#!=no_line";
        }

        public void buildNest()
        {
            Container newNest = new ($"{methodName}<nest>");
            nest = newNest;
        }

        public Container getNest()
        {
            return (nest);
        }

        public void inNest(string line)
        {
            nest.add(line);
        }

        public string nestAt(int index)
        {
            if (index < nest.size())
                return (nest.at(index));
            else
                return "nothing!!!";
        }

        public void clear()
        {
            lines.Clear();
            methodVariables.Clear();
        }

        public List<string> getLines()
        {
            return lines;
        }

        public void initialize(string name)
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

        public bool isBad()
        {
            return name().StartsWith("[bad_meth");
        }

        public string name()
        {
            return methodName;
        }

        public void setTemplateSize(int size)
        {
            templateObjects = size;
        }

        public int getTemplateSize()
        {
            return templateObjects;
        }

        public int size()
        {
            return lines.Count;
        }

        public bool isIF()
        {
            return isIF_;
        }

        public bool isForLoop()
        {
            return isForLoop_;
        }

        public bool isWhileLoop()
        {
            return isWhileLoop_;
        }

        public bool isInfinite()
        {
            return isInfinite_;
        }

        public int start()
        {
            return startValue;
        }

        public int stop()
        {
            return stopValue;
        }

        public string valueOne()
        {
            return valueOne_;
        }

        public string valueTwo()
        {
            return valueTwo_;
        }

        public string logicOperator()
        {
            return logicOperatorValue;
        }

        public void setInfinite()
        {
            isInfinite_ = true;
        }

        public void setBool(bool b)
        {
            isIF_ = b;
        }

        public void setFor(bool b)
        {
            isForLoop_ = b;
        }

        public void setWhile(bool b)
        {
            isWhileLoop_ = b;
        }

        public void setWhileValues(string v1, string op, string v2)
        {
            valueOne_ = v1;
            logicOperatorValue = op;
            valueTwo_ = v2;
        }

        public void setForValues(int a, int b)
        {
            startValue = a;
            stopValue = b;
        }

        public void setForList(List l)
        {
            list = l;
        }

        public bool isListLoop()
        {
            return isListLoop_;
        }

        public void setListLoop()
        {
            isListLoop_ = true;
        }

        public List getList()
        {
            return list;
        }
    }
}
