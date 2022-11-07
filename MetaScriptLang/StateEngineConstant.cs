namespace MetaScriptLang.Engine
{
    public partial class StateEngine
    {
        public bool IsNumberConstant(string constantName)
        {
            return this.constants[constantName].ConstNumber();
        }

        public bool IsStringConstant(string constantName)
        {
            return this.constants[constantName].ConstString();
        }

        public double GetConstantNumber(string constantName)
        {
            return this.constants[constantName].getNumber();
        }

        public string GetConstantString(string constantName)
        {
            return this.constants[constantName].getString();
        }

        public bool ConstantExists(string constantName)
        {
            return this.constants.ContainsKey(constantName);
        }
    }
}
