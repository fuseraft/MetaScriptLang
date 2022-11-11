namespace Metal.Core.Parsing.Keywords
{
    public partial class KeywordLookup
    {
        public static bool IsKeyword(string word) =>
            IsModifierKeyword(word)
            || IsFlowKeyword(word)
            || IsTerminatorKeyword(word)
            || IsLibraryKeyword(word)
            || IsBlockKeyword(word)
            || IsDataKeyword(word)
            || IsComparisonOperator(word)
            || IsAssignmentOperator(word)
            || IsBuiltin(word)
            || IsMathBuiltin(word)
            || IsGrouper(word);

        public static bool IsModifierKeyword(string word) => ModifierKeywords.Contains(word);

        public static bool IsFlowKeyword(string word) => FlowKeywords.Contains(word);

        public static bool IsTerminatorKeyword(string word) => TerminatorKeywords.Contains(word);

        public static bool IsLibraryKeyword(string word) => LibraryKeywords.Contains(word);

        public static bool IsBlockKeyword(string word) => BlockKeywords.Contains(word);

        public static bool IsDataKeyword(string word) => DataKeywords.Contains(word);

        public static bool IsComparisonOperator(string word) => ComparisonOperators.Contains(word);

        public static bool IsAssignmentOperator(string word) => AssignmentOperators.Contains(word);

        public static bool IsBuiltin(string word) => Builtins.Contains(word);

        public static bool IsMathBuiltin(string word) => MathBuiltins.Contains(word);

        public static bool IsGrouper(string word) => Groupers.Contains(word);
    }
}
