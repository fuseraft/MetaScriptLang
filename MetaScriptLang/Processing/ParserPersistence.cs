using MetaScriptLang.Data;
using System.IO;
using System.Linq;

namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        void saveVariable(string variableName)
        {
            Crypt c = new();

            System.IO.File.AppendAllText(__SavedVars, c.e(variableName) + Environment.NewLine);
        }
    }
}
