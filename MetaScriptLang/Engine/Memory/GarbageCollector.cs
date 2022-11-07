namespace MetaScriptLang.Engine.Memory
{
    using MetaScriptLang.Data;

    public class GarbageCollector
    {
        private StateEngine engine = null;

        public GarbageCollector(StateEngine engine)
        {
            this.engine = engine;
        }

        public void DoGarbageCollection()
        {
            System.Collections.Generic.List<string> garbageVars = new();
            System.Collections.Generic.List<string> garbageLists = new();
            System.Collections.Generic.List<string> garbageObjects = new();

            foreach (var key in this.engine.GetVariableKeys())
            {
                if (this.engine.variables[key].garbage() && !this.engine.__ExecutedIfStatement)
                    if (!this.engine.__DontCollectMethodVars)
                        garbageVars.Add(this.engine.variables[key].name());
            }

            for (int i = 0; i < (int)garbageVars.Count; i++)
                this.engine.DeleteVariable(garbageVars[i]);

            foreach (var key in this.engine.lists.Keys)
            {
                if (this.engine.lists[key].garbage() && !this.engine.__ExecutedIfStatement)
                    garbageLists.Add(this.engine.lists[key].name());
            }

            for (int i = 0; i < garbageLists.Count; i++)
                this.engine.DeleteList(garbageLists[i]);

            foreach (var key in this.engine.objects.Keys)
            {
                if (this.engine.objects[key].garbage() && !this.engine.__ExecutedIfStatement)
                    garbageObjects.Add(this.engine.objects[key].name());
            }

            for (int i = 0; i < (int)garbageObjects.Count; i++)
                this.engine.DeleteObject(garbageObjects[i]);
        }

        public void ClearAll()
        {
            ClearMethods();
            ClearObjects();
            ClearVariables();
            ClearLists();
            ClearArgs();
            ClearIfStatements();
            ClearForLoops();
            ClearWhileLoops();
            ClearConstants();
        }

        public void ClearConstants()
        {
            engine.constants.Clear();
        }

        public void ClearArgs()
        {
            engine.args.Clear();
        }

        public void ClearForLoops()
        {
            engine.forLoops.Clear();
        }

        public void ClearWhileLoops()
        {
            engine.whileLoops.Clear();
        }

        public void ClearIfStatements()
        {
            engine.ifStatements.Clear();
        }

        public void ClearLists()
        {
            engine.lists.Clear();
        }

        public void ClearMethods()
        {
            System.Collections.Generic.List<Method> indestructibleMethods = new();

            foreach (var key in this.engine.methods.Keys)
            {
                if (this.engine.methods[key].IsLocked())
                    indestructibleMethods.Add(this.engine.methods[key]);
            }

            this.engine.methods.Clear();

            for (int i = 0; i < (int)indestructibleMethods.Count; i++)
                this.engine.methods.Add(indestructibleMethods[i].GetName(), indestructibleMethods[i]);
        }

        public void ClearObjects()
        {
            this.engine.objects.Clear();
        }

        public void ClearVariables()
        {
            System.Collections.Generic.List<Variable> indestructibleVariables = new();

            foreach (var key in this.engine.variables.Keys)
            {
                if (this.engine.variables[key].indestructible())
                {
                    // TODO Shallow Copy
                    indestructibleVariables.Add(this.engine.variables[key]);
                }
            }

            this.engine.variables.Clear();

            for (int i = 0; i < indestructibleVariables.Count; i++)
                this.engine.variables.Add(indestructibleVariables[i].name(), indestructibleVariables[i]);
        }
    }
}
