namespace Metal.Core.Objects
{
    using Metal.Core.Objects.Base;

    public class Object : BaseObject, IObject
    {
        public Object(string name) : base(name, Typing.Enums.TypeDefinition.Object)
        {
        }

        public Object(string name, IObject owner) : base(name, owner, Typing.Enums.TypeDefinition.Object)
        {
        }

        public Object(string name, IDictionary<string, Property> properties) : base(name, Typing.Enums.TypeDefinition.Object)
        {
            Properties = properties;
        }

        public Object(string name, IObject owner, IDictionary<string, Property> properties) : base(name, owner, Typing.Enums.TypeDefinition.Object)
        {
            Properties = properties;
        }

        public IDictionary<string, Property> Properties { get; set; } = new Dictionary<string, Property>();

        public IDictionary<string, Method> Methods { get; set; } = new Dictionary<string, Method>();

        public IDictionary<string, Variable> Variables { get; set; } = new Dictionary<string, Variable>();
    }
}
