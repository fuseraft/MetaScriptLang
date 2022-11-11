namespace Metal.Core.Objects.Base
{
    using Metal.Core.Typing.Base;
    using Metal.Core.Typing.Enums;

    public interface IVariable
    {
        public string Name { get; set; }
        public IValue Value { get; set; }
        public ValueType ValueType { get; set; }
    }
}
