namespace Metal.Core.TypeSystem.Objects.Base
{
    using Metal.Core.TypeSystem.Typing.Enums;

    public interface IProperty
    {
        string Name { get; set; }

        public ValueType ValueType { get; set; }

        public Method Getter { get; set; }

        public Method Setter { get; set; }
    }
}
