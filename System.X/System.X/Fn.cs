using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Net.Sockets;

namespace System
{
    public sealed class Fn
    {
        public static Text.Encoding UTF8 = System.Text.Encoding.UTF8;
        /// <summary>
        /// 当前运行的操作系统平台。
        /// </summary>
        public static X.Enums.OSPlatforms OSPlatform { get; }

        static Fn()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                OSPlatform = X.Enums.OSPlatforms.Windows;
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                OSPlatform = X.Enums.OSPlatforms.Linux;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                OSPlatform = X.Enums.OSPlatforms.MacOSX;
            else
                OSPlatform = X.Enums.OSPlatforms.Unknown;
        }

        public static bool ToBoolean(string value)
        {
            switch (value)
            {
                case null:
                case "":
                case "0": return false;
                case "1": return true;
                default: break;
            }
            bool result;
            return bool.TryParse(value, out result) ? result : false;
        }
        public static int ToInt32(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            int result;
            if (int.TryParse(value, Globalization.NumberStyles.Integer, Globalization.NumberFormatInfo.CurrentInfo, out result))
                return result;
            return 0;
        }
        public static decimal ToDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return decimal.Zero;
            decimal result;
            if (decimal.TryParse(value, Globalization.NumberStyles.Integer, Globalization.NumberFormatInfo.CurrentInfo, out result))
                return result;
            return decimal.Zero;
        }
        public static DateTime? ToDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            DateTime result;
            if (DateTime.TryParse(value, out result))
                return result;
            return null;
        }
        public static T? ToEnum<T>(string value) where T : struct
        {
            T result;
            if (Enum.TryParse(value, true, out result))
                return result;
            return null;
        }
        public static Int64 NewUniqueId()
        {
            return BitConverter.ToInt64(NewSequenceGuid().ToByteArray(), 0);
        }
        public static Guid NewSequenceGuid()
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
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
        public static string NewGuid(string format)
        {
            return Guid.NewGuid().ToString(format);
        }
        public static string GetDescription(Enum value)
        {
            string text = value.ToString();
            System.Reflection.FieldInfo field = value.GetType().GetField(text);
            object[] attrs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            return attrs.Length == 0 ? text : ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
        }

        static System.IO.FileHelper Profile { get; set; }
        public static System.Text.HtmlHelper HTML { get; set; }
        public static System.X.Text.TextHelper Text { get; set; }
        public static System.Security.Cryptography.CryptoHelper Crypto { get; set; }
        public static System.X.Net.HttpHelper HTTP { get; set; }


        /// <summary>
        /// 生成随机数。
        /// </summary>
        /// <param name="minValue">最小值。（包括）</param>
        /// <param name="maxValue">最大值。（不包括）</param>
        /// <returns></returns>
        public static int Next(int minValue, int maxValue)
        {
            byte[] bytes = new byte[4];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return new Random(BitConverter.ToInt32(bytes, 0)).Next(minValue, maxValue);
        }

        /// <summary>
        /// 字符串压缩
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Compress(string input)
        {
            string result = string.Empty;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(input);
            using (var outputStream = new System.IO.MemoryStream())
            {
                using (var zipStream = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(outputStream))
                {
                    zipStream.Write(buffer, 0, buffer.Length);
                    zipStream.Close();
                }
                return Convert.ToBase64String(outputStream.ToArray());
            }
        }
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decompress(string input)
        {
            string result = string.Empty;
            byte[] buffer = Convert.FromBase64String(input);
            using (Stream inputStream = new MemoryStream(buffer))
            {
                var zipStream = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(inputStream);

                using (StreamReader reader = new StreamReader(zipStream, System.Text.Encoding.UTF8))
                {
                    //输出
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }

        public static string Zip(string value)
        {
            //Transform string into byte[] 
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }
            //Prepare for compress
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream sw = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress);
            //Compress
            sw.Write(byteArray, 0, byteArray.Length);
            //Close, DO NOT FLUSH cause bytes will go missing...
            sw.Close();
            //Transform byte[] zip data to string
            byteArray = ms.ToArray();
            System.Text.StringBuilder sB = new System.Text.StringBuilder(byteArray.Length);
            foreach (byte item in byteArray)
            {
                sB.Append((char)item);
            }
            ms.Close();
            sw.Dispose();
            ms.Dispose();
            return sB.ToString();
        }
        public static string UnZip(string value)
        {
            //Transform string into byte[]
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }
            //Prepare for decompress
            System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArray);
            System.IO.Compression.GZipStream sr = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress);
            //Reset variable to collect uncompressed result
            byteArray = new byte[byteArray.Length];
            //Decompress
            int rByte = sr.Read(byteArray, 0, byteArray.Length);
            //Transform byte[] unzip data to string
            System.Text.StringBuilder sB = new System.Text.StringBuilder(rByte);
            //Read the number of bytes GZipStream red and do not a for each bytes in
            //resultByteArray;
            for (int i = 0; i < rByte; i++)
            {
                sB.Append((char)byteArray[i]);
            }
            sr.Close();
            ms.Close();
            sr.Dispose();
            ms.Dispose();
            return sB.ToString();
        }


        /// <summary>
        /// 指示指定主机或 Ip 是否是本机。
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static bool IsLocalIpOrHost(string host)
        {
            if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(host, "127.0.0.1"))
                return true;
            if (string.Equals(host, "::1"))
                return true;

            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            var _ips = IpEntry.AddressList.Where(c => c.AddressFamily == AddressFamily.InterNetwork).Select(c => c.ToString()).ToArray();
            return _ips.Any(c => c == host);
        }

        /// <summary>
        /// 拷贝目录及目录下的文件到新目录。
        /// </summary>
        /// <param name="source">需要拷贝的目录。</param>
        /// <param name="dest">拷贝到的目录。</param>
        /// <param name="skipFiles">跳过拷贝的文件名称。</param>
        public static void CopyFolder(string source, string dest, params string[] skipFiles)
        {
            CopyFolder(new DirectoryInfo(source), dest, skipFiles);
        }
        static void CopyFolder(DirectoryInfo source, string dest, params string[] skipFiles)
        {
            if (!source.Exists || source.Attributes.HasFlag(FileAttributes.Hidden))
                return;

            Directory.CreateDirectory(dest);
            var fileInfos = source.GetFiles();
            foreach (var info in fileInfos)
            {
                if (info.Attributes.HasFlag(FileAttributes.Hidden))
                    continue;
                if (skipFiles.Any(c => string.Equals(c, info.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;
                if (info.Exists)
                    info.CopyTo(Path.Combine(dest, info.Name), true);
            }

            var dirInfos = source.GetDirectories();
            foreach (var info in dirInfos)
            {
                CopyFolder(info, Path.Combine(dest, info.Name));
            }
        }

        /// <summary>
        /// 执行命令并退出，支持：cmd、/bin/bash
        /// </summary>
        /// <param name="command">命令。</param>
        /// <returns></returns>
        public static string ExecuteCmd(string command)
        {
            using (var process = new Diagnostics.Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                switch (OSPlatform)
                {
                    case X.Enums.OSPlatforms.Windows:
                        process.StartInfo.FileName = IO.Path.Combine(System.Environment.GetEnvironmentVariable("windir"), @"system32\cmd.exe");
                        break;
                    case X.Enums.OSPlatforms.Linux:
                        process.StartInfo.FileName = "/bin/bash";
                        break;
                    case X.Enums.OSPlatforms.MacOSX:
                        process.StartInfo.FileName = "/bin/bash";
                        break;
                    default:
                        throw new System.InvalidOperationException("Unknown OS Platform!");
                }
                process.Start();
                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine("exit");
                process.WaitForExit();

                return process.StandardOutput.ReadToEnd();
            }
        }

        /// <summary>
        /// 执行脚本或批处理。
        /// </summary>
        /// <param name="name">脚本、批处理名称或路径。</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string ExecuteShell(string name, string args)
        {
            using (var process = new Diagnostics.Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = name;
                process.StartInfo.Arguments = args;
                process.Start();
                process.WaitForExit();
                return process.StandardOutput.ReadToEnd();
            }
        }
    }
}