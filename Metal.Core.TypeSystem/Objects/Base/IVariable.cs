namespace Metal.Core.TypeSystem.Objects.Base
{
    using Metal.Core.TypeSystem.Typing.Base;
    using Metal.Core.TypeSystem.Typing.Enums;

    public interface IVariable
    {
        public string Name { get; set; }
        public IValue Value { get; set; }
        public ValueType ValueType { get; set; }
    }
}
