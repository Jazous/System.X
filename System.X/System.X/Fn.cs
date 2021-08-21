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
        public static Text.Encoding UTF8 { get => System.Text.Encoding.UTF8; }
        public const string Alphabet = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";

        /// <summary>
        /// 当前运行的操作系统平台。
        /// </summary>
        public static X.Enums.OSPlatforms OSPlatform { get; }

        public static System.X.IO.ProfileHelper Profile { get => System.X.IO.ProfileHelper.Instance; }
        public static System.X.Text.HtmlHelper Html { get => System.X.Text.HtmlHelper.Instance; }
        public static System.X.Text.TextHelper Text { get => System.X.Text.TextHelper.Instance; }
        public static System.X.Cryptography.CryptoHelper Crypto { get => System.X.Cryptography.CryptoHelper.Instance; }
        public static System.X.Net.HttpHelper Http { get => X.Net.HttpHelper.Instance; }
        public static System.X.IO.CompressionHelper Compress { get => X.IO.CompressionHelper.Instance; }

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
        public static int? ToInt32(string value)
        {
            return XExtension.ToInt32(value);
        }
        public static decimal? ToDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return decimal.Zero;
            decimal result;
            if (decimal.TryParse(value, Globalization.NumberStyles.Integer, Globalization.NumberFormatInfo.CurrentInfo, out result))
                return result;
            return null;
        }
        public static DateTime? ToDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            DateTime result;

            if (DateTime.TryParse(value, out result))
                return result;
            if (DateTime.TryParseExact(value, new string[] { "yyyy/MM/dd HH:mm", "yyyy-MM-dd HH:mm", "yyyy/MM/dd HH", "yyyy-MM-dd HH" }, Globalization.DateTimeFormatInfo.CurrentInfo, Globalization.DateTimeStyles.None, out result))
                return result;
            return null;
        }
        public static DateTime? ToDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            DateTime result;
            if (DateTime.TryParse(value, out result))
                return result.Date;
            return null;
        }
        public static T? ToEnum<T>(string value) where T : struct
        {
            T result;
            if (Enum.TryParse(value, true, out result))
                return result;
            return null;
        }
        public static Collections.Generic.List<NameValue> ToList<T>() where T : Enum
        {
            var result = new Collections.Generic.List<NameValue>();
            var values = Enum.GetValues(typeof(T));
            foreach (var item in values)
            {
                var nv = new NameValue();
                var name = item.ToString();
                var descAttr = item.GetType().GetField(name).GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).FirstOrDefault();
                nv.Name = descAttr != null ? ((System.ComponentModel.DescriptionAttribute)descAttr).Description : name;
                nv.Value = Convert.ToInt32(item).ToString();
                result.Add(nv);
            }
            return result;
        }
        public static string NewGuid(string format = "N")
        {
            return Guid.NewGuid().ToString(format);
        }
        public static string NewTempDir()
        {
            string tempDir = string.Concat(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }
        public static async Task<string> NewTempFile(byte[] bytes)
        {
            string tempFile = Path.GetTempFileName();
            using (var fileStream = File.Open(tempFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                await fileStream.WriteAsync(bytes, 0, bytes.Length);
                fileStream.Flush();
            }
            return tempFile;
        }
        public static async Task<string> NewTempFile(Stream stream, bool leaveOpen = false)
        {
            string tempFile = Path.GetTempFileName();
            using (var fileStream = File.Open(tempFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
                fileStream.Flush();
            }
            if (!leaveOpen)
                stream.Close();
            return tempFile;
        }

        public static string MD5(string value)
        {
            return Fn.Crypto.MD5(value);
        }
        public static string MD5(byte[] bytes)
        {
            return Fn.Crypto.MD5(bytes);
        }
        public static string SHA1(string value)
        {
            return Fn.Crypto.SHA1(value);
        }
        public static string SHA1(byte[] bytes)
        {
            return Fn.Crypto.SHA1(bytes);
        }

        public static string Zip(string sourceDirName)
        {
            return Compression.Zip(sourceDirName);
        }
        public static string ZipExtract(string sourceFileName)
        {
            string ext = Path.GetExtension(sourceFileName).ToLower();
            switch (ext)
            {
                case ".zip": return Compression.ZipExtract(sourceFileName);
                case ".tar": return Compression.TarExtract(sourceFileName);
                case ".gz": return Compression.TargzExtract(sourceFileName);
                default: return Compression.ZipExtract(sourceFileName);
            }
        }
        public static bool IsLocalIpOrHost(string host)
        {
            if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(host, "127.0.0.1"))
                return true;
            if (string.Equals(host, "::1"))
                return true;

            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ips = IpEntry.AddressList.Where(c => c.AddressFamily == AddressFamily.InterNetwork).Select(c => c.ToString()).ToArray();
            return ips.Any(c => c == host);
        }

        public static void CopyFolder(string sourceDirName, string destDirName, params string[] skipFiles)
        {
            CopyFolder(new DirectoryInfo(sourceDirName), destDirName, skipFiles);
        }
        static void CopyFolder(DirectoryInfo source, string destDirName, params string[] skipFiles)
        {
            if (!source.Exists || source.Attributes.HasFlag(FileAttributes.Hidden))
                return;

            Directory.CreateDirectory(destDirName);
            var fileInfos = source.GetFiles();
            foreach (var info in fileInfos)
            {
                if (info.Attributes.HasFlag(FileAttributes.Hidden))
                    continue;
                if (skipFiles.Any(c => string.Equals(c, info.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;
                if (info.Exists)
                    info.CopyTo(Path.Combine(destDirName, info.Name), true);
            }

            var dirInfos = source.GetDirectories();
            foreach (var info in dirInfos)
                CopyFolder(info, Path.Combine(destDirName, info.Name));
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