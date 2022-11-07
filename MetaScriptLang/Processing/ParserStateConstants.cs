namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        bool IsCNumber(string constantName)
        {
            return this.constants[constantName].ConstNumber();
        }

        bool IsCString(string constantName)
        {
            return this.constants[constantName].ConstString();
        }

        double GetCNumber(string constantName)
        {
            return this.constants[constantName].getNumber();
        }

        string GetCString(string constantName)
        {
            return this.constants[constantName].getString();
        }

        bool CExists(string constantName)
        {
            return this.constants.ContainsKey(constantName);
        }
    }
}
