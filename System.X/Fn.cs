using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace System
{
    public sealed class Fn
    {
        public static Text.Encoding UTF8 { get => System.Text.Encoding.UTF8; }
        public const string Alphabet = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
        static readonly string _tempDir = Path.GetTempPath();
        /// <summary>
        /// The path of the current user's temporary folder ending with a backslash.
        /// </summary>
        public static string TempDir { get => _tempDir; }

        /// <summary>
        /// Operating system that current application is running at.
        /// </summary>
        public static X.Enums.OSPlatforms Platform { get; }

        //public static System.X.Data.DataHelper Data { get => System.X.Data.DataHelper.Instance; }
        //public static System.X.IO.FileHelper File { get => System.X.IO.FileHelper.Instance; }
        //public static System.X.IO.ProfileHelper Profile { get => System.X.IO.ProfileHelper.Instance; }
        //public static System.X.Text.HtmlHelper Html { get => System.X.Text.HtmlHelper.Instance; }
        //public static System.X.Text.TextHelper Text { get => System.X.Text.TextHelper.Instance; }
        public static System.X.Drawing.ImageHelper Image { get => System.X.Drawing.ImageHelper.Instance; }
        public static System.X.Cryptography.CryptoHelper Crypto { get => System.X.Cryptography.CryptoHelper.Instance; }
        public static System.X.Net.HttpHelper HTTP { get => X.Net.HttpHelper.Instance; }
        public static System.X.IO.CompressionHelper Compress { get => X.IO.CompressionHelper.Instance; }

        static Fn()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                Platform = X.Enums.OSPlatforms.Windows;
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                Platform = X.Enums.OSPlatforms.Linux;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                Platform = X.Enums.OSPlatforms.MacOSX;
            else
                Platform = X.Enums.OSPlatforms.Unknown;

        }
        private Fn() {
        
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
            if (decimal.TryParse(value, out result))
                return result;
            return null;
        }
        public static DateTime? ToDateTime(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            DateTime result;
            if (DateTime.TryParse(value, out result))
                return result;
            return null;
        }
        public static DateTime? ToDate(string value)
        {
            switch (value)
            {
                case null:
                case "": return null;
                case "昨天":
                case "昨日":
                    return DateTime.Today.AddDays(-1);
                case "今天":
                case "今日":
                    return DateTime.Today;
                case "明天":
                case "明日":
                    return DateTime.Today.AddDays(1);
            }
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

            var values = Enum.GetValues(typeof(T));
            foreach (var item in values)
            {
                var desc = (System.ComponentModel.DescriptionAttribute)item.GetType().GetField(item.ToString()).GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).FirstOrDefault();
                if (desc != null && string.Equals(desc.Description, value, StringComparison.OrdinalIgnoreCase))
                    return (T)item;
            }
            return null;
        }
        public static List<KeyValuePair<int, string>> ToList<T>(bool descFirst) where T : Enum
        {
            var result = new List<KeyValuePair<int, string>>();
            var values = Enum.GetValues(typeof(T));
            if (descFirst)
            {
                foreach (var item in values)
                {
                    var name = item.ToString();
                    var desc = (System.ComponentModel.DescriptionAttribute)item.GetType().GetField(name).GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).FirstOrDefault();
                    result.Add(new KeyValuePair<int, string>((int)item, desc == null ? name : desc.Description));
                }
                return result;
            }
            foreach (var item in values)
                result.Add(new KeyValuePair<int, string>((int)item, item.ToString()));

            return result;
        }
        public static List<T> ToList<T>(System.Data.DataTable dataTable, params NameValue[] mapping) where T : new()
        {
            return System.X.Data.DataHelper.Instance.MapTo<T>(dataTable, mapping);
        }

        public static string GetDescription(Enum value)
        {
            string text = value.ToString();
            var desc = (System.ComponentModel.DescriptionAttribute)value.GetType().GetField(text).GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).FirstOrDefault();
            return desc == null ? text : desc.Description;
        }
        public static string GetDescription<TSource>(Linq.Expressions.Expression<Func<TSource, dynamic>> memberSelector)
        {
            return GetAttributes<TSource, System.ComponentModel.DescriptionAttribute>(memberSelector, false).FirstOrDefault()?.Description;
        }
        public static TAttribute[] GetAttributes<TSource, TAttribute>(Linq.Expressions.Expression<Func<TSource, dynamic>> memberSelector, bool inherit) where TAttribute : Attribute
        {
            var member = (Linq.Expressions.MemberExpression)memberSelector.Body;
            return (TAttribute[])member.Member.GetCustomAttributes(typeof(TAttribute), inherit);
        }
        public static object Invoke(Type type, object[] ctorArgs, string methodName, params object[] methodArgs)
        {
            var instance = System.Activator.CreateInstance(type, ctorArgs);
            return type.GetMethod(methodName).Invoke(instance, methodArgs);
        }


        public static string NewGuid()
        {
            return System.Guid.NewGuid().ToString();
        }
        public static string NewGuid(string format)
        {
            return Guid.NewGuid().ToString(format);
        }
        public static string NewDir()
        {
            string tempDir = Path.Combine(TempDir, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }
        public static string NewFile()
        {
            return Path.GetTempFileName();
        }
        public static string NewFile(string extension)
        {
            string path = TempDir + Guid.NewGuid().ToString("N") + extension;
            System.IO.File.Create(path);
            return path;
        }
        public static Task<string> NewFile(byte[] bytes)
        {
            return System.X.IO.FileHelper.Instance.Create(bytes);
        }
        public static Task<string> NewFile(Stream stream)
        {
            using (stream)
                return System.X.IO.FileHelper.Instance.Create(stream, false);
        }
        public void CopyFolder(string srcDirName, string destDirName, params string[] skipFiles)
        {
            System.X.IO.FileHelper.Instance.CopyFolder(srcDirName, destDirName, skipFiles);
        }

        public static string Zip(string srcDirName)
        {
            return Compress.Zip(srcDirName);
        }
        /// <summary>
        /// Extracts all of the files in the specified zip archive to temporary directory on the file system.
        /// </summary>
        /// <param name="srcFileName">The path on the file system to the archive that is to be extracted.</param>
        /// <returns>The path to the destination directory on the file system.</returns>
        public static string ZipExtract(string srcFileName)
        {
            return Compress.ZipExtract(srcFileName, true);
        }
        public static bool IsLocalIpOrHost(string hostNameOrIpAddress)
        {
            if (string.Equals(hostNameOrIpAddress, "localhost", StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(hostNameOrIpAddress, "127.0.0.1"))
                return true;
            if (string.Equals(hostNameOrIpAddress, "::1"))
                return true;

            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ips = IpEntry.AddressList.Where(c => c.AddressFamily == AddressFamily.InterNetwork || c.AddressFamily == AddressFamily.InterNetworkV6).Select(c => c.ToString()).ToArray();
            return ips.Any(c => c == hostNameOrIpAddress);
        }

        /// <summary>
        /// Execute command with cmd.exe or /bin/bash then exit.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string RunCmd(string command)
        {
            using (var process = new Diagnostics.Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                switch (Platform)
                {
                    case X.Enums.OSPlatforms.Windows:
                        process.StartInfo.FileName = System.IO.Path.Combine(System.Environment.GetEnvironmentVariable("windir"), @"system32\cmd.exe");
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
        public static string Run(string fileName, string args)
        {
            using (var process = new Diagnostics.Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = args;
                process.Start();
                process.WaitForExit();
                return process.StandardOutput.ReadToEnd();
            }
        }

        static bool Eval(string expression)
        {
            throw new NotImplementedException();
        }
    }
}