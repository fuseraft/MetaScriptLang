namespace MetaScriptLang.Engine
{
    using MetaScriptLang.Helpers;
    public partial class StateEngine
    {
        public bool GCCanCollectList(string listName)
        {
            return this.lists[listName].garbage();
        }

        public void DeleteList(string target)
        {
            this.lists.Remove(target);
        }

        public void ListReplace(string before, string after, string replacement)
        {
            System.Collections.Generic.List<string> newList = new();

            for (int i = 0; i < this.GetListSize(before); i++)
            {
                if (i == StringHelper.StoI(after))
                    newList.Add(replacement);
                else
                    newList.Add(this.GetListLine(before, i));
            }

            ListClear(before);

            for (int i = 0; i < (int)newList.Count; i++)
                this.AddToList(before, newList[i]);

            newList.Clear();
        }

        public void ListClear(string listName)
        {
            this.lists[listName].clear();
        }

        public void ListSort(string listName)
        {
            this.lists[listName].listSort();
        }

        public void ListReverse(string listName)
        {
            this.lists[listName].listReverse();
        }

        public void ListRevert(string listName)
        {
            this.lists[listName].listRevert();
        }

        public void RemoveFromList(string listName, string line)
        {
            this.lists[listName].remove(line);
        }

        public void AddToList(string listName, string line)
        {
            this.lists[listName].add(line);
        }

        public MetaScriptLang.Data.List GetList(string listName)
        {
            return this.lists[listName];
        }

        public int GetListSize(string listName)
        {
            return this.GetList(listName).size();
        }

        public string GetListName(string listName)
        {
            return this.lists[listName].name();
        }

        public string GetListLine(string listName, int lineNumber)
        {
            return this.lists[listName].at(lineNumber);
        }

        public void SetListName(string listName, string newName)
        {
            this.lists[listName].setName(newName);
        }

        public bool ListExists(string listName)
        {
            return this.lists.ContainsKey(listName);
        }
    }
}
