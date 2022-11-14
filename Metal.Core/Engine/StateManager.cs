using Metal.Core.Objects.Base;
using Metal.Core.Objects;
using Metal.Core.Typing;
using Metal.Core.Typing.Enums;

namespace Metal.Core.Engine
{
    public class StateManager
    {
        #region Object Creation
        public static void CreateModule(string name)
        {
            IObject owner = State.GetCurrentOwner();
            Module module = new Module(name, owner);

            if (owner is Script)
            {
                ((Script)owner).Modules.Add(name, module);
                State.BeginBlock(module);
            }
        }

        public static void CreateClass(string name)
        {
            IObject owner = State.GetCurrentOwner();
            Class obj = new(name, owner);

            if (owner is IClassContainer)
            {
                ((IClassContainer)owner).Classes.Add(name, obj);
                State.BeginBlock(obj);
            }
            else
            {
                throw new Exception($"Cannot create class in current owner [{owner.Name}] of type: {owner.Type}");
            }
        }

        public static void CreateMethod(string name)
        {
            IObject owner = State.GetCurrentOwner();
            Method method = new(name, owner);

            if (owner is IMethodContainer)
            {
                ((IMethodContainer)owner).Methods.Add(name, method);
                State.BeginBlock(method);
            }
            else
            {
                throw new Exception($"Cannot create method in current owner [{owner.Name}] of type: {owner.Type}");
            }
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
                Metal.Core.Typing.Base.IValue value = null;
                Metal.Core.Typing.Enums.ValueType valueType = method.ReturnType;

                if (DateTime.TryParse(defaultValue, out DateTime dateValue))
                {
                    valueType = Metal.Core.Typing.Enums.ValueType.Date;
                    value = new DateValue()
                    {
                        Value = dateValue,
                    };
                }
                else if (Double.TryParse(defaultValue, out double numberValue))
                {
                    valueType = Metal.Core.Typing.Enums.ValueType.Number;
                    value = new NumberValue()
                    {
                        Value = numberValue,
                    };
                }
                else
                {
                    valueType = Metal.Core.Typing.Enums.ValueType.String;
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
    }
}
