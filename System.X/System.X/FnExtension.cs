namespace System
{
    public static class FnExtension
    {
        public static int ToInt32(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return 0;
            int result;
            if (int.TryParse(source, Globalization.NumberStyles.Integer, Globalization.NumberFormatInfo.CurrentInfo, out result))
                return result;
            return 0;
        }
        public static string Trim(this string source, string value)
        {
            return source.TrimStart(value).TrimEnd(value);
        }
        public static string TrimStart(this string source, string value)
        {
            if (value == null)
            {
                return source;
            }
            int vLength = value.Length;
            if (vLength == 0)
            {
                return source;
            }
            int sLength = source.Length;
            if (sLength == 0)
            {
                return string.Empty;
            }
            if (sLength < vLength)
            {
                return source;
            }
            int index = 0;
            if (vLength == 1)
            {
                char ch = value[0];
                for (; index < sLength; index++)
                {
                    if (source[index] != ch)
                    {
                        return index != 0 ? source.Remove(0, index) : source;
                    }
                }
                return source.Remove(0, index);
            }
            int j = 0;
            do
            {
                for (j = 0; j < vLength; j++)
                {
                    if (source[index + j] != value[j])
                    {
                        return index != 0 ? source.Remove(0, index) : source;
                    }
                }
                index += vLength;
            }
            while (index + vLength <= sLength);
            return source.Remove(0, index);
        }
        public static string TrimEnd(this string source, string value)
        {
            if (value == null)
            {
                return source;
            }
            int vLength = value.Length;
            if (vLength == 0)
            {
                return source;
            }
            int sLength = source.Length;
            if (sLength == 0)
            {
                return string.Empty;
            }
            int index = sLength - vLength;
            if (index < 0)
            {
                return source;
            }
            if (vLength == 1)
            {
                char ch = value[0];
                int sLIndex = sLength - 1;
                if (ch != source[sLIndex])
                {
                    return source;
                }
                sLIndex--;
                for (int i = sLIndex; i > -1; i--)
                {
                    if (source[i] != ch)
                    {
                        return source.Remove(i + 1, sLength - i - 1);
                    }
                }
                return string.Empty;
            }
            int vLIndex = vLength - 1;
            do
            {
                for (int i = vLIndex; i > -1; i--)
                {
                    if (source[index + i] != value[i])
                    {
                        return index != sLength - vLength ? source.Remove(index + vLength, sLength - index - vLength) : source;
                    }
                }
                index -= vLength;
            }
            while (index > -1);
            return source.Remove(index + vLength, sLength - index - vLength);
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
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToString(this DateTime source)
        {
            return source.ToString("yyyy-MM-dd HH:mm:ss");
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

        public static string Remove(this string source, string value)
        {
            if (value == null)
            {
                return source;
            }
            int vLength = value.Length;
            if (vLength == 0)
            {
                return source;
            }
            int sLength = source.Length;
            if (sLength == 0)
            {
                return string.Empty;
            }
            if (sLength < vLength)
            {
                return source;
            }
            return source.Replace(value, string.Empty);
        }

        public static string GetDescription(this Enum value)
        {
            string text = value.ToString();
            System.Reflection.FieldInfo field = value.GetType().GetField(text);
            object[] attrs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            return attrs.Length == 0 ? text : ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
        }

        public static TValue Get<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> source, TKey key, TValue defaultValue)
        {
            TValue result;
            return source.TryGetValue(key, out result) ? result : defaultValue;
        }
        public static TValue Get<TValue>(this System.Collections.Generic.IDictionary<string, TValue> source, string key) where TValue : class
        {
            TValue result;
            return source.TryGetValue(key, out result) ? result : null;
        }
        public static string Get(this System.Collections.Generic.IDictionary<string, string> source, string key)
        {
            string result;
            return source.TryGetValue(key, out result) ? result : string.Empty;
        }
    }
}