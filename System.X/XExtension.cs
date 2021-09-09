namespace System
{
    public static class XExtension
    {
        public static bool EqualsIgnoreCase(this string source, string value)
        {
            return string.Equals(source, value, StringComparison.OrdinalIgnoreCase);
        }
        public static int? ToInt32(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return 0;
            int result;
            if (int.TryParse(source, out result))
                return result;
            return null;
        }
        public static string TrimStart(this string source, string value, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            if (source.StartsWith(value, comparisonType))
                return TrimStart(source.Substring(0, value.Length), value, comparisonType);
            return source;
        }
        public static string TrimEnd(this string source, string value, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            if (source.EndsWith(value, comparisonType))
                return TrimStart(source.Substring(source.Length - value.Length, value.Length), value, comparisonType);
            return source;
        }
        public static TSource Get<TKey, TSource>(this Collections.Generic.IDictionary<TKey, TSource> source, TKey key)
        {
            TSource result;
            if (source.TryGetValue(key, out result))
                return result;
            return default(TSource);
        }


        public static string ToString(this DateTime? source, DateFormats format)
        {
            return source == null ? string.Empty : ToString(source.Value, format);
        }
        public static string ToString(this DateTime source, DateFormats format)
        {
            switch (format)
            {
                case DateFormats.Date: return source.ToString("yyyy-MM-dd");
                case DateFormats.DateTime: return source.ToString("yyyy-MM-dd HH:mm:ss");
                case DateFormats.SDate: return source.ToString("MM-dd");
                case DateFormats.SDateSTime: return source.ToString("MM-dd HH:mm");
                case DateFormats.SDateTime: return source.ToString("yyyy-MM-dd HH:mm");
                case DateFormats.STime: return source.ToString("HH:mm");
                case DateFormats.Time: return source.ToString("HH:mm:ss");
                case DateFormats.TimeSpan: return source.ToString("yyyyMMddHHmmssttt");
                default: return source.ToString();
            }
        }
    }
}