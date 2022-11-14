namespace Metal.Core.Engine
{
    using Metal.Core.Objects.Base;

    public class StatePrinter
    {
        #region Printing State
        public static void PrintState()
        {
            Console.WriteLine($"Global Modules: {State.Modules.Count}");
            foreach (var key in State.Modules.Keys)
            {
                PrintState(State.Modules[key]);
            }

            Console.WriteLine("Script Internals:");
            PrintState(State.Script);
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
    }
}
