namespace System
{
    /// <summary>
    /// Integer prefix string comparer suport 9 bit number for special sorting.
    /// </summary>
    public sealed class StringIntegerComparer : System.Collections.Generic.IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (string.IsNullOrEmpty(x)) return string.IsNullOrEmpty(y) ? 0 : -1;
            if (string.IsNullOrEmpty(y)) return string.IsNullOrEmpty(x) ? 0 : 1;

            int xi = GetInt32Length(x);
            int yi = GetInt32Length(y);

            if (xi != -1 && yi != -1)
            {
                int x1 = int.Parse(x.Substring(0, xi + 1));
                int y1 = int.Parse(y.Substring(0, yi + 1));
                if (x1 == y1)
                    return x.Substring(xi + 1).CompareTo(y.Substring(yi + 1));
                return x1.CompareTo(y1);
            }
            if (xi == -1 && yi != -1)
                return -1;
            if (xi != -1 && yi == -1)
                return 1;

            return x.CompareTo(y);
        }
        static int GetInt32Length(string value)
        {
            int len = value.Length > 9 ? 9 : value.Length;
            char ch;
            int i = 0;
            for (; i < len; i++)
            {
                ch = value[i];
                switch (ch)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        continue;
                    default:
                        return i - 1;
                }
            }
            switch (value[0])
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return len - 1;
                default:
                    return -1;
            }
        }
    }
}