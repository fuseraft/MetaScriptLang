namespace Metal.Core.Objects
{
    using Metal.Core.Objects.Base;
    using Metal.Core.Typing.Enums;
    using System.Collections.Generic;

    public class Method : BaseObject, IObject, IMethod
    {
        public Method(string name, IObject owner)
            : base(name, owner, Typing.Enums.TypeDefinition.Method)
        {
        }

        public Method(string name, IObject owner, ValueType returnType)
            : base(name, owner, Typing.Enums.TypeDefinition.Method)
        {
            this.ReturnType = returnType;
        }

        public Method(string name, IList<IVariable> parameters, ValueType returnType)
            : base(name, Typing.Enums.TypeDefinition.Method)
        {
            this.Parameters = parameters;
            this.ReturnType = returnType;
        }

        public Method(string name, IObject owner, IList<IVariable> parameters, ValueType returnType)
            : base(name, owner, Typing.Enums.TypeDefinition.Method)
        {
            this.Parameters = parameters;
            this.ReturnType = returnType;
        }

        public IList<IVariable> Parameters { get; set; } = new List<IVariable>();
        public ValueType ReturnType { get; set; } = ValueType.Void;
        public IList<string> Instructions { get; set; } = new List<string>();
    }
}
