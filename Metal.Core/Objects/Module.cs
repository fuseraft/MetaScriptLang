namespace Metal.Core.Objects
{
    using Metal.Core.Objects.Base;
    using Metal.Core.Typing.Enums;

    public class Module : BaseObject, IObject
    {
        public Module(string name) : base(name, null, TypeDefinition.Module)
        {
        }

        public Module(string name, IObject owner) : base(name, owner, TypeDefinition.Module)
        {
        }

        public IDictionary<string, Object> Objects { get; set; } = new Dictionary<string, Object>();

        public IDictionary<string, Method> Methods { get; set; } = new Dictionary<string, Method>();

        public IDictionary<string, Variable> Variables { get; set; } = new Dictionary<string, Variable>();
    }
}
