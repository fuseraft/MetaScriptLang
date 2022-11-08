namespace MetaScriptLang.Data
{
    using MetaScriptLang.Helpers;
    using MetaScriptLang.Logging;

    public class Object
    {
        private System.Collections.Generic.List<Method> methods = new();
        private System.Collections.Generic.List<Variable> variables = new();

        private int badMethods = 0, badVariables = 0;

        private string objectName = string.Empty, currentMethod = string.Empty;

        private bool collectable = false;

        public Object() { }

        public Object(string name)
        {
            initialize(name);
            currentMethod = string.Empty;
        }

        ~Object()
        {
            clear();
        }

        public void setName(string name)
        {
            objectName = name;
        }

        public void collect()
        {
            collectable = true;
        }

        public void dontCollect()
        {
            collectable = false;
        }

        public bool garbage()
        {
            return collectable;
        }

        public void setCurrentMethod(string methodName)
        {
            currentMethod = methodName;
        }

        public void setPublic()
        {
            if (MethodExists(currentMethod))
                methods[methodAt(currentMethod)].SetPublic();
        }

        public void setPrivate()
        {
            if (MethodExists(currentMethod))
                methods[methodAt(currentMethod)].SetPrivate();
        }

        public void addToCurrentMethod(string line)
        {
            if (MethodExists(currentMethod))
                methods[methodAt(currentMethod)].AddLine(line);
            else
                Logger.LogInfo("#!=add_to_currentMethod:undefined");
        }

        public int methodSize()
        {
            return methods.Count;
        }

        public int variableSize()
        {
            return variables.Count;
        }

        public int methodAt(string methodName)
        {
            for (int i = 0; i < methodSize(); i++)
            {
                if (methods[i].GetName() == methodName)
                    return i;
            }

            return -1;
        }

        public int variableAt(string variableName)
        {
            for (int i = 0; i < variableSize(); i++)
            {
                if (variables[i].name() == variableName)
                    return i;
            }

            return -1;
        }

        public string getMethodName(int index)
        {
            if (index < methods.Count)
                return methods[index].GetName();

            return "[undefined]";
        }

        public string getVariableName(int index)
        {
            if (index < variables.Count)
                return variables[index].name();

            return "[undefined]";
        }

        public void addMethod(Method method)
        {
            if (!method.IsBad())
                methods.Add(method);
        }

        public void addVariable(Variable variable)
        {
            if (!variable.isNull())
                variables.Add(variable);
        }

        public void clear()
        {
            clearMethods();
            clearVariables();
        }

        public void clearMethods()
        {
            methods.Clear();
        }

        public void clearVariables()
        {
            variables.Clear();
        }

        public bool isBad()
        {
            if (name().StartsWith("[bad_meth"))
                return true;

            return false;
        }

        public void removeVariable(string variableName)
        {
            System.Collections.Generic.List<Variable> oldVariables = new (getVariables());

            clearVariables();

            for (int i = 0; i < oldVariables.Count; i++)
                if (oldVariables[i].name() != variableName)
                    variables.Add(oldVariables[i]);
        }

        public Method getMethod(string methodName)
        {
            Method badMethod = new ($"[bad_meth#{badMethods}]");

            for (int i = 0; i < methods.Count; i++)
                if (methods[i].GetName() == methodName)
                    return methods[i];

            badMethods++;
            return badMethod;
        }

        public System.Collections.Generic.List<Method> getMethods()
        {
            return methods;
        }

        public Variable getVariable(string variableName)
        {
            Variable badVariable = new ($"[bad_var#{badVariables}]", "[null]");

            for (int i = 0; i < variables.Count; i++)
                if (variables[i].name() == variableName)
                    return variables[i];

            badVariables++;

            return badVariable;
        }

        public System.Collections.Generic.List<Variable> getVariables()
        {
            return variables;
        }

        public void initialize(string name)
        {
            badMethods = 0;
            badVariables = 0;
            currentMethod = string.Empty;
            objectName = name;
        }

        public bool MethodExists(string methodName)
        {
            for (int i = 0; i < methods.Count; i++)
                if (methods[i].GetName() == methodName)
                    return true;

            return false;
        }

        public string name()
        {
            return objectName;
        }

        public bool variableExists(string variableName)
        {
            for (int i = 0; i < variables.Count; i++)
                if (variables[i].name() == variableName)
                    return true;

            return false;
        }
    }
}
