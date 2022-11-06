using MetaScriptLang.Processing;

namespace MetaScriptLang
{
    public class Engine
    {
        public int Run(int c, params string[] v)
        {
            var parser = new Parser();
            return parser.main(c, v);
        }
    }
}