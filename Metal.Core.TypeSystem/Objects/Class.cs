namespace Metal.Core.TypeSystem.Objects
{
    using Metal.Core.TypeSystem.Objects.Base;

    public class Class : BaseObject, IObject, IClass, IMethodContainer, IVariableContainer, IPropertyContainer
    {
        public Class(string name) : base(name, Typing.Enums.TypeDefinition.Class)
        {
        }

        public Class(string name, IObject owner) : base(name, owner, Typing.Enums.TypeDefinition.Class)
        {
        }

        public Class(string name, IDictionary<string, Property> properties) : base(name, Typing.Enums.TypeDefinition.Class)
        {
            Properties = properties;
        }

        public Class(string name, IObject owner, IDictionary<string, Property> properties) : base(name, owner, Typing.Enums.TypeDefinition.Class)
        {
            Properties = properties;
        }

        public IDictionary<string, Property> Properties { get; set; } = new Dictionary<string, Property>();

        public IDictionary<string, Method> Methods { get; set; } = new Dictionary<string, Method>();

        public IDictionary<string, Variable> Variables { get; set; } = new Dictionary<string, Variable>();

        public void InheritFrom(Class parent)
        {
            foreach (var key in parent.Properties.Keys)
            {
                if (!Properties.ContainsKey(key))
                {
                    Properties.Add(key, parent.Properties[key]);
                }
            }

            foreach (var key in parent.Methods.Keys)
            {
                if (!Methods.ContainsKey(key))
                {
                    Methods.Add(key, parent.Methods[key]);
                }
            }

            foreach (var key in parent.Variables.Keys)
            {
                if (!Variables.ContainsKey(key))
                {
                    Variables.Add(key, parent.Variables[key]);
                }
            }
        }
    }
}
