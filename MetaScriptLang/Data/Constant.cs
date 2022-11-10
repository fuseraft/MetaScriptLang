namespace MetaScriptLang.Data
{
    public class Constant
    {
        private string stringValue = string.Empty, constantName = string.Empty;
        private double numericValue = -Double.MaxValue;
        private bool isNumber_ = false, isString_ = false;

        public Constant(string name, string val)
        {
            constantName = name;
            stringValue = val;
            isString_ = true;
            isNumber_ = false;
        }

        public Constant(string name, double val)
        {
            constantName = name;
            numericValue = val;
            isNumber_ = true;
            isString_ = false;
        }

        public string StringValue => stringValue;

        public double NumberValue => numericValue;

        public string Name => constantName;

        public bool IsNumber => isNumber_;

        public bool IsString => isString_;
    }
}
