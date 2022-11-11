namespace MetaScriptLang.Engine
{
    public partial class StateEngine
    {
        public bool IsNumberConstant(string constantName)
        {
            return this.constants[constantName].IsNumber;
        }

        public bool IsStringConstant(string constantName)
        {
            return this.constants[constantName].IsString;
        }

        public double GetConstantNumber(string constantName)
        {
            return this.constants[constantName].NumberValue;
        }

        public string GetConstantString(string constantName)
        {
            return this.constants[constantName].StringValue;
        }

        public bool ConstantExists(string constantName)
        {
            return this.constants.ContainsKey(constantName);
        }
    }
}
