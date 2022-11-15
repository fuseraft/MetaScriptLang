namespace Metal.Core.TypeSystem.Objects
{
    using Metal.Core.TypeSystem.Objects.Base;
    using Metal.Core.TypeSystem.Typing.Enums;

    public class Module : BaseObject, IObject, IModule, IClassContainer, IMethodContainer, IVariableContainer
    {
        public Module(string name) : base(name, null, TypeDefinition.Module)
        {
        }

        public Module(string name, IObject owner) : base(name, owner, TypeDefinition.Module)
        {
        }

        public IDictionary<string, Class> Classes { get; set; } = new Dictionary<string, Class>();

        public IDictionary<string, Method> Methods { get; set; } = new Dictionary<string, Method>();

        public IDictionary<string, Variable> Variables { get; set; } = new Dictionary<string, Variable>();
    }
}
