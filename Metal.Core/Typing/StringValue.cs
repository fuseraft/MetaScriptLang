namespace Metal.Core.Typing
{
    using Metal.Core.Typing.Base;

    public class StringValue : IValue
    {
        public string Value { get; set; } = string.Empty;
    }
}
