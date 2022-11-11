namespace Metal.Core.Typing
{
    using Metal.Core.Typing.Base;

    public class DateValue : IValue
    {
        public DateTime Value { get; set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
