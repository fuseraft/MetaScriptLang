namespace Metal.Core.Parsing.Keywords
{
    public partial class KeywordLookup
    {
        private static HashSet<string> ModifierKeywords { get; } = new HashSet<string>()
        {
            "private",
            "public",
            "protected",
        };

        private static HashSet<string> FlowKeywords { get; } = new HashSet<string>()
        {
            "if",
            "else",
            "elsif",
            "case",
            "when",
            "then",
            "for",
            "foreach",
            "while",
            "do",
            "break",
            "continue",
            "in",
            "is",
            "like",
            "not",
            "try",
            "catch",
        };

        public static HashSet<string> TerminatorKeywords { get; } = new HashSet<string>()
        {
            "end",
        };

        private static HashSet<string> LibraryKeywords { get; } = new HashSet<string>()
        {
            "import",
            "from",
        };

        private static HashSet<string> BlockKeywords { get; } = new HashSet<string>()
        {
            "module",
            "class",
            "method",
        };

        private static HashSet<string> DataKeywords { get; } = new HashSet<string>()
        {
            "as",
            "string",
            "date",
            "number",
            "null",
            "list",
            "map",
            "set",
            "property",
        };

        private static HashSet<string> ComparisonOperators { get; } = new HashSet<string>()
        {
            "<",
            "<=",
            ">",
            ">=",
            "==",
            "!=",
        };

        private static HashSet<string> AssignmentOperators { get; } = new HashSet<string>()
        {
            "=",
            "+=",
            "-=",
            "*=",
            "%=",
            "/=",
            "**=",
            "+",
            "-",
            "*",
            "**",
            "/",
            "%",
            "++=",
            "--=",
        };

        private static HashSet<string> Groupers { get; } = new HashSet<string>
        {
            "(",
            ")",
            "[",
            "]",
        };

        private static HashSet<string> Builtins { get; } = new HashSet<string>()
        {
            "write",
            "writeline",
            "readkey",
            "readline",
            "new",
            "error",
            "tostring",
            "tonumber",
        };

        private static HashSet<string> MathBuiltins { get; } = new HashSet<string>()
        {
            "abs",
            "acos",
            "acosh",
            "asin",
            "asinh",
            "atan",
            "atan2",
            "atanh",
            "bigmul",
            "bitdecrement",
            "bitincrement",
            "cbrt",
            "ceiling",
            "clamp",
            "copysign",
            "cos",
            "cosh",
            "divrem",
            "exp",
            "e",
            "floor",
            "fusedmultiplyadd",
            "ieeeremainder",
            "ilogb",
            "log",
            "log10",
            "log2",
            "max",
            "maxmagnitude",
            "min",
            "minmagnitude",
            "pow",
            "pi",
            "reciprocalestimate",
            "reciprocalsqrtestimate",
            "round",
            "scaleb",
            "sign",
            "sin",
            "sincos",
            "sinh",
            "sqrt",
            "tan",
            "tanh",
            "tau",
            "truncate",
        };
    }
}
