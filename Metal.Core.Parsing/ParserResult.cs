namespace Metal.Core.Parsing
{
    public class ParserResult
    {
        public bool Success { get; set; }

        public static ParserResult SuccessResult => new ParserResult { Success = true };

        public static ParserResult FailResult => new ParserResult { Success = false };
    }
}
