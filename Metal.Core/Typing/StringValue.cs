namespace Metal.Core.Typing
{
    using Metal.Core.Typing.Base;

    public class StringValue : IValue
    {
        public string Value { get; set; } = string.Empty;

        public override string ToString()
        {
            return Value;
        }
    }
}
