
namespace Metal.Core.TypeSystem.Objects.Base
{
    using Metal.Core.TypeSystem.Typing.Enums;
    public interface IObject
    {
        IObject Owner { get; set; }
        TypeDefinition Type { get; set; }
        string Name { get; set; }
    }
}
