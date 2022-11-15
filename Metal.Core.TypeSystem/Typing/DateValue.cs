namespace Metal.Core.TypeSystem.Typing
{
    using Metal.Core.TypeSystem.Typing.Base;

    public class DateValue : IValue
    {
        public DateTime Value { get; set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
