namespace System.X.IO;

public sealed class CompressHelper
{
    internal static readonly CompressHelper Instance = new CompressHelper();

    private CompressHelper() { }

    public string GZip(string value)
    {
        var bytes = new byte[value.Length];
        int index = 0;
        var items = value.ToCharArray();
        foreach (char item in items)
            bytes[index++] = (byte)item;

        var buffer = GZip(bytes);
        var sb = new System.Text.StringBuilder(buffer.Length);
        foreach (byte item in buffer)
            sb.Append((char)item);

        return sb.ToString();
    }
    public string UnGZip(string value)
    {
        var bytes = new byte[value.Length];
        int index = 0;
        var items = value.ToCharArray();
        foreach (char item in items)
            bytes[index++] = (byte)item;

        var buffer = UnGZip(bytes);
        var sb = new System.Text.StringBuilder(buffer.Length);
        for (int i = 0; i < buffer.Length; i++)
            sb.Append((char)buffer[i]);

        return sb.ToString();
    }

    public byte[] GZip(byte[] bytes)
    {
        using (var inputStream = new System.IO.MemoryStream())
        {
            using (var gZipStream = new System.IO.Compression.GZipStream(inputStream, System.IO.Compression.CompressionMode.Compress))
                gZipStream.Write(bytes, 0, bytes.Length);

            return inputStream.ToArray();
        }
    }
    public async Task<byte[]> GZipAsync(byte[] bytes)
    {
        using (var inputStream = new System.IO.MemoryStream())
        {
            using (var gZipStream = new System.IO.Compression.GZipStream(inputStream, System.IO.Compression.CompressionMode.Compress))
            {
                await gZipStream.WriteAsync(bytes, 0, bytes.Length);
                return inputStream.ToArray();
            }
        }
    }
    public byte[] UnGZip(byte[] bytes)
    {
        using (var inputStream = new System.IO.MemoryStream(bytes))
        {
            using (var gZipStream = new System.IO.Compression.GZipStream(inputStream, System.IO.Compression.CompressionMode.Decompress))
            {
                using (var outputStream = new System.IO.MemoryStream())
                {
                    gZipStream.CopyTo(outputStream);
                    return outputStream.ToArray();
                }
            }
        }
    }
    public async Task<byte[]> UnGZipAsync(byte[] bytes)
    {
        using (var inputStream = new System.IO.MemoryStream(bytes))
        {
            using (var gZipStream = new System.IO.Compression.GZipStream(inputStream, System.IO.Compression.CompressionMode.Decompress))
            {
                using (var outputStream = new System.IO.MemoryStream())
                {
                    await gZipStream.CopyToAsync(outputStream);
                    return outputStream.ToArray();
                }
            }
        }
    }

    /// <summary>
    /// Creates a zip archive that contains the files and directories from the specified directory.
    /// </summary>
    /// <param name="srcDirName">The path to the directory to be archived, specified as a relative or absolute path.</param>
    /// <returns>The absolute path of the archive to be created.</returns>
    public string Zip(string srcDirName)
    {
        var dirInfo = new DirectoryInfo(srcDirName);
        string destFileName = Path.Combine(dirInfo.Parent.FullName, dirInfo.Name + ".zip");
        System.IO.Compression.ZipFile.CreateFromDirectory(srcDirName, destFileName);
        return destFileName;
    }
    /// <summary>
    /// Creates a zip archive that contains the files and directories from the specified directory.
    /// </summary>
    /// <param name="srcDirName">The path to the directory to be archived, specified as a relative or absolute path.</param>
    /// <param name="destFileName">The path of the archive to be created, specified as a relative or absolute path.</param>
    public void Zip(string srcDirName, string destFileName, System.IO.Compression.CompressionLevel compressionLevel = System.IO.Compression.CompressionLevel.Optimal, bool includeBaseDirectory = true)
    {
        System.IO.Compression.ZipFile.CreateFromDirectory(srcDirName, destFileName, compressionLevel, includeBaseDirectory);
    }
    /// <summary>
    /// Extracts all of the files in the specified zip archive to current directory named zip archive fileName without extension on the file system.
    /// </summary>
    /// <param name="srcFileName">The path on the file system to the archive that is to be extracted.</param>
    /// <returns>The path to the destination directory on the file system.</returns>
    public string ZipExtract(string srcFileName)
    {
        string tempDir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(srcFileName), System.IO.Path.GetFileNameWithoutExtension(srcFileName));
        ZipExtract(srcFileName, tempDir, true);
        return tempDir;
    }
    /// <summary>
    /// Extracts all of the files in the specified zip archive to a directory on the file system
    /// </summary>
    /// <param name="srcFileName">The path on the file system to the archive that is to be extracted.</param>
    /// <param name="destDirName">The path to the destination directory on the file system.</param>
    /// <param name="overwrite">true to overwrite files; false otherwise.</param>
    public void ZipExtract(string srcFileName, string destDirName, bool overwrite = true)
    {
        System.IO.Directory.CreateDirectory(destDirName);
        System.IO.Compression.ZipFile.ExtractToDirectory(srcFileName, destDirName, overwrite);
    }
    ///// <summary>
    ///// Create a tar archive that contains the files and directories from the specified directory.
    ///// </summary>
    ///// <param name="srcDirName">The path to the directory to be archived, specified as a relative or absolute path.</param>
    ///// <returns>The path of the archive to be created.</returns>
    //public string Tar(string srcDirName)
    //{
    //    var tempFile = Path.GetTempFileName();
    //    Tar(srcDirName, tempFile);
    //    return tempFile;
    //}
    ///// <summary>
    ///// Create a tar archive that contains the files and directories from the specified directory.
    ///// </summary>
    ///// <param name="srcDirName">The path to the directory to be archived, specified as a relative or absolute path.</param>
    ///// <param name="destFileName">The path of the archive to be created, specified as a relative or absolute path.</param>
    //public void Tar(string srcDirName, string destFileName)
    //{
    //    var files = Directory.GetFiles(srcDirName);
    //    using (var fs = File.OpenWrite(destFileName))
    //    {
    //        using (var archive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateOutputTarArchive(fs, ICSharpCode.SharpZipLib.Tar.TarBuffer.DefaultBlockFactor, System.Text.Encoding.Unicode))
    //        {
    //            foreach (var name in files)
    //            {
    //                var entry = ICSharpCode.SharpZipLib.Tar.TarEntry.CreateEntryFromFile(name);
    //                entry.Name = Path.GetFileName(name);
    //                archive.WriteEntry(entry, true);
    //            }
    //        }
    //    }
    //}
    ///// <summary>
    ///// Extracts all of the files in the specified tar archive to temporary directory on the file system
    ///// </summary>
    ///// <param name="srcFileName">The path on the file system to the archive that is to be extracted.</param>
    ///// <returns>The path to the destination directory on the file system.</returns>
    //public string TarExtract(string srcFileName)
    //{
    //    string tempDir = Fn.NewDir();
    //    TarExtract(srcFileName, tempDir);
    //    return tempDir;
    //}
    ///// <summary>
    ///// Extracts all of the files in the specified tar archive to special directory on the file system
    ///// </summary>
    ///// <param name="srcFileName">The path on the file system to the archive that is to be extracted.</param>
    ///// <param name="destDirName">The path to the destination directory on the file system.</param>
    //public void TarExtract(string srcFileName, string destDirName)
    //{
    //    System.IO.Directory.CreateDirectory(destDirName);
    //    using (var fs = File.OpenRead(srcFileName))
    //    using (var archive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(fs, ICSharpCode.SharpZipLib.Tar.TarBuffer.DefaultBlockFactor, Encoding.Unicode))
    //        archive.ExtractContents(destDirName);
    //}

    ///// <summary>
    ///// Create a tar.gz archive that contains the files and directories from the specified directory.
    ///// </summary>
    ///// <param name="srcDirName">The path to the directory to be archived, specified as a relative or absolute path.</param>
    ///// <returns>The path of the archive to be created.</returns>
    //public string Targz(string srcDirName)
    //{
    //    string tempFile = Path.GetTempFileName();
    //    Targz(srcDirName, tempFile);
    //    return tempFile;
    //}
    ///// <summary>
    ///// Create a tar.gz archive that contains the files and directories from the specified directory.
    ///// </summary>
    ///// <param name="srcDirName">The path to the directory to be archived, specified as a relative or absolute path.</param>
    ///// <param name="destFileName">The path of the archive to be created, specified as a relative or absolute path.</param>
    //public void Targz(string srcDirName, string destFileName)
    //{
    //    Directory.CreateDirectory(Path.GetDirectoryName(destFileName));
    //    using (var fs = File.OpenWrite(destFileName))
    //    using (var gs = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(fs))
    //    using (var archive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateOutputTarArchive(gs, ICSharpCode.SharpZipLib.Tar.TarBuffer.DefaultBlockFactor, System.Text.Encoding.Unicode))
    //    {
    //        archive.WriteEntry(ICSharpCode.SharpZipLib.Tar.TarEntry.CreateEntryFromFile(srcDirName), true);
    //    }
    //}
    ///// <summary>
    ///// Extracts all of the files in the specified tar.gz archive to temporary directory on the file system
    ///// </summary>
    ///// <param name="srcFileName">The path on the file system to the archive that is to be extracted.</param>
    ///// <returns>The path to the destination directory on the file system.</returns>
    //public string TargzExtract(string srcFileName)
    //{
    //    string tempDir = Fn.NewDir();
    //    TargzExtract(srcFileName, tempDir);
    //    return tempDir;
    //}
    ///// <summary>
    ///// Extracts all of the files in the specified tar.gz archive to special directory on the file system
    ///// </summary>
    ///// <param name="srcFileName">The path on the file system to the archive that is to be extracted.</param>
    ///// <param name="destDirName">The path to the destination directory on the file system.</param>
    //public void TargzExtract(string srcFileName, string destDirName)
    //{
    //    Directory.CreateDirectory(destDirName);

    //    using (var fs = File.OpenRead(srcFileName))
    //    using (var gs = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(fs))
    //    using (var archive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(gs, ICSharpCode.SharpZipLib.Tar.TarBuffer.DefaultBlockFactor, System.Text.Encoding.Unicode))
    //        archive.ExtractContents(destDirName);
    //}

}