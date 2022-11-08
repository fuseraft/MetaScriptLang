namespace MetaScriptLang.Engine
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Logging;

    public partial class StateEngine
    {
        public void DeleteObject(string target)
        {
            this.objects.Remove(target);
        }

        public Variable GetObjectVariable(string objectName, string variableName)
        {
            return this.objects[objectName].getVariable(variableName);
        }

        public void AddLineToObjectMethod(string objectName, string line)
        {
            this.objects[objectName].addToCurrentMethod(line);
        }

        public void SetObjectName(string objectName, string newName)
        {
            this.objects[objectName].setName(newName);
            this.objects.Add(newName, this.objects[objectName]);
            this.objects.Remove(objectName);
        }

        public void SetObjectAsPublic(string objectName)
        {
            this.objects[objectName].setPublic();
        }

        public void SetObjectAsPrivate(string objectName)
        {
            this.objects[objectName].setPrivate();
        }

        public void ClearObject(string objectName)
        {
            this.objects[objectName].clear();
        }

        public bool ObjectExists(string objectName)
        {
            return this.objects.ContainsKey(objectName);
        }

        public bool ObjectMethodExists(string objectName, string methodName)
        {
            return this.objects[objectName].MethodExists(methodName);
        }

        public bool ObjectVariableExists(string objectName, string variableName)
        {
            return this.objects[objectName].variableExists(variableName);
        }

        public void CreateObjectMethod(string objectName, Method m)
        {
            this.objects[objectName].addMethod(m);
        }

        public void CreateObjectVariable(string objectName, Variable v)
        {
            this.objects[objectName].addVariable(v);
        }

        public void DeleteObjectVariable(string objectName, string variableName)
        {
            this.objects[objectName].removeVariable(variableName);
        }

        public int GetObjectMethodCount(string objectName)
        {
            return this.objects[objectName].methodSize();
        }

        public string GetObjectMethodNameByIndex(string objectName, int index)
        {
            return this.objects[objectName].getMethod(this.objects[objectName].getMethodName(index)).GetName();
        }

        public Method GetObjectMethod(string objectName, string methodName)
        {
            return this.objects[objectName].getMethod(methodName);
        }

        public List<Method> GetObjectMethodList(string objectName)
        {
            return this.objects[objectName].getMethods();
        }

        public List<Variable> GetObjectVariableList(string objectName)
        {
            return this.objects[objectName].getVariables();
        }

        public string GetObjectMethodLine(string objectName, string methodName, int lineNumber)
        {
            return this.objects[objectName].getMethod(methodName).GetLine(lineNumber);
        }

        public int GetObjectMethodSize(string objectName, string methodName)
        {
            return this.objects[objectName].getMethod(methodName).GetMethodSize();
        }

        public int GetObjectVariableSize(string objectName)
        {
            return this.objects[objectName].variableSize();
        }

        public string GetObjectVariableName(string objectName, string variableName)
        {
            return this.objects[objectName].getVariable(variableName).name();
        }

        public string GetObjectVariableNameByIndex(string objectName, int index)
        {
            return this.objects[objectName].getVariable(this.objects[objectName].getVariableName(index)).name();
        }

        public string GetObjectVariableString(string objectName, string variableName)
        {
            return this.objects[objectName].getVariable(variableName).getString();
        }

        public double GetObjectVariableNumber(string objectName, string variableName)
        {
            return this.objects[objectName].getVariable(variableName).getNumber();
        }

        public void SetObjectCurrentMethod(string objectName, string methodName)
        {
            this.objects[objectName].setCurrentMethod(methodName);
        }

        public void CopyObject(string newName, string oper, string oldName, string s, List<string> command)
        {
            if (oper == "=")
            {
                List<Method> objectMethods = GetObjectMethodList(oldName);
                Data.Object newObject = new(newName);

                for (int i = 0; i < objectMethods.Count; i++)
                    newObject.addMethod(objectMethods[i]);

                List<Variable> objectVariables = this.GetObjectVariableList(oldName);

                for (int i = 0; i < objectVariables.Count; i++)
                    newObject.addVariable(objectVariables[i]);

                if (__ExecutedMethod)
                    newObject.collect();
                else
                    newObject.dontCollect();

                this.objects.Add(newName, newObject);
                __CurrentObject = newName;
                __DefiningObject = false;

                newObject.clear();
                objectMethods.Clear();
            }
            else
                ErrorLogger.Error(ErrorLogger.INVALID_OPERATOR, oper, false);
        }
    }
}
