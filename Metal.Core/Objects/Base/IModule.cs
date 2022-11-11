namespace Metal.Core.Objects.Base
{
    public interface IModule
    {
        string Name { get; set; }

        IDictionary<string, Class> Classes { get; set; }

        IDictionary<string, Method> Methods { get; set; }

        IDictionary<string, Variable> Variables { get; set; }
    }
}
