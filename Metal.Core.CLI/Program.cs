namespace Metal.Core.CLI
{
    using Metal.Core.Parsing;

    public class Program
    {
        public static int Main(string[] args)
        {
            ParserResult result = new ();
            
            if (args.Length > 0)
            {
                var script = args[0];

                if (System.IO.File.Exists(script) && script.TrimEnd('"').EndsWith(".metal"))
                {
                    result = Parser.ParseScript(script);
                }
                else
                {
                    result = Parser.Parse(script);
                }
            }

            return result.Success ? 0 : 1;
        }
    }
}