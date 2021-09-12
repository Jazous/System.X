
using System.Threading.Tasks;

namespace System
{
    public static class XExtension
    {
        public static bool EqualsIgC(this string source, string value)
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
        public static string TrimStart(this string source, string value, bool ignoreCase)
        {
            return TrimStart(source, value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }
        static string TrimStart(this string source, string value, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            if (source.StartsWith(value, comparisonType))
                return TrimStart(source.Substring(0, value.Length), value, comparisonType);
            return source;
        }
        public static string TrimEnd(this string source, string value, bool ignoreCase)
        {
            return TrimEnd(source, value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }
        static string TrimEnd(this string source, string value, StringComparison comparisonType)
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
            return default;
        }


        public static string ToString(this DateTime? source, DateFormat format)
        {
            return source == null ? string.Empty : ToString(source.Value, format);
        }
        public static string ToString(this DateTime source, DateFormat format)
        {
            switch (format)
            {
                case DateFormat.Date: return source.ToString("yyyy-MM-dd");
                case DateFormat.DateTime: return source.ToString("yyyy-MM-dd HH:mm:ss");
                case DateFormat.SDate: return source.ToString("MM-dd");
                case DateFormat.SDateSTime: return source.ToString("MM-dd HH:mm");
                case DateFormat.SDateTime: return source.ToString("yyyy-MM-dd HH:mm");
                case DateFormat.STime: return source.ToString("HH:mm");
                case DateFormat.Time: return source.ToString("HH:mm:ss");
                case DateFormat.TimeSpan: return source.ToString("yyyyMMddHHmmssttt");
                default: return source.ToString();
            }
        }

        public static byte[] GetBytes(this System.Drawing.Image source)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                source.Save(ms, source.RawFormat);
                return ms.GetBuffer();
            }
        }
    }
}