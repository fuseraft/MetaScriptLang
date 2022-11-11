using Metal.Core.Objects.Base;
using Metal.Core.Typing.Enums;

namespace Metal.Core.Objects
{
    public class Script : BaseObject, IObject
    {
        public Script(string name) : base(name, TypeDefinition.Script)
        {
        }

        public IDictionary<string, Module> Modules { get; set; } = new Dictionary<string, Module>();
        public IDictionary<string, Object> Objects { get; set; } = new Dictionary<string, Object>();
        public IDictionary<string, Method> Methods { get; set; } = new Dictionary<string, Method>();
        public IDictionary<string, Variable> Variables { get; set; } = new Dictionary<string, Variable>();
    }
}
