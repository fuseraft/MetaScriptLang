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
            return var.GetNumberValue() != __NullNum;
        }

        public bool IsNumberVariable(string varName)
        {
            return GetVariableNumber(varName) != __NullNum;
        }

        public bool IsStringVariable(Variable var)
        {
            return var.GetStringValue() != __Null;
        }

        public bool IsStringVariable(string varName)
        {
            return GetVariableString(varName) != __Null;
        }
        #endregion

        #region GC
        public bool GCCanCollectVariable(string target)
        {
            return this.variables[target].IsGarbage();
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
                newVariable.Collect();
            else
                newVariable.DontCollect();

            variables.Add(name, newVariable);
            SetLastValue(value);
        }

        public void CreateVariableNumber(string name, double value)
        {
            Variable newVariable = new(name, value);

            if (__ExecutedTemplate || __ExecutedMethod || __ExecutedTryBlock) 
                newVariable.Collect();
            else
                newVariable.DontCollect();

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
            return this.variables[target].GetStringValue();
        }

        public double GetVariableNumber(string target)
        {
            return this.variables[target].GetNumberValue();
        }

        public string GetVariableName(string target)
        {
            return this.variables[target].SetName();
        }

        public bool VariableWaiting(string target)
        {
            return this.variables[target].StartWaitingForAssignment();
        }
        #endregion

        #region Setters
        public void SetVariableString(string target, string value)
        {
            this.variables[target].SetValue(value);
            SetLastValue(value);
        }

        public void SetVariableNumber(string target, double value)
        {
            if (IsNumberVariable(target))
                this.variables[target].SetValue(StringHelper.DtoS(value));
            else if (IsNumberVariable(target))
                this.variables[target].SetValue(value);
            else
            {
                if (VariableWaiting(target))
                    StopWaitVariable(target);

                this.variables[target].SetValue(value);
            }

            SetLastValue(StringHelper.DtoS(value));
        }

        public void SetVariableName(string target, string newName)
        {
            this.variables[target].SetName(newName);
        }

        public void LockVariable(string target)
        {
            this.variables[target].Lock();
        }

        public void UnlockVariable(string target)
        {
            this.variables[target].Unlock();
        }

        public void SetVariableNull(string target)
        {
            this.variables[target].SetNull();
        }

        public void StopWaitVariable(string target)
        {
            this.variables[target].StopWaitingForAssignment();
        }
        #endregion
    }
}
