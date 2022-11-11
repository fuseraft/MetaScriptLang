namespace Metal.Core.Objects
{
    using Metal.Core.Objects.Base;
    using Metal.Core.Typing.Base;
    using Metal.Core.Typing.Enums;

    public class Variable : BaseObject, IObject, IVariable
    {
        public Variable(string name, ValueType valueType, IValue value) : base(name, TypeDefinition.Variable)
        {
            this.Value = value;
            this.ValueType = valueType;
        }

        public Variable(string name, IObject owner, ValueType valueType, IValue value) : base(name, owner, TypeDefinition.Variable)
        {
            this.Value = value;
            this.ValueType = valueType;
        }

        public IValue Value { get; set; }
        public ValueType ValueType { get; set; }
    }
}
