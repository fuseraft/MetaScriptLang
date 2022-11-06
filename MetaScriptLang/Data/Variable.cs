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

        private void setAll(double numValue, string strValue)
        {
            setVariable(numValue);
            setVariable(strValue);
            collectable = false;
        }

        public Variable()
        {
            setAll(-Double.MaxValue, "[null]");
        }

        public Variable(string name)
        {
            initialize(name);
            setAll(-Double.MaxValue, "[null]");
        }

        public Variable(string name, string value)
        {
            initialize(name);

            if (value == "null")
            {
                setAll(-Double.MaxValue, "[null]");
                waitToAssign = true;
            }
            else
                setAll(-Double.MaxValue, value);
        }

        public Variable(string name, double value)
        {
            initialize(name);
            setAll(value, "[null]");
        }

        ~Variable() { }

        public void collect()
        {
            collectable = true;
        }

        public void dontCollect()
        {
            collectable = false;
        }

        public bool garbage()
        {
            return collectable;
        }

        public void clear()
        {
            setAll(0, string.Empty);
        }

        public void setNull()
        {
            setAll(-Double.MaxValue, "[null]");
            waitToAssign = true;
        }

        public void setName(string name)
        {
            variableName = name;
        }

        public bool waiting()
        {
            return (waitToAssign);
        }

        public void stopWait()
        {
            waitToAssign = false;
        }

        public void setVariable(double value)
        {
            if (waiting())
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

        public void setVariable(string value)
        {
            if (waiting())
            {
                stringValue = value;
                numericValue = -Double.MaxValue;
                waitToAssign = false;
            }
            else
                stringValue = value;
        }

        public void setPrivate()
        {
            isPrivate_ = true;
            isPublic_ = false;
        }

        public void setPublic()
        {
            isPublic_ = true;
            isPrivate_ = false;
        }

        public bool isPublic()
        {
            return (isPublic_);
        }

        public bool isPrivate()
        {
            return (isPrivate_);
        }

        public double getNumber()
        {
            return (numericValue);
        }

        public string getString()
        {
            return (stringValue);
        }

        public void initialize(string name)
        {
            variableName = name;
            collectable = false;
            isIndestructible = false;
            waitToAssign = false;
        }

        public void setIndestructible()
        {
            isIndestructible = true;
        }

        public void setDestructible()
        {
            isIndestructible = false;
        }

        public bool indestructible()
        {
            return isIndestructible;
        }

        public bool isNullString()
        {
            return getString() == "[null]" && getNumber() == -Double.MaxValue;
        }

        public bool isNull()
        {
            return getString() == "[null]" && getNumber() == -Double.MaxValue;
        }

        public string name()
        {
            return variableName;
        }
    }
}
