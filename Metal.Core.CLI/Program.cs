namespace Metal.Core.CLI
{
    using Metal.Core.Objects;
    using Metal.Core.Parsing;
    using System.Text;

    public class Program
    {
        public static void Main(string[] args)
        {
            Module mod_MetalcoreLib = new ("MetalcoreLib");
            Object o_StringContainer = new ("StringContainer", mod_MetalcoreLib);
            Method m_GetSize = new ("GetSize", o_StringContainer, Typing.Enums.ValueType.Number);

            StringBuilder script = new ();

            script.AppendLine("# an example script                     ");
            script.AppendLine("class Person                            ");
            script.AppendLine("  method greet(name = \"Scott\")        ");
            script.AppendLine("    println \"Hello! My name is {name}\"");
            script.AppendLine("  end                                   ");
            script.AppendLine("end                                     ");
            script.AppendLine("                                        ");
            script.AppendLine("# instantiate new Person instance       ");
            script.AppendLine("@p = Person.new()                       ");
            script.AppendLine("@p.greet(\"Scott Christopher Stauffer\")");

            ParserResult result = Parsing.Parser.Parse(script.ToString());

            Console.WriteLine("Press enter to close.");
            Console.ReadLine();
        }
    }
}