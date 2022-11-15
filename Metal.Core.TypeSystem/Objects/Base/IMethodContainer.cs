namespace Metal.Core.TypeSystem.Objects.Base
{
    public interface IMethodContainer
    {
        IDictionary<string, Method> Methods { get; set; }

    }
}
