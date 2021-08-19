namespace System.X.Text
{
    public sealed class TextHelper
    {
        static internal readonly TextHelper Instance = new TextHelper();
        private TextHelper() { }

        public string Alphabet { get => "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz"; }

        public string ToBitString(byte[] value)
        {
            global::System.Int32 length = value.Length * 2;
            char[] chars = new char[length];
            for (global::System.Int32 i = 0, j = 0; i < length; i += 2, j++)
            {
                byte tmp = value[j];
                chars[i] = ToHexValue(tmp / 0x10);
                chars[i + 1] = ToHexValue(tmp % 0x10);
            }
            return new string(chars, 0, length);
        }
        public char ToHexValue(global::System.Int32 i)
        {
            return i < 10 ? (char)(i + 0x30) : (char)((i - 10) + 0x41);
        }

        public bool IsMatch(string source, string pattern, bool ignoreCase = false)
        {
            return source == null ? false : System.Text.RegularExpressions.Regex.IsMatch(source, pattern, ignoreCase ? System.Text.RegularExpressions.RegexOptions.Compiled : (System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase));
        }
        public string Match(string source, string pattern, bool ignoreCase = false)
        {
            if (source == null)
                return string.Empty;
            var match = System.Text.RegularExpressions.Regex.Match(source, pattern, ignoreCase ? System.Text.RegularExpressions.RegexOptions.Compiled : (System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase));
            return match.Success ? match.Value : string.Empty;
        }
        /// <summary>
        /// 半角转全角(SBC case)
        /// </summary>
        /// <param name="source">任意字符串</param>
        /// <returns>全角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>        
        public string ToSBC(string source)
        {
            char[] c = source.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }
        /// <summary>
        /// 全角转半角(DBC case)
        /// </summary>
        /// <param name="source">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public string ToDBC(string source)
        {
            char[] c = source.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
        /// <summary> 
        /// 取单个字符的拼音声母 
        /// </summary> 
        /// <param name="c">要转换的单个汉字</param> 
        /// <returns>拼音声母。</returns> 
        private string GetPYChar(string c)
        {
            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(c);
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
            if (i < 0xB0A1) return "*";
            if (i < 0xB0C5) return "A";
            if (i < 0xB2C1) return "B";
            if (i < 0xB4EE) return "C";
            if (i < 0xB6EA) return "D";
            if (i < 0xB7A2) return "E";
            if (i < 0xB8C1) return "F";
            if (i < 0xB9FE) return "G";
            if (i < 0xBBF7) return "H";
            if (i < 0xBFA6) return "G";
            if (i < 0xC0AC) return "K";
            if (i < 0xC2E8) return "L";
            if (i < 0xC4C3) return "M";
            if (i < 0xC5B6) return "N";
            if (i < 0xC5BE) return "O";
            if (i < 0xC6DA) return "P";
            if (i < 0xC8BB) return "Q";
            if (i < 0xC8F6) return "R";
            if (i < 0xCBFA) return "S";
            if (i < 0xCDDA) return "T";
            if (i < 0xCEF4) return "W";
            if (i < 0xD1B9) return "X";
            if (i < 0xD4D1) return "Y";
            if (i < 0xD7FA) return "Z";
            return "*";
        }
    }
}