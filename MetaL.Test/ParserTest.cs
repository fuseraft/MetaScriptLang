namespace MetaL.Test
{
    using Metal.Core.Parsing;

    public class Tests
    {
        private string ScriptData { get; set; } = string.Empty;

        [SetUp]
        public void Setup()
        {
            string scriptFile = "helloworld.metal";
            ScriptData = File.ReadAllText(scriptFile);
        }

        [Test]
        public void TestScriptLoad()
        {
            Assert.IsTrue(!string.IsNullOrEmpty(ScriptData));
        }

        [Test]
        public void TestParseScript()
        {
            var result = Parser.Parse(ScriptData);
            Assert.IsTrue(result.Success);
        }
    }
}