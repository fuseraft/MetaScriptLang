namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        void DeleteO(string target)
        {
            this.objects.Remove(target);
        }

        Variable GetOV(string objectName, string variableName)
        {
            return this.objects[objectName].getVariable(variableName);
        }

        void SetOMAddLineToCurrentMethod(string objectName, string line)
        {
            this.objects[objectName].addToCurrentMethod(line);
        }

        void SetOName(string objectName, string newName)
        {
            this.objects[objectName].setName(newName);
            // TODO fix the object lookup.
            //this.objects.Add(newName, this.objects[objectName]);
            //this.objects.RemoveAt(objectName);
        }

        void SetOPublic(string objectName)
        {
            this.objects[objectName].setPublic();
        }

        void SetOPrivate(string objectName)
        {
            this.objects[objectName].setPrivate();
        }

        void ClearO(string objectName)
        {
            this.objects[objectName].clear();
        }

        bool OExists(string objectName)
        {
            return this.objects.ContainsKey(objectName);
        }

        bool OMExists(string objectName, string methodName)
        {
            return this.objects[objectName].methodExists(methodName);
        }

        bool OVExists(string objectName, string variableName)
        {
            return this.objects[objectName].variableExists(variableName);
        }

        void CreateOM(string objectName, Method m)
        {
            this.objects[objectName].addMethod(m);
        }

        void CreateOV(string objectName, Variable v)
        {
            this.objects[objectName].addVariable(v);
        }

        void DeleteOV(string objectName, string variableName)
        {
            this.objects[objectName].removeVariable(variableName);
        }

        int GetOMCount(string objectName)
        {
            return this.objects[objectName].methodSize();
        }

        string GetOMNameByIndex(string objectName, int index)
        {
            return this.objects[objectName].getMethod(objects[objectName].getMethodName(index)).GetName();
        }

        Method GetOM(string objectName, string methodName)
        {
            return this.objects[objectName].getMethod(methodName);
        }

        System.Collections.Generic.List<Method> GetOMList(string objectName)
        {
            return this.objects[objectName].getMethods();
        }

        System.Collections.Generic.List<Variable> GetOVList(string objectName)
        {
            return this.objects[objectName].getVariables();
        }

        string GetOMLine(string objectName, string methodName, int lineNumber)
        {
            return objects[objectName].getMethod(methodName).GetLine(lineNumber);
        }

        int GetOMSize(string objectName, string methodName)
        {
            return objects[objectName].getMethod(methodName).GetMethodSize();
        }

        int GetOVCount(string objectName)
        {
            return objects[objectName].variableSize();
        }

        string GetOVName(string objectName, string variableName)
        {
            return this.objects[objectName].getVariable(variableName).name();
        }

        string GetOVNameByIndex(string objectName, int index)
        {
            return objects[objectName].getVariable(objects[objectName].getVariableName(index)).name();
        }

        string GetOVString(string objectName, string variableName)
        {
            return this.objects[objectName].getVariable(variableName).getString();
        }

        double GetOVNumber(string objectName, string variableName)
        {
            return this.objects[objectName].getVariable(variableName).getNumber();
        }

        void SetOMCurrentMethod(string objectName, string methodName)
        {
            objects[objectName].setCurrentMethod(methodName);
        }

        void CopyObject(string newName, string oper, string oldName, string s, System.Collections.Generic.List<string> command)
        {
            if (oper == "=")
            {
                System.Collections.Generic.List<Method> objectMethods = GetOMList(oldName);
                MetaScriptLang.Data.Object newObject = new(newName);

                for (int i = 0; i < objectMethods.Count; i++)
                    newObject.addMethod(objectMethods[i]);

                System.Collections.Generic.List<Variable> objectVariables = GetOVList(oldName);

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
                error(ErrorLogger.INVALID_OPERATOR, oper, false);
        }
    }
}
