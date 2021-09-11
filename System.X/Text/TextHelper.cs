using System.Text.RegularExpressions;

namespace System.X.Text
{
    public sealed class TextHelper
    {
        internal static readonly TextHelper Instance = new TextHelper();
        private TextHelper() { }

        static readonly string _base32 = "0123456789AaBbCcDdEeFfGgHhJjKkMmNnPpQqRrSsTtUuVvWwXxYyZz";

        public Int64 NewUniqueId()
        {
            return BitConverter.ToInt64(NewSequentialGuid().ToByteArray(), 0);
        }
        public Guid NewSequentialGuid()
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();
            DateTime baseDate = new DateTime(1900, 1, 1);
            DateTime now = DateTime.Now;
            TimeSpan days = new TimeSpan(now.Ticks - baseDate.Ticks);
            TimeSpan msecs = now.TimeOfDay;
            byte[] daysArray = BitConverter.GetBytes(days.Days);
            byte[] msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);
            return new Guid(guidArray);
        }
        public string NewGuid(string format)
        {
            return Guid.NewGuid().ToString(format);
        }
        public string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
        public int NewRandom(int minValue, int maxValue)
        {
            byte[] bytes = new byte[4];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                rng.GetBytes(bytes);
            return new Random(BitConverter.ToInt32(bytes, 0)).Next(minValue, maxValue);
        }
        string NewVercode(int length = 4)
        {
            byte[] bytes = new byte[4];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                rng.GetBytes(bytes);
            var random = new Random(BitConverter.ToInt32(bytes, 0));
            var chs = new char[length];
            var len = _base32.Length;
            for (int i = 0; i < chs.Length; i++)
                chs[i] = _base32[random.Next(0, len)];
            return new string(chs);
        }

        public string ToBitString(byte[] value)
        {
            int length = value.Length * 2;
            char[] chars = new char[length];
            for (int i = 0, j = 0; i < length; i += 2, j++)
            {
                byte tmp = value[j];
                chars[i] = ToHexValue(tmp / 0x10);
                chars[i + 1] = ToHexValue(tmp % 0x10);
            }
            return new string(chars, 0, length);
        }
        public char ToHexValue(int i)
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
        string ToSBC(string source)
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
        string ToDBC(string source)
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
        public string ToUpperCaseCN(decimal value)
        {
            string[] numList = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string[] unitList = { "分", "角", "元", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟" };

            decimal money = value;
            if (money == 0)
            {
                return "零元整";
            }

            var strMoney = new System.Text.StringBuilder();
            //只取小数后2位



            string strNum = decimal.Truncate(money * 100).ToString();

            int len = strNum.Length;
            int zero = 0;
            for (int i = 0; i < len; i++)
            {
                int num = int.Parse(strNum.Substring(i, 1));
                int unitNum = len - i - 1;

                if (num == 0)
                {
                    zero++;
                    if (unitNum == 2 || unitNum == 6 || unitNum == 10)
                    {
                        if (unitNum == 2 || zero < 4)
                            strMoney.Append(unitList[unitNum]);
                        zero = 0;
                    }
                }
                else
                {

                    if (zero > 0)
                    {
                        strMoney.Append(numList[0]);
                        zero = 0;
                    }
                    strMoney.Append(numList[num]);
                    strMoney.Append(unitList[unitNum]);
                }

            }
            if (zero > 0)
                strMoney.Append("整");

            return strMoney.ToString();
        }

        public string RMB(decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字 
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字 
            string str3 = "";    //从原num值中取出的值 
            string str4 = "";    //数字的字符串形式 
            string str5 = "";  //人民币大写金额形式 
            int i;    //循环变量 
            int j;    //num的值乘以100的字符串长度 
            string ch1 = "";    //数字的汉语读法 
            string ch2 = "";    //数字位的汉字读法 
            int nzero = 0;  //用来计算连续的零值是几个 
            int temp;            //从原num值中取出的值 

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数 
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式 
            j = str4.Length;      //找出最高位 
            if (j > 15) { return "溢出"; }
            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分 

            //循环取出每一位需要转换的值 
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值 
                temp = Convert.ToInt32(str3);      //转换为数字 
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时 
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位 
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3))
                {
                    //如果该位是亿位或元位，则必须写上 
                    ch2 = str2.Substring(i, 1);
                }
                str5 = str5 + ch1 + ch2;

                if (i == j - 1 && str3 == "0")
                {
                    //最后一位（分）为0时，加上“整” 
                    str5 = str5 + '整';
                }
            }
            if (num == 0)
            {
                str5 = "零元整";
            }
            return str5;
        }
    }
}