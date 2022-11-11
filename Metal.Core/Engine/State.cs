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

        #region Printing State
        public static void PrintState()
        {
            Console.WriteLine($"Global Modules: {Modules.Count}");
            foreach (var key in Modules.Keys)
            {
                PrintState(Modules[key]);
            }

            Console.WriteLine("Script Internals:");
            PrintState(Script);
        }

        public static void PrintState(IModule module)
        {
            Console.WriteLine($"  Module[{module.Name}],Variables={module.Variables.Count}");
            foreach (var key in module.Variables.Keys)
            {
                PrintState(module.Variables[key]);
            }

            Console.WriteLine($"  Module[{module.Name}],Methods={module.Methods.Count}");
            foreach (var key in module.Methods.Keys)
            {
                PrintState(module.Methods[key]);
            }

            Console.WriteLine($"  Module[{module.Name}],Classes={module.Classes.Count}");
            foreach (var key in module.Classes.Keys)
            {
                PrintState(module.Classes[key]);
            }
        }

        public static void PrintState(IClass obj)
        {
            Console.WriteLine($"    Class[{obj.Name}],Variables={obj.Variables.Count}");
            foreach (var key in obj.Variables.Keys)
            {
                PrintState(obj.Variables[key]);
            }

            Console.WriteLine($"    Class[{obj.Name}],Methods={obj.Methods.Count}");
            foreach (var key in obj.Methods.Keys)
            {
                PrintState(obj.Methods[key]);
            }
        }

        public static void PrintState(IMethod method)
        {
            Console.WriteLine($"      Method[{method.Name}],Parameters={method.Parameters.Count}");
            foreach (var parameter in method.Parameters)
            {
                PrintState(parameter);
            }

            Console.WriteLine($"      Method[{method.Name}],Instructions={method.Instructions.Count}");
            for (var i = 0; i < method.Instructions.Count; i++)
            {
                Console.WriteLine($"         {i}:\t{method.Instructions[i]}");
            }
        }

        public static void PrintState(IVariable v)
        {
            Console.WriteLine($"         Variable[{v.Name}],valueType={v.ValueType},value={v.Value}");
        }
        #endregion

        #region Object Creation
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

        public static void CreateClass(string name)
        {
            IObject owner = GetCurrentOwner();
            Class obj = new(name, owner);

            if (owner is IClassContainer)
            {
                ((IClassContainer)owner).Classes.Add(name, obj);
                StartBlock(obj);
            }
            else
            {
                throw new Exception($"Cannot create class in current owner [{owner.Name}] of type: {owner.Type}");
            }
        }

        public static void CreateMethod(string name)
        {
            IObject owner = GetCurrentOwner();
            Method method = new(name, owner);
            
            if (owner is IMethodContainer)
            {
                ((IMethodContainer)owner).Methods.Add(name, method);
                StartBlock(method);
            }
            else
            {
                throw new Exception($"Cannot create method in current owner [{owner.Name}] of type: {owner.Type}");
            }
        }

        public static void InheritFrom(string name)
        {
            IDictionary<string, Class> classDefinitions = GetLoadedClassDefinitions();
            
            if (!classDefinitions.ContainsKey(name))
            {
                throw new Exception($"Inheritance error. Class not found: {name}");
            }

            IObject current = GetCurrentOwner();
            Class parent = classDefinitions[name];

            if (current is Class)
            {
                var cls = (Class)current;
                cls.InheritFrom(parent);
            }
        }
        #endregion

        #region Ownership
        public static void EndBlock()
        {
            if (OwnerStack.Count > 1)
            {
                var lastOwner = OwnerStack.Pop();
                Owners.Remove(lastOwner);
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
        #endregion

        #region Method Definition
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
        #endregion

        #region Class Definitions
        private static IDictionary<string, Class> GetLoadedClassDefinitions()
        {
            IDictionary<string, Class> loadedClasses = new Dictionary<string, Class>();

            // Check loaded modules.
            LoadClassDefinitionsFromModuleDictionary(Modules, loadedClasses);

            // Check script modules.
            LoadClassDefinitionsFromModuleDictionary(Script.Modules, loadedClasses);

            // Check script classes.
            foreach (var clsKey in Script.Classes.Keys)
            {
                loadedClasses.Add(clsKey, Script.Classes[clsKey]);
            }

            return loadedClasses;
        }

        private static void LoadClassDefinitionsFromModuleDictionary(IDictionary<string, Module> modules, IDictionary<string, Class> loadedClasses)
        {
            foreach (var key in modules.Keys)
            {
                var modClasses = GetClassDefinitionsFromModule(modules[key]);

                foreach (var modClass in modClasses.Keys)
                {
                    loadedClasses.Add(modClass, modClasses[modClass]);
                }
            }
        }

        private static IDictionary<string, Class> GetClassDefinitionsFromModule(Module m)
        {
            IDictionary<string, Class> loadedClasses = new Dictionary<string, Class>();

            foreach (var clsKey in m.Classes.Keys)
            {
                if (!loadedClasses.ContainsKey(clsKey))
                {
                    loadedClasses.Add(clsKey, m.Classes[clsKey]);
                }
            }

            return loadedClasses;
        }
        #endregion
    }
}
