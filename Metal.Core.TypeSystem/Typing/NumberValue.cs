namespace Metal.Core.TypeSystem.Typing
{
    using Metal.Core.TypeSystem.Typing.Base;

    public class NumberValue : IValue
    {
        public Double Value { get; set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
