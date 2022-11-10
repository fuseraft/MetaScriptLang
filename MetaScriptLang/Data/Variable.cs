using System.Reflection.Metadata.Ecma335;

namespace MetaScriptLang.Data
{
    public class Variable
    {
        private double numericValue = -Double.MaxValue;
        private string stringValue = string.Empty, variableName = string.Empty;

        private bool autoCollect = false,
            isPrivate_ = false,
            isPublic_ = false,
            isLocked = false,
            waitToAssign = false;

        public Variable(string name, bool autoCollect = false)
        {
            Initialize(name, autoCollect);
            SetAll(-Double.MaxValue, "[null]");
        }

        public Variable(string name, string value, bool autoCollect = false)
            : this(name, autoCollect)
        {
            if (value == "null")
            {
                SetAll(-Double.MaxValue, "[null]");
                waitToAssign = true;
            }
            else
                SetAll(-Double.MaxValue, value);
        }

        public Variable(string name, double value, bool autoCollect = false)
            : this(name, autoCollect)
        {
            SetAll(value, "[null]");
        }

        public bool CanCollect => this.autoCollect;

        public bool IsLocked => this.isLocked;

        public bool IsNull => this.StringValue == "[null]" && this.NumberValue == -Double.MaxValue;

        public string StringValue { get; set; }

        public double NumberValue { get; set; }

        private void SetAll(double numValue, string strValue, bool autoCollect = false)
        {
            SetAutoCollect(autoCollect);
            SetValue(numValue);
            SetValue(strValue);
        }

        public void SetAutoCollect(bool autoCollect)
        {
            this.autoCollect = autoCollect;
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

        public void Initialize(string name, bool autoCollect = false)
        {
            variableName = name;
            this.autoCollect = false;
            isLocked = false;
            waitToAssign = false;
        }

        public void Lock()
        {
            isLocked = true;
        }

        public void Unlock()
        {
            isLocked = false;
        }
    }
}
