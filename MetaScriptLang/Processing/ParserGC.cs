using MetaScriptLang.Data;

namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        void collectGarbage()
        {
            System.Collections.Generic.List<string> garbageVars = new();
            System.Collections.Generic.List<string> garbageLists = new();
            System.Collections.Generic.List<string> garbageObjects = new();

            foreach (var key in this.variables.Keys)
            {
                if (this.variables[key].garbage() && !__ExecutedIfStatement)
                    if (!__DontCollectMethodVars)
                        garbageVars.Add(variables[key].name());
            }
                
            for (int i = 0; i < (int)garbageVars.Count; i++)
                DeleteV(garbageVars[i]);

            foreach (var key in this.lists.Keys)
            {
                if (lists[key].garbage() && !__ExecutedIfStatement)
                    garbageLists.Add(lists[key].name());
            }
                
            for (int i = 0; i < garbageLists.Count; i++)
                DeleteL(garbageLists[i]);

            foreach (var key in this.objects.Keys)
            {
                if (objects[key].garbage() && !__ExecutedIfStatement)
                    garbageObjects.Add(objects[key].name());
            }

            for (int i = 0; i < (int)garbageObjects.Count; i++)
                DeleteO(garbageObjects[i]);
        }

        void clearAll()
        {
            clearMethods();
            clearObjects();
            clearVariables();
            clearLists();
            clearArgs();
            clearIf();
            clearFor();
            clearWhile();
            clearConstants();
        }

        void clearConstants()
        {
            constants.Clear();
        }

        void clearArgs()
        {
            args.Clear();
        }

        void clearFor()
        {
            forLoops.Clear();
        }

        void clearWhile()
        {
            whileLoops.Clear();
        }

        void clearIf()
        {
            ifStatements.Clear();
        }

        void clearLists()
        {
            lists.Clear();
        }

        void clearMethods()
        {
            System.Collections.Generic.List<Method> indestructibleMethods = new();

            foreach (var key in this.methods.Keys)
            {
                if (methods[key].IsLocked())
                    indestructibleMethods.Add(methods[key]);
            }

            methods.Clear();

            for (int i = 0; i < (int)indestructibleMethods.Count; i++)
                methods.Add(indestructibleMethods[i].GetName(), indestructibleMethods[i]);
        }

        void clearObjects()
        {
            objects.Clear();
        }

        void clearVariables()
        {
            System.Collections.Generic.List<Variable> indestructibleVariables = new();

            foreach (var key in this.variables.Keys)
            {
                if (this.variables[key].indestructible())
                {
                    // TODO Shallow Copy
                    indestructibleVariables.Add(variables[key]);
                }
            }

            variables.Clear();
            
            for (int i = 0; i < indestructibleVariables.Count; i++)
                variables.Add(indestructibleVariables[i].name(), indestructibleVariables[i]);
        }
    }
}
