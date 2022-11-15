namespace Metal.Core.TypeSystem.Typing
{
    using Metal.Core.TypeSystem.Typing.Base;

    public class StringValue : IValue
    {
        public string Value { get; set; } = string.Empty;

        public override string ToString()
        {
            return Value;
        }
    }
}
