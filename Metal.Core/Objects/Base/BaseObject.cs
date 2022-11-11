using Metal.Core.Typing.Enums;

namespace Metal.Core.Objects.Base
{
    public abstract class BaseObject : IObject
    {
        public BaseObject(string name, IObject owner, TypeDefinition type)
        {
            this.Owner = owner;
            this.Type = type;
            this.Name = name;
        }

        public BaseObject(string name, TypeDefinition type)
        {
            this.Type = type;
            this.Name = name;
        }

        public virtual IObject Owner { get; set; }
        public TypeDefinition Type { get; set; } = TypeDefinition.Class;
        public string Name { get; set; } = string.Empty;
    }
}
