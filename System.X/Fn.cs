﻿using System.Collections.Generic;
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
        public static X.Enums.OSPlatforms OSPlatform { get; }

        public static System.X.Data.DataHelper Data { get => System.X.Data.DataHelper.Instance; }
        public static System.X.IO.FileHelper File { get => System.X.IO.FileHelper.Instance; }
        //public static System.X.IO.ProfileHelper Profile { get => System.X.IO.ProfileHelper.Instance; }
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
        private Fn() { }

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
            if (string.IsNullOrWhiteSpace(value))
                return null;
            DateTime result;
            if (DateTime.TryParse(value, out result))
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
        public static string GetDesc<T>(T value) where T : Enum
        {
            string text = value.ToString();
            var desc = (System.ComponentModel.DescriptionAttribute)value.GetType().GetField(text).GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).FirstOrDefault();
            return desc == null ? text : desc.Description;
        }
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
        public static string NewGuid(string format)
        {
            return Guid.NewGuid().ToString(format);
        }
        public static string NewTempDir()
        {
            string tempDir = Path.Combine(TempDir, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }
        public static async Task<string> NewTempFile(byte[] bytes)
        {
            return await Fn.File.Create(bytes);
        }
        public static async Task<string> NewTempFile(Stream stream)
        {
            return await Fn.File.Create(stream, false);
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
        public static bool IsLocalIpOrHost(string hostNameOrIp)
        {
            if (string.Equals(hostNameOrIp, "localhost", StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(hostNameOrIp, "127.0.0.1"))
                return true;
            if (string.Equals(hostNameOrIp, "::1"))
                return true;

            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ips = IpEntry.AddressList.Where(c => c.AddressFamily == AddressFamily.InterNetwork).Select(c => c.ToString()).ToArray();
            return ips.Any(c => c == hostNameOrIp);
        }

        /// <summary>
        /// Execute command with cmd.exe or /bin/bash then exit.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string ExeCmd(string command)
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
        public static string ExeShell(string name, string args)
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

        public static bool Eval(string expression)
        {
            throw new NotImplementedException();
        }
        public static void DoIF<T>(bool flag, Action action)
        {
            if (flag == true) action.Invoke();
        }
        public static T DoIF<T>(bool flag, Func<T> source) where T : class
        {
            if (flag == true) return source.Invoke();
            return null;
        }
        public static T Invoke<T>(Type type, object[] ctorArgs, string methodName, params object[] methodArgs)
        {
            var instance = System.Activator.CreateInstance(type, ctorArgs);
            return (T)typeof(T).GetMethod(methodName).Invoke(instance, methodArgs);
        }
    }
}