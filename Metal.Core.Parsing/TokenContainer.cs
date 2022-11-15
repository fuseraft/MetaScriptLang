namespace Metal.Core.Parsing
{
    public class TokenContainer
    {
        private int currentIndex = 0;
        private int amountMoved = 0;

        public TokenContainer(IList<string> tokens)
        {
            Tokens.AddRange(tokens);
        }

        public int Length => Tokens.Count;

        public int CurrentIndex => currentIndex;

        public bool EndOfContainer => CurrentIndex + 1 >= Length;

        public int AmountMoved => amountMoved;

        public string Current()
        {
            return Tokens[currentIndex];
        }

        public string Next()
        {
            if (EndOfContainer)
            {
                return string.Empty;
            }

            currentIndex++;
            amountMoved++;
            return Tokens[currentIndex];
        }

        public string PeekPrevious()
        {
            if (currentIndex == 0)
            {
                return string.Empty;
            }

            return Tokens[currentIndex - 1];
        }

        public string PeekNext()
        {
            if (EndOfContainer)
            {
                return string.Empty;
            }

            return Tokens[currentIndex + 1];
        }

        public int Skip(int count)
        {
            var newIndex = CurrentIndex + count;

            if (newIndex >= Length)
            {
                return -1;
            }

            currentIndex = newIndex;
            return count;
        }

        public List<string> Tokens { get; } = new List<string>();
    }
}
