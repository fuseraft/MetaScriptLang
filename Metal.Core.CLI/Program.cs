namespace Metal.Core.CLI
{
    using Metal.Core.Parsing;

    public class Program
    {
        public static int Main(string[] args)
        {
            args = new string[]
            {
                ".\\helloworld.metal",
            };

            return args.Length > 0 ? ConsoleMode(args) : ReplMode();
        }

        private static int ConsoleMode(string[] args)
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

        private static int ReplMode()
        {
            Parser.ReplEnabled = true;
            int result = 0;
            bool running = true;

            while (running)
            {
                Console.Write("> ");
                var line = Console.ReadLine();

                if (line.Trim().ToLower() == "exit")
                {
                    running = false;
                }
                else
                {
                    Parser.ReplParse(line);
                }
            }

            return result;
        }
    }
}