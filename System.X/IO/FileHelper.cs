using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.X.IO
{
    public sealed class FileHelper
    {
        internal static readonly FileHelper Instance = new FileHelper();
        private FileHelper()
        {
        }

        /// <summary>
        /// Copy all files in source directory to the destination directory surpport skip spcial files.
        /// </summary>
        /// <param name="srcDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="skipFiles">The list of file name to skip.</param>
        public void CopyFolder(string srcDirName, string destDirName, params string[] skipFiles)
        {
            CopyFolder(new DirectoryInfo(srcDirName), destDirName, skipFiles);
        }
        void CopyFolder(DirectoryInfo source, string destDirName, params string[] skipFiles)
        {
            if (!source.Exists || source.Attributes.HasFlag(FileAttributes.Hidden))
                return;

            Directory.CreateDirectory(destDirName);
            var fileInfos = source.GetFiles();
            foreach (var info in fileInfos)
            {
                if (info.Attributes.HasFlag(FileAttributes.Hidden))
                    continue;
                if (skipFiles != null && skipFiles.Any(c => string.Equals(c, info.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;
                if (info.Exists)
                    info.CopyTo(Path.Combine(destDirName, info.Name), true);
            }

            var dirInfos = source.GetDirectories();
            foreach (var info in dirInfos)
                CopyFolder(info, Path.Combine(destDirName, info.Name));
        }

        public async Task<string> Create(byte[] bytes)
        {
            string tempFile = Path.GetTempFileName();
            using (var fs = System.IO.File.OpenWrite(tempFile))
            {
                await fs.WriteAsync(bytes, 0, bytes.Length);
                fs.Flush();
            }
            return tempFile;
        }
        public async Task<string> Create(Stream stream, bool leaveOpen = false)
        {
            string tempFile = Path.GetTempFileName();
            using (var fs = System.IO.File.OpenWrite(tempFile))
            {
                await stream.CopyToAsync(fs);
                fs.Flush();
            }
            if (!leaveOpen)
                stream.Close();
            return tempFile;
        }
    }
}