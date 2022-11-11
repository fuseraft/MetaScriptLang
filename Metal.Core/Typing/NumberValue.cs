﻿namespace Metal.Core.Typing
{
    using Metal.Core.Typing.Base;

    public class NumberValue : IValue
    {
        public Double Value { get; set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}