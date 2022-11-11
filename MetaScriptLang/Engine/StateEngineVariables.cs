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
            return var.NumberValue != __NullNum;
        }

        public bool IsNumberVariable(string varName)
        {
            return GetVariableNumber(varName) != __NullNum;
        }

        public bool IsStringVariable(Variable var)
        {
            return var.StringValue != __Null;
        }

        public bool IsStringVariable(string varName)
        {
            return GetVariableString(varName) != __Null;
        }
        #endregion

        #region GC
        public bool GCCanCollectVariable(string target)
        {
            return this.variables[target].AutoCollect;
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
            return Variable.Create($"[bad_var#{StringHelper.ItoS(__BadVarCount)}]");
        }
        #endregion

        #region Creation
        public void DeleteVariable(string target)
        {
            this.variables.Remove(target);
        }

        public void CreateStringVariable(string name, string value)
        {
            Variable newVariable = Variable.Create(name, value, __ExecutedTemplate || __ExecutedMethod || __ExecutedTryBlock);
            variables.Add(name, newVariable);
            SetLastValue(value);
        }

        public void CreateNumberVariable(string name, double value)
        {
            Variable newVariable = Variable.Create(name, value, __ExecutedTemplate || __ExecutedMethod || __ExecutedTryBlock);
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
            return this.variables[target].StringValue;
        }

        public double GetVariableNumber(string target)
        {
            return this.variables[target].NumberValue;
        }

        public string GetVariableName(string target)
        {
            return this.variables[target].Name;
        }

        public bool VariableWaiting(string target)
        {
            return this.variables[target].WaitingForAssignment;
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
            this.variables[target].Name = newName;
        }

        public void LockVariable(string target)
        {
            this.variables[target].Locked = true;
        }

        public void UnlockVariable(string target)
        {
            this.variables[target].Locked = false;
        }

        public void SetVariableNull(string target)
        {
            this.variables[target].Nullify();
        }

        public void StopWaitVariable(string target)
        {
            this.variables[target].WaitingForAssignment = false;
        }
        #endregion
    }
}
