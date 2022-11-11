using MetaScriptLang.Processing;

namespace MetaScriptLang
{
    public class LanguageEngine
    {
        public int Run(int c, params string[] v)
        {
            var parser = new Parser();
            return parser.main(c, v);
        }
    }
}