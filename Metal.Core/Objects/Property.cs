namespace Metal.Core.Objects
{
    using Metal.Core.Objects.Base;
    using Metal.Core.Typing.Enums;

    public class Property : BaseObject, IObject, IValueType
    {
        public Property(string name, Typing.Enums.ValueType valueType)
            : base(name, Typing.Enums.TypeDefinition.Property)
        {
            this.ValueType = valueType;
        }

        public ValueType ValueType { get; set; }
    }
}
