namespace MetaScriptLang.Engine
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;

    public partial class StateEngine
    {
        private string CurrentMethodName = string.Empty;

        void DeleteMethod(string target)
        {
            this.methods.Remove(target);
        }

        #region Existence
        bool MethodExists(string s)
        {
            if (StringHelper.ZeroDots(s))
            {
                foreach (var key in this.methods.Keys)
                {
                    if (methods[key].GetName() == s)
                        return true;
                }
            }
            else if (ObjectExists(StringHelper.BeforeDot(s)) && ObjectMethodExists(StringHelper.BeforeDot(s), StringHelper.AfterDot(s)))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Getters
        Method CreateMethod(string name)
        {
            CurrentMethodName = name;
            return new Method(name);
        }

        Method GetMethod(string target)
        {
            if (methods.ContainsKey(target))
            {
                return methods[target];
            }

            __BadMethodCount++;
            return new Method($"[bad_meth#{StringHelper.ItoS(__BadMethodCount)}]");
        }

        string GetMethodLine(string target, int lineNumber)
        {
            return methods[target].GetLine(lineNumber);
        }

        int GetMethodSize(string target)
        {
            return methods[target].GetMethodSize();
        }
        #endregion

        #region Setters
        void SetMethodName(string target, string newName)
        {
            this.methods[target].SetName(newName);
        }

        void LockMethod(string target)
        {
            this.methods[target].Lock();
        }

        void UnlockMethod(string target)
        {
            this.methods[target].Unlock();
        }
        #endregion
    }
}
