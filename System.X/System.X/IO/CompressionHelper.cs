﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace System.X.IO
{
    public sealed class CompressionHelper
    {
        internal static readonly CompressionHelper Instance = new CompressionHelper();

        public async Threading.Tasks.Task<string> BZip2Compress(string value)
        {
            return Convert.ToBase64String(await BZip2Compress(System.Text.Encoding.UTF8.GetBytes(value)));
        }
        public async Threading.Tasks.Task<string> BZip2Decompress(string value)
        {
            using (var inputStream = new MemoryStream(Convert.FromBase64String(value)))
            using (var zipStream = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(inputStream))
            using (var reader = new StreamReader(zipStream, System.Text.Encoding.UTF8))
                return await reader.ReadToEndAsync();
        }
        public async Threading.Tasks.Task<byte[]> BZip2Compress(byte[] bytes)
        {
            using (var outputStream = new System.IO.MemoryStream())
            {
                using (var zipStream = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(outputStream))
                {
                    await zipStream.WriteAsync(bytes, 0, bytes.Length);
                    zipStream.Close();
                }
                return outputStream.ToArray();
            }
        }
        public async Threading.Tasks.Task<byte[]> BZip2Decompress(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            using (var zipStream = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(ms))
            {
                var buffer = new byte[zipStream.Length];
                await zipStream.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
        }
        public async Threading.Tasks.Task<string> GZipCompress(string value)
        {
            var bytes = new byte[value.Length];
            int index = 0;
            foreach (char item in value.ToCharArray())
                bytes[index++] = (byte)item;

            var buffer = await GZipCompress(bytes);
            var sb = new System.Text.StringBuilder(buffer.Length);
            foreach (byte item in buffer)
                sb.Append((char)item);

            return sb.ToString();
        }
        public async Threading.Tasks.Task<string> GZipDecompress(string value)
        {
            var bytes = new byte[value.Length];
            int index = 0;
            foreach (char item in value.ToCharArray())
                bytes[index++] = (byte)item;

            var buffer = await GZipDecompress(bytes);
            var sb = new System.Text.StringBuilder(buffer.Length);
            for (int i = 0; i < buffer.Length; i++)
                sb.Append((char)buffer[i]);

            return sb.ToString();
        }
        public async Threading.Tasks.Task<byte[]> GZipCompress(byte[] bytes)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                using (var sw = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress))
                    await sw.WriteAsync(bytes, 0, bytes.Length);

                return ms.ToArray();
            }
        }
        public async Threading.Tasks.Task<byte[]> GZipDecompress(byte[] bytes)
        {
            using (var ms = new System.IO.MemoryStream(bytes))
            {
                using (var sr = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[sr.Length];
                    await sr.ReadAsync(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
        }

        /// <summary>
        /// Creates a zip archive that contains the files and directories from the specified directory.
        /// </summary>
        /// <param name="sourceDirName">The path to the directory to be archived, specified as a relative or absolute path.</param>
        /// <returns>The absolute path of the archive to be created.</returns>
        public string ZipFileCompress(string sourceDirName)
        {
            string tempFile = Path.GetTempFileName();
            ZipFileCompress(sourceDirName, tempFile);
            return tempFile;
        }
        /// <summary>
        /// Creates a zip archive that contains the files and directories from the specified directory.
        /// </summary>
        /// <param name="sourceDirName">The path to the directory to be archived, specified as a relative or absolute path.</param>
        /// <param name="destFileName">The path of the archive to be created, specified as a relative or absolute path.</param>
        public void ZipFileCompress(string sourceDirName, string destFileName)
        {
            System.IO.Compression.ZipFile.CreateFromDirectory(sourceDirName, destFileName);
        }
        /// <summary>
        /// Extracts all of the files in the specified zip archive to temporary directory on the file system
        /// </summary>
        /// <param name="sourceFileName">The path on the file system to the archive that is to be extracted.</param>
        /// <param name="overwrite">true to overwrite files; false otherwise.</param>
        /// <returns>The path to the destination directory on the file system.</returns>
        public string ZipFileExtract(string sourceFileName, bool overwrite = true)
        {
            string tempDir = Fn.NewTempDir();
            ZipFileExtract(sourceFileName, tempDir, overwrite);
            return tempDir;
        }
        /// <summary>
        /// Extracts all of the files in the specified zip archive to a directory on the file system
        /// </summary>
        /// <param name="sourceFileName">The path on the file system to the archive that is to be extracted.</param>
        /// <param name="destDirName">The path to the destination directory on the file system.</param>
        /// <param name="overwrite">true to overwrite files; false otherwise.</param>
        public void ZipFileExtract(string sourceFileName, string destDirName, bool overwrite = true)
        {
            System.IO.Directory.CreateDirectory(destDirName);
            System.IO.Compression.ZipFile.ExtractToDirectory(sourceFileName, destDirName, overwrite);
        }
        /// <summary>
        /// Create a tar archive that contains the files and directories from the specified directory.
        /// </summary>
        /// <param name="sourceDirName">The path to the directory to be archived, specified as a relative or absolute path.</param>
        /// <returns>The path of the archive to be created.</returns>
        public string TarFile(string sourceDirName)
        {
            var tempFile = Path.GetTempFileName();
            TarFile(sourceDirName, tempFile);
            return tempFile;
        }
        /// <summary>
        /// Create a tar archive that contains the files and directories from the specified directory.
        /// </summary>
        /// <param name="sourceDirName">The path to the directory to be archived, specified as a relative or absolute path.</param>
        /// <param name="destFileName">The path of the archive to be created, specified as a relative or absolute path.</param>
        public void TarFile(string sourceDirName, string destFileName)
        {
            var files = Directory.GetFiles(sourceDirName);
            using (var fs = File.OpenWrite(destFileName))
            {
                using (var archive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateOutputTarArchive(fs, ICSharpCode.SharpZipLib.Tar.TarBuffer.DefaultBlockFactor, System.Text.Encoding.Unicode))
                {
                    foreach (var name in files)
                    {
                        var entry = ICSharpCode.SharpZipLib.Tar.TarEntry.CreateEntryFromFile(name);
                        entry.Name = Path.GetFileName(name);
                        archive.WriteEntry(entry, true);
                    }
                }
            }
        }
        /// <summary>
        /// Extracts all of the files in the specified tar archive to temporary directory on the file system
        /// </summary>
        /// <param name="sourceFileName">The path on the file system to the archive that is to be extracted.</param>
        /// <returns>The path to the destination directory on the file system.</returns>
        public string TarFileExtract(string sourceFileName)
        {
            string tempDir = Fn.NewTempDir();
            TarFileExtract(sourceFileName, tempDir);
            return tempDir;
        }
        /// <summary>
        /// Extracts all of the files in the specified tar archive to special directory on the file system
        /// </summary>
        /// <param name="sourceFileName">The path on the file system to the archive that is to be extracted.</param>
        /// <param name="destDirName">The path to the destination directory on the file system.</param>
        public void TarFileExtract(string sourceFileName, string destDirName)
        {
            System.IO.Directory.CreateDirectory(destDirName);
            using (var fs = File.OpenRead(sourceFileName))
            using (var archive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(fs, ICSharpCode.SharpZipLib.Tar.TarBuffer.DefaultBlockFactor, Encoding.Unicode))
                archive.ExtractContents(destDirName);
        }

        /// <summary>
        /// Create a tar.gz archive that contains the files and directories from the specified directory.
        /// </summary>
        /// <param name="sourceDirName">The path to the directory to be archived, specified as a relative or absolute path.</param>
        /// <returns>The path of the archive to be created.</returns>
        public string TarGzFileCompress(string sourceDirName)
        {
            string tempFile = Path.GetTempFileName();
            TarGzFileCompress(sourceDirName, tempFile);
            return tempFile;
        }
        /// <summary>
        /// Create a tar.gz archive that contains the files and directories from the specified directory.
        /// </summary>
        /// <param name="sourceDirName">The path to the directory to be archived, specified as a relative or absolute path.</param>
        /// <param name="destFileName">The path of the archive to be created, specified as a relative or absolute path.</param>
        public void TarGzFileCompress(string sourceDirName, string destFileName)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destFileName));
            using (var fs = File.OpenWrite(destFileName))
            using (var gs = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(fs))
            using (var archive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateOutputTarArchive(gs, ICSharpCode.SharpZipLib.Tar.TarBuffer.DefaultBlockFactor, System.Text.Encoding.Unicode))
                archive.WriteEntry(ICSharpCode.SharpZipLib.Tar.TarEntry.CreateEntryFromFile(Path.ChangeExtension(destFileName, string.Empty)), true);
        }
        /// <summary>
        /// Extracts all of the files in the specified tar.gz archive to temporary directory on the file system
        /// </summary>
        /// <param name="sourceFileName">The path on the file system to the archive that is to be extracted.</param>
        /// <returns>The path to the destination directory on the file system.</returns>
        public string TarGzFileExtract(string sourceFileName)
        {
            string tempDir = Fn.NewTempDir();
            TarGzFileExtract(sourceFileName, tempDir);
            return tempDir;
        }
        /// <summary>
        /// Extracts all of the files in the specified tar.gz archive to special directory on the file system
        /// </summary>
        /// <param name="sourceFileName">The path on the file system to the archive that is to be extracted.</param>
        /// <param name="destDirName">The path to the destination directory on the file system.</param>
        public void TarGzFileExtract(string sourceFileName, string destDirName)
        {
            Directory.CreateDirectory(destDirName);
            using (var fs = File.OpenRead(sourceFileName))
            using (var gs = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(fs))
            using (var archive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(gs, ICSharpCode.SharpZipLib.Tar.TarBuffer.DefaultBlockFactor, System.Text.Encoding.Unicode))
                archive.ExtractContents(destDirName);
        }

    }
}