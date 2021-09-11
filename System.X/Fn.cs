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
        public static X.Enums.OSPlatform Platform { get; }

        //public static System.X.Data.DataHelper Data { get => System.X.Data.DataHelper.Instance; }
        //public static System.X.IO.FileHelper File { get => System.X.IO.FileHelper.Instance; }
        //public static System.X.IO.ProfileHelper Profile { get => System.X.IO.ProfileHelper.Instance; }
        //public static System.X.Text.HtmlHelper Html { get => System.X.Text.HtmlHelper.Instance; }
        //public static System.X.Text.ValidHelper Valid { get => System.X.Text.ValidHelper.Instance; }
        public static System.X.Drawing.ImageHelper Image { get => System.X.Drawing.ImageHelper.Instance; }
        public static System.X.Cryptography.CryptoHelper Crypto { get => System.X.Cryptography.CryptoHelper.Instance; }
        public static System.X.Net.HttpHelper HTTP { get => X.Net.HttpHelper.Instance; }
        public static System.X.IO.CompressHelper Compress { get => X.IO.CompressHelper.Instance; }

        static Fn()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                Platform = X.Enums.OSPlatform.Windows;
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                Platform = X.Enums.OSPlatform.Linux;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                Platform = X.Enums.OSPlatform.MacOSX;
        }
        private Fn()
        {

        }

        public static bool ToBoolean(string value)
        {
            switch (value)
            {
                case null:
                case "":
                case "0":
                case "false":
                case "False":
                case "FALSE": return false;
                case "1":
                case "true":
                case "True":
                case "TRUE": return true;
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
            switch (value)
            {
                case null:
                case "": return null;
                case "现在":
                case "Now":
                    return DateTime.Now;
            }
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
                case "yday":
                case "Yesterday":
                    return DateTime.Today.AddDays(-1);
                case "今天":
                case "今日":
                case "Today":
                case "现在":
                case "Now":
                    return DateTime.Today;
                case "明天":
                case "明日":
                case "tmw":
                case "Tomorrow":
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
        public static System.Data.DataTable ToDataTable<T>(IEnumerable<T> source, params NameValue[] mapping)
        {
            return System.X.Data.DataHelper.Instance.MapTo<T>(source, mapping);
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
        public static string GetDescription<TSource>(string memberName)
        {
            var member = typeof(TSource).GetMember(memberName).FirstOrDefault();
            if (member == null) return null;

            var desc = (System.ComponentModel.DescriptionAttribute)member.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).FirstOrDefault();
            return desc == null ? null : desc.Description;
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

        public static string NewDir()
        {
            string tempDir = Path.Combine(TempDir, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }
        public static string NewFilePath(string extension = ".tmp")
        {
            return TempDir + Guid.NewGuid().ToString("N") + extension;
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
                    case X.Enums.OSPlatform.Windows:
                        process.StartInfo.FileName = System.IO.Path.Combine(System.Environment.GetEnvironmentVariable("windir"), @"system32\cmd.exe");
                        break;
                    case X.Enums.OSPlatform.Linux:
                        process.StartInfo.FileName = "/bin/bash";
                        break;
                    case X.Enums.OSPlatform.MacOSX:
                        process.StartInfo.FileName = "/bin/bash";
                        break;
                    default:
                        return null;
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

        public static bool Ping(string host)
        {
            var options = new Net.NetworkInformation.PingOptions() { DontFragment = true };
            var buffer = Text.Encoding.ASCII.GetBytes(".");
            using (var p = new System.Net.NetworkInformation.Ping())
                return p.Send(host, 2000, buffer, options).Status == Net.NetworkInformation.IPStatus.Success;
        }

        public static bool IsLocalhost(string host)
        {
            if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(host, "127.0.0.1"))
                return true;
            if (string.Equals(host, "::1"))
                return true;

            var IpEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            var ips = IpEntry.AddressList.Where(c => c.AddressFamily == AddressFamily.InterNetwork || c.AddressFamily == AddressFamily.InterNetworkV6).Select(c => c.ToString()).ToArray();
            return ips.Any(c => c == host);
        }
    }
}