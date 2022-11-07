namespace MetaScriptLang.Engine
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;

    public partial class StateEngine
    {
        public List<string> GetVariableKeys()
        {
            return this.lists.Keys.ToList();
        }

        #region Typechecking
        public bool IsNumberVariable(Variable var)
        {
            return var.getNumber() != __NullNum;
        }

        public bool IsNumberVariable(string varName)
        {
            return GetVariableNumber(varName) != __NullNum;
        }

        public bool IsStringVariable(Variable var)
        {
            return var.getString() != __Null;
        }

        public bool IsStringVariable(string varName)
        {
            return GetVariableString(varName) != __Null;
        }
        #endregion

        #region GC
        public bool GCCanCollectVariable(string target)
        {
            return this.variables[target].garbage();
        }
        #endregion

        #region Engine
        public Variable GetVariable(string target)
        {
            if (VariableExists(target))
            {
                return this.variables[target];
            }

            __BadVarCount++;
            return new Variable($"[bad_var#{StringHelper.ItoS(__BadVarCount)}]");
        }
        #endregion

        #region Creation
        public void DeleteVariable(string target)
        {
            this.variables.Remove(target);
        }

        public void CreateVariableString(string name, string value)
        {
            Variable newVariable = new(name, value);

            if (__ExecutedTemplate || __ExecutedMethod || __ExecutedTryBlock)
                newVariable.collect();
            else
                newVariable.dontCollect();

            variables.Add(name, newVariable);
            SetLastValue(value);
        }

        public void CreateVariableNumber(string name, double value)
        {
            Variable newVariable = new(name, value);

            if (__ExecutedTemplate || __ExecutedMethod || __ExecutedTryBlock) 
                newVariable.collect();
            else
                newVariable.dontCollect();

            variables.Add(name, newVariable);
            SetLastValue(StringHelper.DtoS(value));
        }
        #endregion

        #region Existence
        public bool VariableExists(string s)
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

                if (this.ObjectExists(before))
                {
                    if (ObjectMethodExists(before, after))
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
        public string GetVariableString(string target)
        {
            return this.variables[target].getString();
        }

        public double GetVariableNumber(string target)
        {
            return this.variables[target].getNumber();
        }

        public string GetVariableName(string target)
        {
            return this.variables[target].name();
        }

        public bool VariableWaiting(string target)
        {
            return this.variables[target].waiting();
        }
        #endregion

        #region Setters
        public void SetVariableString(string target, string value)
        {
            this.variables[target].setVariable(value);
            SetLastValue(value);
        }

        public void SetVariableNumber(string target, double value)
        {
            if (IsNumberVariable(target))
                this.variables[target].setVariable(StringHelper.DtoS(value));
            else if (IsNumberVariable(target))
                this.variables[target].setVariable(value);
            else
            {
                if (VariableWaiting(target))
                    StopWaitVariable(target);

                this.variables[target].setVariable(value);
            }

            SetLastValue(StringHelper.DtoS(value));
        }

        public void SetVariableName(string target, string newName)
        {
            this.variables[target].setName(newName);
        }

        public void LockVariable(string target)
        {
            this.variables[target].setIndestructible();
        }

        public void UnlockVariable(string target)
        {
            this.variables[target].setDestructible();
        }

        public void SetVariableNull(string target)
        {
            this.variables[target].setNull();
        }

        public void StopWaitVariable(string target)
        {
            this.variables[target].stopWait();
        }
        #endregion
    }
}
