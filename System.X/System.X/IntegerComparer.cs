namespace System
{
    public sealed class IntegerComparer : System.Collections.Generic.IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null) return y == null ? 0 : -1;
            if (y == null) return x == null ? 0 : 1;

            var xMatches = System.Text.RegularExpressions.Regex.Matches(x, "[0-9]+|[a-zA-Z]+", System.Text.RegularExpressions.RegexOptions.Compiled);
            var yMatches = System.Text.RegularExpressions.Regex.Matches(y, "[0-9]+|[a-zA-Z]+", System.Text.RegularExpressions.RegexOptions.Compiled);
            int minCount = xMatches.Count > yMatches.Count ? yMatches.Count : xMatches.Count;
            int xTempInt, yTempInt;
            int result = 0;
            for (int i = 0; i < minCount; i++)
            {
                if (int.TryParse(xMatches[i].Value, out xTempInt) && int.TryParse(yMatches[i].Value, out yTempInt))
                    result = xTempInt.CompareTo(yTempInt);
                else
                    result = xMatches[i].Value.CompareTo(yMatches[i].Value);

                if (result != 0)
                    return result;
            }
            if (xMatches.Count > yMatches.Count)
                return 1;
            if (yMatches.Count > xMatches.Count)
                return -1;
            return result;
        }
    }
}