namespace Metal.Core.Engine
{
    using Metal.Core.TypeSystem.Objects.Base;
    using Metal.Core.TypeSystem.Objects;
    using Metal.Core.TypeSystem.Typing;

    public class StateManager
    {
        #region Object Creation
        public static IModule CreateModule(string name)
        {
            IObject owner = State.GetCurrentOwner();

            if (owner is not Script)
            {
                throw new Exception($"Cannot create module in current owner[{owner.Name}] of type: {owner.Type}");
            }

            Module module = new Module(name, owner);
            ((Script)owner).Modules.Add(name, module);
            State.PushOwner(module);

            return module;
        }

        public static IClass CreateClass(string name)
        {
            IObject owner = State.GetCurrentOwner();
            
            if (owner is not IClassContainer)
            {
                throw new Exception($"Cannot create class in current owner [{owner.Name}] of type: {owner.Type}");
            }

            Class obj = new(name, owner);
            ((IClassContainer)owner).Classes.Add(name, obj);
            State.PushOwner(obj);

            return obj;
        }

        public static IMethod CreateMethod(string name)
        {
            IObject owner = State.GetCurrentOwner();

            if (owner is not IMethodContainer)
            {
                throw new Exception($"Cannot create method in current owner [{owner.Name}] of type: {owner.Type}");
            }

            Method method = new(name, owner);
            ((IMethodContainer)owner).Methods.Add(name, method);
            State.PushOwner(method);
            return method;
        }

        public static IVariable CreateVariable(string name, TypeSystem.Typing.Enums.ValueType valueType, TypeSystem.Typing.Base.IValue value)
        {
            IObject owner = State.GetCurrentOwner();

            if (owner is not IVariableContainer)
            {
                throw new Exception($"Cannot create variable in current owner [{owner.Name}] of type: {owner.Type}");
            }

            Variable variable = new (name, owner, valueType, value);
            State.PushLookup(variable);
            return variable;
        }

        public static IProperty CreateProperty(string name, TypeSystem.Typing.Enums.ValueType valueType)
        {
            IObject owner = State.GetCurrentOwner();

            if (owner is not IPropertyContainer)
            {
                throw new Exception($"Cannot create property in current owner [{owner.Name}] of type: {owner.Type}");
            }

            Property property = new(name, valueType);
            State.PushLookup(property);
            return property;
        }

        public static void InheritFrom(string name)
        {
            IDictionary<string, Class> classDefinitions = State.GetLoadedClassDefinitions();

            if (!classDefinitions.ContainsKey(name))
            {
                throw new Exception($"Inheritance error. Class not found: {name}");
            }

            IObject current = State.GetCurrentOwner();
            Class parent = classDefinitions[name];

            if (current is Class)
            {
                var cls = (Class)current;
                cls.InheritFrom(parent);
            }
        }
        #endregion

        #region Method Definition
        public static void AddParameter(string name, string defaultValue = "")
        {
            IObject owner = State.GetCurrentOwner();

            if (owner is Method)
            {
                Method method = (Method)owner;
                Metal.Core.TypeSystem.Typing.Base.IValue value = null;
                Metal.Core.TypeSystem.Typing.Enums.ValueType valueType = method.ReturnType;

                if (DateTime.TryParse(defaultValue, out DateTime dateValue))
                {
                    valueType = Metal.Core.TypeSystem.Typing.Enums.ValueType.Date;
                    value = new DateValue()
                    {
                        Value = dateValue,
                    };
                }
                else if (Double.TryParse(defaultValue, out double numberValue))
                {
                    valueType = Metal.Core.TypeSystem.Typing.Enums.ValueType.Number;
                    value = new NumberValue()
                    {
                        Value = numberValue,
                    };
                }
                else
                {
                    valueType = Metal.Core.TypeSystem.Typing.Enums.ValueType.String;
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
            IObject owner = State.GetCurrentOwner();

            if (owner is Method)
            {
                ((Method)owner).Instructions.Add(instruction);
            }
        }
        #endregion

        #region Object Existence
        public static IObject GetObjectByName(string name)
        {
            return State.FindObjectByName(name);
        }
        #endregion

        #region Object Retrieval
        public static IMethod GetMethod(string name)
        {
            IObject search = State.FindObjectByName(name);

            if (search is null || search is not Method)
            {
                throw new Exception($"Method undefined: {name}");
            }

            return (Method)search;
        }

        public static IClass GetClass(string name)
        {
            IObject search = State.FindObjectByName(name);

            if (search is null || search is not Class)
            {
                throw new Exception($"Class undefined: {name}");
            }

            return (Class)search;
        }
        #endregion
    }
}
