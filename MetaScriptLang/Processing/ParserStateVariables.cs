﻿namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;

    public partial class Parser
    {
        #region GC
        bool GCCanCollectVariable(string target)
        {
            return this.variables[target].garbage();
        }
        #endregion

        #region Engine
        public List<string> GetVariableKeys()
        {
            return this.lists.Keys.ToList();
        }

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
        void DeleteVariable(string target)
        {
            this.variables.Remove(target);
        }

        void CreateVariableString(string name, string value)
        {
            Variable newVariable = new(name, value);

            if (__ExecutedTemplate || __ExecutedMethod || __ExecutedTryBlock)
                newVariable.collect();
            else
                newVariable.dontCollect();

            variables.Add(name, newVariable);
            SetLastValue(value);
        }

        void CreateVariableNumber(string name, double value)
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
        string GetVariableString(string target)
        {
            return this.variables[target].getString();
        }

        double GetVariableNumber(string target)
        {
            return this.variables[target].getNumber();
        }

        string GetVariableName(string target)
        {
            return this.variables[target].name();
        }

        bool VariableWaiting(string target)
        {
            return this.variables[target].waiting();
        }
        #endregion

        #region Setters
        void SetVariableString(string target, string value)
        {
            this.variables[target].setVariable(value);
            SetLastValue(value);
        }

        void SetVariableNumber(string target, double value)
        {
            if (IsStringVariable(target))
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

        void SetVariableName(string target, string newName)
        {
            this.variables[target].setName(newName);
        }

        void LockVariable(string target)
        {
            this.variables[target].setIndestructible();
        }

        void UnlockVariable(string target)
        {
            this.variables[target].setDestructible();
        }

        void SetVariableNull(string target)
        {
            this.variables[target].setNull();
        }

        void StopWaitVariable(string target)
        {
            this.variables[target].stopWait();
        }
        #endregion
    }
}
