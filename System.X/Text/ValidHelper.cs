using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.X.Text
{
    public sealed class ValidHelper
    {
        internal static readonly ValidHelper Instance = new ValidHelper();
        private ValidHelper() { }

        public bool IsIpAddress(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public bool IsMatch(string source, string pattern, bool ignoreCase = false)
        {
            return source == null ? false : System.Text.RegularExpressions.Regex.IsMatch(source, pattern, ignoreCase ? System.Text.RegularExpressions.RegexOptions.Compiled : (System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase));
        }

        public bool IsEmail(string value)
        {
            return Regex.IsMatch(value, @"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", RegexOptions.IgnoreCase);
        }
        public static bool IsUrl(string value)
        {
            return Regex.IsMatch(value, @"^(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\.))+(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&amp;%_\./-~-]*)?$", RegexOptions.IgnoreCase);
        }
        public static bool IsMobile(string value)
        {
            return Regex.IsMatch(value, @"^1[3456789]\d{9}$", RegexOptions.IgnoreCase); 
        }
        public static bool IsIPv4(string value)
        {
            return Regex.IsMatch(value, @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$", RegexOptions.IgnoreCase);
        }

        public bool IsIDCard(string value)
        {
            switch (value.Length)
            {
                case 15: return IsIDCard15(value);
                case 18: return IsIDCard18(value);
                default: return false;
            }
        }
        bool IsIDCard18(string value)
        {
            long n = 0;
            if (long.TryParse(value.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(value.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(value.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = value.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = value.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != value.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }
        bool IsIDCard15(string value)
        {
            long n = 0;
            if (long.TryParse(value, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(value.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = value.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }

        public static bool IsNormalChar(string value)
        {
            return Regex.IsMatch(value, @"[\w\d_]+", RegexOptions.IgnoreCase);
        }
        public static bool IsAccountName(string value)
        {
            return Regex.IsMatch(value, "^[a-zA-Z]{1}([a-zA-Z0-9]){4,19}$");
        }
    }
}