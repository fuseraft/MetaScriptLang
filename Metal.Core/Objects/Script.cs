﻿using Metal.Core.Objects.Base;
using Metal.Core.Typing.Enums;

namespace Metal.Core.Objects
{
    public class Script : BaseObject, IObject, IModule, IClassContainer, IMethodContainer
    {
        public Script(string name) : base(name, TypeDefinition.Script)
        {
        }

        public IDictionary<string, Module> Modules { get; set; } = new Dictionary<string, Module>();
        public IDictionary<string, Class> Classes { get; set; } = new Dictionary<string, Class>();
        public IDictionary<string, Method> Methods { get; set; } = new Dictionary<string, Method>();
        public IDictionary<string, Variable> Variables { get; set; } = new Dictionary<string, Variable>();
    }
}