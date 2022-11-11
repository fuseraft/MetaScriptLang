using Metal.Core.Typing.Enums;

namespace Metal.Core.Objects.Base
{
    public interface IObject
    {
        IObject Owner { get; set; }
        TypeDefinition Type { get; set; }
        string Name { get; set; }
    }
}
