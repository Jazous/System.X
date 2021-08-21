namespace System
{
    public static class XExtension
    {
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }
        public static bool EqualsIgnoreCase(this string source, string value)
        {
            return string.Equals(source, value, StringComparison.OrdinalIgnoreCase);
        }
        public static int? ToInt32(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return 0;
            int result;
            if (int.TryParse(source, Globalization.NumberStyles.Integer, Globalization.NumberFormatInfo.CurrentInfo, out result))
                return result;
            return null;
        }
        public static string TrimStart(this string source, string value, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            int vlen = value.Length;
            string str = source;
            var ctype = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            while (str.StartsWith(value, ctype))
                str = str.Substring(0, vlen);
            return str;
        }
        public static string TrimEnd(this string source, string value, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            if (string.IsNullOrEmpty(value))
                return source;

            string str = source;
            int vlen = value.Length;
            var ctype = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            while (str.EndsWith(value, ctype))
                str = str.Substring(str.Length - vlen, vlen);
            return str;
        }

        public static string ToString(this DateTime? source, string format)
        {
            return source != null ? source.Value.ToString(format) : string.Empty;
        }
        /// <summary>
        /// 格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToString(this DateTime? source)
        {
            return source != null ? ToString(source.Value) : string.Empty;
        }
        /// <summary>
        /// 格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="source">日期。</param>
        /// <returns></returns>
        public static string ToStringG(this DateTime source)
        {
            return source.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string ToStringG(this DateTime? source)
        {
            return source == null ? string.Empty : ToStringG(source.Value);
        }
        /// <summary>
        /// 格式：yyyy-MM-dd
        /// </summary>
        /// <param name="source">日期。</param>
        /// <returns></returns>
        public static string ToStringD(this DateTime source)
        {
            return source.ToString("yyyy-MM-dd");
        }
        public static string ToStringD(this DateTime? source)
        {
            return source == null ? string.Empty : ToStringD(source.Value);
        }
        /// <summary>
        /// 返回日期格式：MM-dd、yyyy-MM-dd
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToShowDate(this DateTime? source)
        {
            return source != null ? ToShowDate(source.Value) : string.Empty;
        }
        /// <summary>
        /// 返回日期格式：MM-dd、yyyy-MM-dd
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToShowDate(this DateTime source)
        {
            DateTime today = DateTime.Today;
            if (source.Year == today.Year)
            {
                return source.ToString("MM-dd");
            }

            return source.ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 返回日期格式：昨天、今天、明天、MM月dd日、yyyy年MM月dd日
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToShowDate_CN(this DateTime? source)
        {
            return source != null ? ToShowDate_CN(source.Value) : string.Empty;
        }
        /// <summary>
        /// 返回日期格式：昨天、今天、明天、MM月dd日、yyyy年MM月dd日
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToShowDate_CN(this DateTime source)
        {
            DateTime today = DateTime.Today;
            if (source.Year == today.Year)
            {
                int days = (today - source).Days;
                switch (days)
                {
                    case -1: return "明天";
                    case 0: return "今天";
                    case 1: return "昨天";
                    default: return source.ToString("MM月dd日");
                }
            }
            return source.ToString("yyyy年MM月dd日");
        }

        public static string GetDescription(this Enum value)
        {
            string text = value.ToString();
            System.Reflection.FieldInfo field = value.GetType().GetField(text);
            object[] attrs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            return attrs.Length == 0 ? text : ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
        }

        public static T Get<T>(this System.Collections.Generic.IDictionary<string, T> source, string key) where T : class
        {
            T result;
            return source.TryGetValue(key, out result) ? result : null;
        }
        public static string Get(this System.Collections.Generic.IDictionary<string, string> source, string key)
        {
            string result;
            return source.TryGetValue(key, out result) ? result : string.Empty;
        }
    }
}