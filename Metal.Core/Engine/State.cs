namespace Metal.Core.Engine
{
    using Metal.Core.Objects;
    using Metal.Core.Objects.Base;
    using Metal.Core.Typing;
    using Metal.Core.Typing.Enums;

    public class State
    {
        static State()
        {
            StartBlock(Script);
        }

        public static Script Script { get; set; } = new Script("__INIT__");
        public static IDictionary<string, Module> Modules { get; set; } = new Dictionary<string, Module>();
        public static TypeDefinition CurrentTypeDefinition => GetCurrentOwner().Type;
        public static string CurrentOwnerName => GetCurrentOwner().Name;

        private static Stack<string> OwnerStack { get; set; } = new Stack<string>();
        private static Dictionary<string, IObject> Owners { get; set; } = new Dictionary<string, IObject>();

        public static void CreateModule(string name)
        {
            IObject owner = GetCurrentOwner();
            Module module = new Module(name, owner);

            if (owner is Script)
            {
                ((Script)owner).Modules.Add(name, module);
                StartBlock(module);
            }
        }

        public static void CreateObject(string name)
        {
            IObject owner = GetCurrentOwner();
            Object obj = new(name, owner);

            if (owner is Script)
            {
                ((Script)owner).Objects.Add(name, obj);
                StartBlock(obj);
            }
            else if (owner is Module)
            {
                ((Module)owner).Objects.Add(name, obj);
                StartBlock(obj);
            }
        }

        public static void CreateMethod(string name)
        {
            IObject owner = GetCurrentOwner();
            Method method = new(name, owner);
            
            if (owner is Script)
            {
                ((Script)owner).Methods.Add(name, method);
                StartBlock(method);
            }
            else if (owner is Module)
            {
                ((Module)owner).Methods.Add(name, method);
                StartBlock(method);
            }
            else if (owner is Object)
            {
                ((Object)owner).Methods.Add(name, method);
                StartBlock(method);
            }
        }

        public static void EndBlock()
        {
            if (OwnerStack.Count > 1)
            {
                var lastOwner = OwnerStack.Pop();
                Owners.Remove(lastOwner);
            }
        }

        public static void AddParameter(string name, string defaultValue = "")
        {
            IObject owner = GetCurrentOwner();

            if (owner is Method)
            {
                Method method = (Method)owner;
                Metal.Core.Typing.Base.IValue value = null;
                ValueType valueType = method.ReturnType;

                if (DateTime.TryParse(defaultValue, out DateTime dateValue))
                {
                    valueType = ValueType.Date;
                    value = new DateValue()
                    {
                        Value = dateValue,
                    };
                }
                else if (Double.TryParse(defaultValue, out double numberValue))
                {
                    valueType = ValueType.Number;
                    value = new NumberValue()
                    {
                        Value = numberValue,
                    };
                }
                else
                {
                    valueType = ValueType.String;
                    value = new StringValue()
                    {
                        Value = defaultValue,
                    };
                }

                method.Parameters.Add(new Variable(name, method, valueType, value));
            }
        }

        public static void AddInstruction(string instruction)
        {
            IObject owner = GetCurrentOwner();

            if (owner is Method)
            {
                ((Method)owner).Instructions.Add(instruction);
            }
        }

        private static void StartBlock(IObject owner)
        {
            OwnerStack.Push(owner.Name);
            Owners.Add(owner.Name, owner);
        }

        private static IObject GetCurrentOwner()
        {
            string ownerName = OwnerStack.Peek();

            if (Owners.ContainsKey(ownerName))
            {
                return Owners[ownerName];
            }

            return null;
        }
    }
}
