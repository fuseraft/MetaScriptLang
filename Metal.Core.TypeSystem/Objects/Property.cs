namespace Metal.Core.TypeSystem.Objects
{
    using Metal.Core.TypeSystem.Objects.Base;
    using Metal.Core.TypeSystem.Typing.Enums;

    public class Property : BaseObject, IObject, IValueType, IProperty
    {
        public Property(string name, Typing.Enums.ValueType valueType)
            : base(name, Typing.Enums.TypeDefinition.Property)
        {
            this.ValueType = valueType;
        }

        public ValueType ValueType { get; set; }
        public Method Getter { get; set; }
        public Method Setter { get; set; }
    }
}
