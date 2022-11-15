namespace Metal.Core.Engine
{
    using Metal.Core.TypeSystem.Objects;
    using Metal.Core.TypeSystem.Objects.Base;
    using Metal.Core.TypeSystem.Typing.Enums;

    public class State
    {
        static State()
        {
            PushOwner(Script);
        }

        public static Script Script { get; set; } = new Script("__INIT__");
        public static IDictionary<string, Module> Modules { get; set; } = new Dictionary<string, Module>();
        public static TypeDefinition CurrentTypeDefinition => GetCurrentOwner().Type;
        public static string CurrentOwnerName => GetCurrentOwner().Name;

        private static Stack<string> OwnerStack { get; set; } = new Stack<string>();
        private static Dictionary<string, IObject> Owners { get; set; } = new Dictionary<string, IObject>();

        public static Dictionary<string, IObject> ObjectLookup { get; set; } = new Dictionary<string, IObject>();

        #region Ownership
        public static void PushOwner(IObject owner)
        {
            OwnerStack.Push(owner.Name);
            Owners.Add(owner.Name, owner);
            PushLookup(owner);
        }

        public static void PushLookup(IObject newObject)
        {
            ObjectLookup.Add(newObject.Name, newObject);
        }

        public static void PopOwner()
        {
            if (OwnerStack.Count > 1)
            {
                var lastOwner = OwnerStack.Pop();
                Owners.Remove(lastOwner);
            }
        }

        public static IObject FindObjectByName(string name)
        {
            return Owners.ContainsKey(name) ? Owners[name] : null;
        }

        public static IObject GetCurrentOwner()
        {
            string ownerName = OwnerStack.Peek();

            if (Owners.ContainsKey(ownerName))
            {
                return Owners[ownerName];
            }

            return null;
        }
        #endregion

        #region Class Definitions
        public static IDictionary<string, Class> GetLoadedClassDefinitions()
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
