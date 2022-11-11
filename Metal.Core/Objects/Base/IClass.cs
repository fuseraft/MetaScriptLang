namespace Metal.Core.Objects.Base
{
    public interface IClass
    {
        string Name { get; set; }

        IDictionary<string, Property> Properties { get; set; }

        IDictionary<string, Method> Methods { get; set; }

        IDictionary<string, Variable> Variables { get; set; }
    }
}
