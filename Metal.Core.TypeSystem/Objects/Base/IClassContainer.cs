namespace Metal.Core.TypeSystem.Objects.Base
{
    public interface IClassContainer
    {
        IDictionary<string, Class> Classes { get; set; }
    }
}
