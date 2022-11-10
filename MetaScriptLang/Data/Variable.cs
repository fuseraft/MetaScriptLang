namespace MetaScriptLang.Data
{
    public class Variable
    {
        private Variable(string name, string stringValue = "[null]", double numericValue = -Double.MaxValue, bool autoCollect = false)
        {
            this.Name = name;
            this.AutoCollect = autoCollect;
            this.Locked = false;
            this.WaitingForAssignment = false;

            if (stringValue == "null")
            {
                SetAll(-Double.MaxValue, "[null]");
                this.WaitingForAssignment = true;
            }
            else
            {
                SetAll(numericValue, stringValue);
            }
        }

        public static Variable Create(string name)
        {
            return new Variable(name);
        }

        public static Variable Create(string name, string value, bool autoCollect = false)
        {
            return new Variable(name, value, -Double.MaxValue, autoCollect);
        }

        public static Variable Create(string name, double value, bool autoCollect = false)
        {
            return new Variable(name, "[null]", value, autoCollect);
        }

        //public Variable(string name, string value, bool autoCollect = false)
        //    : this(name, value, -Double.MaxValue, autoCollect)
        //{
        //}

        //public Variable(string name, double value, bool autoCollect = false)
        //    : this(name, "[null]", value, autoCollect)
        //{
        //}

        public bool AutoCollect { get; set; }

        public bool Locked { get; set; }

        public bool Null => this.StringValue == "[null]" && this.NumberValue == -Double.MaxValue;

        public string StringValue { get; set; }

        public double NumberValue { get; set; }

        public string Name { get; set; }

        public bool WaitingForAssignment { get; set; }

        public bool IsPublic { get; set; } = true;

        public bool IsPrivate { get; set; }

        public void SetValue(double value)
        {
            if (this.WaitingForAssignment)
            {
                this.NumberValue = value;
                this.StringValue = "[null]";
                this.WaitingForAssignment = false;
            }
            else
            {
                this.NumberValue = 0.0;
                this.NumberValue = value;
            }
        }

        public void SetValue(string value)
        {
            if (this.WaitingForAssignment)
            {
                this.StringValue = value;
                this.NumberValue = -Double.MaxValue;
                this.WaitingForAssignment = false;
            }
            else
            {
                this.StringValue = value;
            }
        }

        private void SetAll(double numValue, string strValue, bool autoCollect = false)
        {
            this.AutoCollect = autoCollect;
            SetValue(numValue);
            SetValue(strValue);
        }

        public void Nullify()
        {
            SetAll(-Double.MaxValue, "[null]");
            this.WaitingForAssignment = true;
        }

        public void MakePrivate()
        {
            this.IsPrivate = true;
            this.IsPublic = false;
        }

        public void MakePublic()
        {
            this.IsPrivate = false;
            this.IsPublic = true;
        }
    }
}
