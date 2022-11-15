namespace Metal.Core.TypeSystem.Objects.Base
{
    public interface IPropertyContainer
    {
        IDictionary<string, Property> Properties { get; set; }
    }
}
