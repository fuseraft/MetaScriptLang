﻿namespace MetaScriptLangCLI
{
    using MetaScriptLang;

    public class Program
    {
        public static void Main(string[] args)
        {
            var engine = new Engine();
            engine.Run(args.Length, args);
        }
    }
}