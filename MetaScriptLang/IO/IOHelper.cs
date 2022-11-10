namespace MetaScriptLang.IO
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Engine;

    public class IOHelper
    {
        private static StateEngine _engine = null;

        public IOHelper(StateEngine engine)
        {
            _engine = engine;
        }

        public static List GetDirectoryList(string before, bool filesOnly)
        {
            List newList = new();
            System.Collections.Generic.List<string> dirList = new();
            // TODO: filesOnly logic
            dirList.AddRange(System.IO.Directory.GetFileSystemEntries(_engine.GetVariableString(before)));

            for (int i = 0; i < dirList.Count; i++)
            {
                newList.add(dirList[i]);
            }

            if (newList.size() == 0)
            {
                _engine.__DefiningForLoop = false;
            }

            return newList;
        }
    }
}
