namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;

    public partial class Parser
    {
        #region GC
        bool GCCanCollectV(string target)
        {
            return this.variables[target].garbage();
        }
        #endregion

        #region Creation
        void DeleteV(string target)
        {
            this.variables.Remove(target);
        }

        void CreateVString(string name, string value)
        {
            Variable newVariable = new(name, value);

            if (__ExecutedTemplate || __ExecutedMethod || __ExecutedTryBlock)
                newVariable.collect();
            else
                newVariable.dontCollect();

            variables.Add(name, newVariable);
            setLastValue(value);
        }

        void CreateVNumber(string name, double value)
        {
            Variable newVariable = new(name, value);

            if (__ExecutedTemplate || __ExecutedMethod || __ExecutedTryBlock)
                newVariable.collect();
            else
                newVariable.dontCollect();

            variables.Add(name, newVariable);
            setLastValue(dtos(value));
        }
        #endregion

        #region Existence
        bool VExists(string s)
        {
            if (zeroDots(s))
            {
                if (this.variables.ContainsKey(s))
                {
                    return true;
                }
            }
            else
            {
                string before = beforeDot(s), after = afterDot(s);

                if (objectExists(before))
                {
                    if (objects[indexOfObject(before)].variableExists(after))
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
        Variable GetV(string target)
        {
            if (VExists(target))
            {
                return this.variables[target];
            }

            __BadVarCount++;
            return new Variable($"[bad_var#{itos(__BadVarCount)}]");
        }

        string GetVString(string target)
        {
            return this.variables[target].getString();
        }

        double GetVNumber(string target)
        {
            return this.variables[target].getNumber();
        }

        string GetVName(string target)
        {
            return this.variables[target].name();
        }

        bool GetVWaiting(string target)
        {
            return this.variables[target].waiting();
        }
        #endregion

        #region Setters
        void SetVString(string target, string value)
        {
            this.variables[target].setVariable(value);
            setLastValue(value);
        }

        void SetVNumber(string target, double value)
        {
            if (isString(target))
                this.variables[target].setVariable(dtos(value));
            else if (isNumber(target))
                this.variables[target].setVariable(value);
            else
            {
                if (GetVWaiting(target))
                    SetVStopWait(target);

                this.variables[target].setVariable(value);
            }

            setLastValue(dtos(value));
        }

        void SetVName(string target, string newName)
        {
            this.variables[target].setName(newName);
        }

        void SetVLock(string target)
        {
            this.variables[target].setIndestructible();
        }

        void SetVUnlock(string target)
        {
            this.variables[target].setDestructible();
        }

        void SetVNull(string target)
        {
            this.variables[target].setNull();
        }

        void SetVStopWait(string target)
        {
            this.variables[target].stopWait();
        }
        #endregion
    }
}
