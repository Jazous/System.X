namespace System.X.Text
{
    public sealed class HtmlHelper
    {

        internal static readonly HtmlHelper Instance = new HtmlHelper();

        static readonly string[] _headerEncodingTable = new string[] {
            "%00", "%01", "%02", "%03", "%04", "%05", "%06", "%07", "%08", "%09", "%0a", "%0b", "%0c", "%0d", "%0e", "%0f",
            "%10", "%11", "%12", "%13", "%14", "%15", "%16", "%17", "%18", "%19", "%1a", "%1b", "%1c", "%1d", "%1e", "%1f"
         };
        private HtmlHelper() { }

        public string HtmlAttributeEncode(string value)
        {
            return System.Web.HttpUtility.HtmlAttributeEncode(value);
        }
        /// <summary>
        /// 将已经为 HTTP 传输进行过 HTML 编码的字符串转换为已解码的字符串。
        /// </summary>
        /// <param name="value">要解码的字符串。</param>
        /// <returns>一个已解码的字符串。</returns>
        public string HtmlDecode(string value)
        {
            return global::System.Net.WebUtility.HtmlDecode(value);
        }
        /// <summary>
        /// 将字符串转换为 HTML 编码的字符串。
        /// </summary>
        /// <param name="value">要编码的字符串。</param>
        /// <returns>一个已编码的字符串。</returns>
        public string HtmlEncode(string value)
        {
            return global::System.Net.WebUtility.HtmlEncode(value);
        }

        public string JavaScriptStringEncode(string value)
        {
            return System.Web.HttpUtility.JavaScriptStringEncode(value, false);
        }

        public string FormatPlainTextAsHtml(string value)
        {
            if (value == null)
            {
                return null;
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter output = new System.IO.StringWriter(sb);
            FormatPlainTextAsHtml(value, output);
            return sb.ToString();
        }
        void FormatPlainTextAsHtml(string value, System.IO.TextWriter output)
        {
            int length = value.Length;
            char ch = '\0';
            for (int i = 0; i < length; i++)
            {
                char ch2 = value[i];
                switch (ch2)
                {
                    case '\r': break;
                    case '\n': output.Write("<br>"); break;
                    case '<': output.Write("&lt;"); break;
                    case '>': output.Write("&gt;"); break;
                    case '"': output.Write("&quot;"); break;
                    case '&': output.Write("&amp;"); break;
                    case ' ':
                        if (ch == ' ') output.Write("&nbsp;");
                        else output.Write(ch2);
                        break;
                    default:
                        if ((ch2 >= '\x00a0') && (ch2 < 'Ā'))
                        {
                            output.Write("&#");
                            output.Write(((int)ch2).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            output.Write(';');
                        }
                        else
                        {
                            output.Write(ch2);
                        }
                        break;
                }
                ch = ch2;
            }
        }

        public System.Collections.Specialized.NameValueCollection ParseQueryString(string value)
        {
            return System.Web.HttpUtility.ParseQueryString(value, System.Text.Encoding.UTF8);
        }
        public System.Collections.Specialized.NameValueCollection ParseQueryString(string value, System.Text.Encoding encoding)
        {
            return System.Web.HttpUtility.ParseQueryString(value, encoding);
        }

        public string HeaderEncode(string value)
        {
            string str = value;
            if (!HeaderValueNeedsEncoding(value))
            {
                return str;
            }
            var builder = new System.Text.StringBuilder();
            foreach (char ch in value)
            {
                if ((ch < ' ') && (ch != '\t'))
                {
                    builder.Append(_headerEncodingTable[ch]);
                }
                else if (ch == '\x007f')
                {
                    builder.Append("%7f");
                }
                else
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }
        bool HeaderValueNeedsEncoding(string value)
        {
            foreach (char ch in value)
            {
                if (((ch < ' ') && (ch != '\t')) || (ch == '\x007f'))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        ///  对 URL 字符串进行编码。
        /// </summary>
        /// <param name="value">要编码的文本。</param>
        /// <returns>一个已编码的字符串。</returns>
        public string UrlEncode(string value)
        {
            return System.Web.HttpUtility.UrlEncode(value, System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 使用指定的编码对象对 URL 字符串进行编码。
        /// </summary>
        /// <param name="value">要编码的文本。</param>
        /// <param name="encoding">指定编码方案的 System.Text.Encoding 对象。</param>
        /// <returns>一个已编码的字符串。</returns>
        public string UrlEncode(string value, System.Text.Encoding encoding)
        {
            return System.Web.HttpUtility.UrlEncode(value, encoding);
        }
        /// <summary>
        /// 将已经为在 URL 中传输而编码的字符串转换为解码的字符串。
        /// </summary>
        /// <param name="value">要解码的字符串。</param>
        /// <returns>一个已解码的字符串。</returns>
        public string UrlDecode(string value)
        {
            return System.Web.HttpUtility.UrlDecode(value, System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 使用指定的解码对象将 URL 编码的字节数组转换为已解码的字符串。
        /// </summary>
        /// <param name="value">要解码的字节数组。</param>
        /// <param name="encoding">指定解码方法的 System.Text.Encoding。</param>
        /// <returns></returns>
        public string UrlDecode(string value, System.Text.Encoding encoding)
        {
            return System.Web.HttpUtility.UrlDecode(value, encoding);
        }
        public string UrlTokenDecode(string value)
        {
            return UrlTokenDecode(value, System.Text.Encoding.UTF8);
        }
        public string UrlTokenDecode(string value, System.Text.Encoding encoding)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            int index = value.Length - 1;
            int num2 = value[index] - '0';
            if ((num2 < 0) || (num2 > 10))
            {
                return string.Empty;
            }
            char[] inArray = new char[index + num2];
            for (int i = 0; i < index; i++)
            {
                char ch = value[i];
                switch (ch)
                {
                    case '-': inArray[i] = '+'; break;
                    case '_': inArray[i] = '/'; break;
                    default: inArray[i] = ch; break;
                }
            }
            for (int j = index; j < inArray.Length; j++)
            {
                inArray[j] = '=';
            }
            return encoding.GetString(System.Convert.FromBase64CharArray(inArray, 0, inArray.Length));
        }
        public string UrlTokenEncode(string value)
        {
            return UrlTokenEncode(value, System.Text.Encoding.UTF8);
        }
        public string UrlTokenEncode(string value, System.Text.Encoding encoding)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            string str = System.Convert.ToBase64String(encoding.GetBytes(value));
            int index = str.Length;
            while (index > 0)
            {
                if (str[index - 1] != '=')
                {
                    break;
                }
                index--;
            }
            char[] chArray = new char[index + 1];
            chArray[index] = (char)((0x30 + str.Length) - index);
            for (int i = 0; i < index; i++)
            {
                char ch = str[i];
                switch (ch)
                {
                    case '+': chArray[i] = '-'; break;
                    case '/': chArray[i] = '_'; break;
                    case '=': chArray[i] = ch; break;
                    default: chArray[i] = ch; break;
                }
            }
            return new string(chArray);
        }
        /// <summary>
        /// 对 URL 字符串的路径部分进行编码，以进行从 Web 服务器到客户端的可靠的 HTTP 传输。
        /// </summary>
        /// <param name="value">要进行 URL 编码的文本。</param>
        /// <returns>URL 编码的文本。</returns>
        public string UrlPathEncode(string value)
        {
            return System.Web.HttpUtility.UrlPathEncode(value);
        }


        static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + 0x30);
            }
            return (char)((n - 10) + 0x61);
        }
        static int HexToInt(char h)
        {
            if ((h >= '0') && (h <= '9'))
            {
                return (h - '0');
            }
            if ((h >= 'a') && (h <= 'f'))
            {
                return ((h - 'a') + 10);
            }
            if ((h >= 'A') && (h <= 'F'))
            {
                return ((h - 'A') + 10);
            }
            return -1;
        }
    }
}