namespace MetaScriptLangCLI
{
    using MetaScriptLang;

    public class Program
    {
        public static void Main(string[] args)
        {
            var engine = new LanguageEngine();
            engine.Run(args.Length, args);
        }
    }
}