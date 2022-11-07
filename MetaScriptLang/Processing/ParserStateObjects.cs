namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        Variable GetObjectVariable(string objectName, string variableName)
        {
            return this.objects[objectName].getVariable(variableName);
        }

        void AddLineToObjectMethod(string objectName, string line)
        {
            this.objects[objectName].addToCurrentMethod(line);
        }

        void SetObjectName(string objectName, string newName)
        {
            this.objects[objectName].setName(newName);
            this.objects.Add(newName, this.objects[objectName]);
            this.objects.Remove(objectName);
        }

        void SetObjectAsPublic(string objectName)
        {
            this.objects[objectName].setPublic();
        }

        void SetObjectAsPrivate(string objectName)
        {
            this.objects[objectName].setPrivate();
        }

        bool ObjectMethodExists(string objectName, string methodName)
        {
            return this.objects[objectName].methodExists(methodName);
        }

        bool ObjectVariableExists(string objectName, string variableName)
        {
            return this.objects[objectName].variableExists(variableName);
        }

        void CreateObjectMethod(string objectName, Method m)
        {
            this.objects[objectName].addMethod(m);
        }

        void CreateObjectVariable(string objectName, Variable v)
        {
            this.objects[objectName].addVariable(v);
        }

        void DeleteObjectVariable(string objectName, string variableName)
        {
            this.objects[objectName].removeVariable(variableName);
        }

        int GetObjectMethodCount(string objectName)
        {
            return this.objects[objectName].methodSize();
        }

        string GetObjectMethodNameByIndex(string objectName, int index)
        {
            return this.objects[objectName].getMethod(objects[objectName].getMethodName(index)).GetName();
        }

        Method GetObjectMethod(string objectName, string methodName)
        {
            return this.objects[objectName].getMethod(methodName);
        }

        System.Collections.Generic.List<Method> GetObjectMethodList(string objectName)
        {
            return this.objects[objectName].getMethods();
        }

        System.Collections.Generic.List<Variable> GetObjectVariableList(string objectName)
        {
            return this.objects[objectName].getVariables();
        }

        string GetObjectMethodLine(string objectName, string methodName, int lineNumber)
        {
            return objects[objectName].getMethod(methodName).GetLine(lineNumber);
        }

        int GetObjectMethodSize(string objectName, string methodName)
        {
            return objects[objectName].getMethod(methodName).GetMethodSize();
        }

        int GetObjectVariableSize(string objectName)
        {
            return objects[objectName].variableSize();
        }

        string GetObjectVariableName(string objectName, string variableName)
        {
            return this.objects[objectName].getVariable(variableName).name();
        }

        string GetObjectVariableNameByIndex(string objectName, int index)
        {
            return objects[objectName].getVariable(objects[objectName].getVariableName(index)).name();
        }

        string GetObjectVariableString(string objectName, string variableName)
        {
            return this.objects[objectName].getVariable(variableName).getString();
        }

        double GetObjectVariableNumber(string objectName, string variableName)
        {
            return this.objects[objectName].getVariable(variableName).getNumber();
        }

        void SetObjectCurrentMethod(string objectName, string methodName)
        {
            objects[objectName].setCurrentMethod(methodName);
        }

        void CopyObject(string newName, string oper, string oldName, string s, System.Collections.Generic.List<string> command)
        {
            if (oper == "=")
            {
                System.Collections.Generic.List<Method> objectMethods = GetObjectMethodList(oldName);
                MetaScriptLang.Data.Object newObject = new(newName);

                for (int i = 0; i < objectMethods.Count; i++)
                    newObject.addMethod(objectMethods[i]);

                System.Collections.Generic.List<Variable> objectVariables = GetObjectVariableList(oldName);

                for (int i = 0; i < objectVariables.Count; i++)
                    newObject.addVariable(objectVariables[i]);

                if (__ExecutedMethod)
                    newObject.collect();
                else
                    newObject.dontCollect();

                objects.Add(newName, newObject);
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
