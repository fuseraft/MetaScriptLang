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

        public bool ConstNumber()
        {
            return isNumber_;
        }

        public bool ConstString()
        {
            return isString_;
        }

        public string getString()
        {
            return stringValue;
        }

        public double getNumber()
        {
            return numericValue;
        }

        public string name()
        {
            return constantName;
        }
    }
}
