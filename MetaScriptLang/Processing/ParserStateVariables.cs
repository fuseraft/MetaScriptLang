namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;

    public partial class Parser
    {
        #region Existence
        bool VariableExists(string s)
        {
            if (StringHelper.ZeroDots(s))
            {
                if (this.variables.ContainsKey(s))
                {
                    return true;
                }
            }
            else
            {
                string before = StringHelper.BeforeDot(s), after = StringHelper.AfterDot(s);

                if (engine.ObjectExists(before))
                {
                    if (engine.ObjectMethodExists(before, after))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }

            return false;
        }
        #endregion

        #region Getters
        string GetVariableString(string target)
        {
            return this.variables[target].getString();
        }

        double GetVariableNumber(string target)
        {
            return this.variables[target].getNumber();
        }
        #endregion
    }
}
