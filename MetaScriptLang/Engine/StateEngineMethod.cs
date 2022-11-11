namespace MetaScriptLang.Engine
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;

    public partial class StateEngine
    {
        public string CurrentMethodName = string.Empty;

        public void DeleteMethod(string target)
        {
            this.methods.Remove(target);
        }

        #region Existence
        public bool MethodExists(string s)
        {
            if (StringHelper.ZeroDots(s))
            {
                foreach (var key in this.methods.Keys)
                {
                    if (this.methods[key].GetName() == s)
                        return true;
                }
            }
            else if (this.ObjectExists(StringHelper.BeforeDot(s)) && this.ObjectMethodExists(StringHelper.BeforeDot(s), StringHelper.AfterDot(s)))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Getters
        public Method CreateMethod(string name)
        {
            this.CurrentMethodName = name;
            return new Method(name);
        }

        public Method GetMethod(string target)
        {
            if (this.methods.ContainsKey(target))
            {
                return this.methods[target];
            }

            __BadMethodCount++;
            return new Method($"[bad_meth#{StringHelper.ItoS(__BadMethodCount)}]");
        }

        public string GetMethodLine(string target, int lineNumber)
        {
            return this.methods[target].GetLine(lineNumber);
        }

        public int GetMethodSize(string target)
        {
            return this.methods[target].GetMethodSize();
        }
        #endregion

        #region Setters
        public void SetMethodName(string target, string newName)
        {
            this.methods[target].SetName(newName);
        }

        public void LockMethod(string target)
        {
            this.methods[target].Lock();
        }

        public void UnlockMethod(string target)
        {
            this.methods[target].Unlock();
        }
        #endregion
    }
}
