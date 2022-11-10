namespace MetaScriptLang.Data
{
    public class Variable
    {
        private double numericValue = -Double.MaxValue;
        private string stringValue = string.Empty, variableName = string.Empty;

        private bool collectable = false,
            isPrivate_ = false,
            isPublic_ = false,
            isIndestructible = false,
            waitToAssign = false;

        private void SetAll(double numValue, string strValue)
        {
            SetValue(numValue);
            SetValue(strValue);
            collectable = false;
        }

        public Variable()
        {
            SetAll(-Double.MaxValue, "[null]");
        }

        public Variable(string name)
        {
            Initialize(name);
            SetAll(-Double.MaxValue, "[null]");
        }

        public Variable(string name, string value)
        {
            Initialize(name);

            if (value == "null")
            {
                SetAll(-Double.MaxValue, "[null]");
                waitToAssign = true;
            }
            else
                SetAll(-Double.MaxValue, value);
        }

        public Variable(string name, double value)
        {
            Initialize(name);
            SetAll(value, "[null]");
        }

        ~Variable() { }

        public void Collect()
        {
            collectable = true;
        }

        public void DontCollect()
        {
            collectable = false;
        }

        public bool IsGarbage()
        {
            return collectable;
        }

        public void SetNull()
        {
            SetAll(-Double.MaxValue, "[null]");
            waitToAssign = true;
        }

        public void SetName(string name)
        {
            variableName = name;
        }

        public string SetName()
        {
            return variableName;
        }

        public bool StartWaitingForAssignment()
        {
            return (waitToAssign);
        }

        public void StopWaitingForAssignment()
        {
            waitToAssign = false;
        }

        public void SetValue(double value)
        {
            if (StartWaitingForAssignment())
            {
                numericValue = value;
                stringValue = "[null]";
                waitToAssign = false;
            }
            else
            {
                numericValue = 0.0;
                numericValue = value;
            }
        }

        public void SetValue(string value)
        {
            if (StartWaitingForAssignment())
            {
                stringValue = value;
                numericValue = -Double.MaxValue;
                waitToAssign = false;
            }
            else
                stringValue = value;
        }

        public void MakePrivate()
        {
            isPrivate_ = true;
            isPublic_ = false;
        }

        public void MakePublic()
        {
            isPublic_ = true;
            isPrivate_ = false;
        }

        public double GetNumberValue()
        {
            return (numericValue);
        }

        public string GetStringValue()
        {
            return (stringValue);
        }

        public void Initialize(string name)
        {
            variableName = name;
            collectable = false;
            isIndestructible = false;
            waitToAssign = false;
        }

        public void Lock()
        {
            isIndestructible = true;
        }

        public void Unlock()
        {
            isIndestructible = false;
        }

        public bool IsIndestructible()
        {
            return isIndestructible;
        }

        public bool IsNull()
        {
            return GetStringValue() == "[null]" && GetNumberValue() == -Double.MaxValue;
        }
    }
}
