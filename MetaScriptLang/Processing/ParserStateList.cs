using System.Xml.Linq;

namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        bool GCCanCollectL(string listName)
        {
            return this.lists[listName].garbage();
        }

        void DeleteL(string target)
        {
            this.lists.Remove(target);
        }

        void LReplace(string before, string after, string replacement)
        {
            System.Collections.Generic.List<string> newList = new();

            for (int i = 0; i < GetLSize(before); i++)
            {
                if (i == stoi(after))
                    newList.Add(replacement);
                else
                    newList.Add(GetLLine(before, i));
            }

            LClear(before);

            for (int i = 0; i < (int)newList.Count; i++)
                LAddToList(before, newList[i]);

            newList.Clear();
        }

        void LClear(string listName)
        {
            this.lists[listName].clear();
        }

        void LSort(string listName)
        {
            this.lists[listName].listSort();
        }

        void LReverse(string listName)
        {
            this.lists[listName].listReverse();
        }

        void LRevert(string listName)
        {
            this.lists[listName].listRevert();
        }

        void LRemoveFromList(string listName, string line)
        {
            this.lists[listName].remove(line);
        }

        void LAddToList(string listName, string line)
        {
            this.lists[listName].add(line);
        }

        MetaScriptLang.Data.List GetL(string listName)
        {
            return this.lists[listName];
        }

        int GetLSize(string listName)
        {
            return this.GetLSize(listName);
        }

        string GetLName(string listName)
        {
            return this.lists[listName].name();
        }

        string GetLLine(string listName, int lineNumber)
        {
            return this.lists[listName].at(lineNumber);
        }

        void SetLName(string listName, string newName)
        {
            this.lists[listName].setName(newName);
        }

        bool LExists(string listName)
        {
            return this.lists.ContainsKey(listName);
        }
    }
}
