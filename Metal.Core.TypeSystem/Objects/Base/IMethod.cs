namespace Metal.Core.TypeSystem.Objects.Base
{
    using Metal.Core.TypeSystem.Typing.Enums;

    public interface IMethod
    {
        string Name { get; set; }
        IList<IVariable> Parameters { get; set; }
        ValueType ReturnType { get; set; }
        IList<string> Instructions { get; set; }
    }
}
