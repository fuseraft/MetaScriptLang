using MetaScriptLang.Data;

namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        private string CurrentMethodName = string.Empty;

        void DeleteM(string target)
        {
            this.methods.Remove(target);
        }

        #region Existence
        bool MExists(string s)
        {
            if (zeroDots(s))
            {
                foreach (var key in this.methods.Keys)
                {
                    if (methods[key].GetName() == s)
                        return true;
                }
            }
            else
            {
                if (objectExists(beforeDot(s)))
                {
                    if (objects[indexOfObject(beforeDot(s))].methodExists(afterDot(s)))
                        return true;
                    else
                        return false;
                }
            }

            return false;
        }
        #endregion

        #region Getters
        Method GetM(string target)
        {
            if (methods.ContainsKey(target))
            {
                return methods[target];
            }

            __BadMethodCount++;
            return new Method($"[bad_meth#{itos(__BadMethodCount)}]");
        }

        string GetMLine(string target, int lineNumber)
        {
            return methods[target].GetLine(lineNumber);
        }

        int GetMSize(string target)
        {
            return methods[target].GetMethodSize();
        }
        #endregion

        #region Setters
        void SetMName(string target, string newName)
        {
            this.methods[target].SetName(newName);
        }

        void SetMLock(string target)
        {
            this.methods[target].Lock();
        }

        void SetMUnlock(string target)
        {
            this.methods[target].Unlock();
        }
        #endregion
    }
}
