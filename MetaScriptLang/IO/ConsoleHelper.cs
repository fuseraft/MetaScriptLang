namespace MetaScriptLang.IO
{
    public class ConsoleHelper
    {
        public static string Output
        {
            set
            {
                Console.Write(value);
            }
        }

        public static string Error
        {
            set
            {
                Console.Error.Write(value);
            }
        }

        public static string GetLine()
        {
            return Console.ReadLine() ?? string.Empty;
        }

        public static string RunAndCaptureOutput(string command)
        {
            // execute process and return result of stdout
            return string.Empty;
        }
    }
}
