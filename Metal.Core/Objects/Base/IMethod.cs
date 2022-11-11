namespace Metal.Core.Objects.Base
{
    using Metal.Core.Typing.Enums;

    public interface IMethod
    {
        string Name { get; set; }
        IList<IVariable> Parameters { get; set; }
        ValueType ReturnType { get; set; }
        IList<string> Instructions { get; set; }
    }
}
