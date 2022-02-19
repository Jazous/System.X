namespace System;

public static class XExtension
{
    public static bool EqualsIgC(this string source, string value)
    {
        return string.Equals(source, value, StringComparison.OrdinalIgnoreCase);
    }
    public static string TrimStart(this string source, string value)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;

        if (source.StartsWith(value, StringComparison.OrdinalIgnoreCase))
            return TrimStart(source.Substring(0, value.Length), value);
        return source;
    }
    public static string TrimEnd(this string source, string value)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;

        if (source.EndsWith(value, StringComparison.OrdinalIgnoreCase))
            return TrimStart(source.Substring(source.Length - value.Length, value.Length), value);
        return source;
    }
    /// <summary>
    ///  Gets the value associated with the specified key.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="key">The key whose value to get.</param>
    /// <returns>The value associated with the specified key,if not exists return null.</returns>
    public static TSource? Get<TKey, TSource>(this IDictionary<TKey, TSource> source, TKey key)
    {
        TSource? result;
        if (source.TryGetValue(key, out result))
            return result;
        return default;
    }

    /// <summary>
    /// Copy all files in the directory to the destination directory.
    /// </summary>
    /// <param name="source">The directory to copy.</param>
    /// <param name="destDirName">The destination directory to copy to.</param>
    public static void CopyTo(this DirectoryInfo source, string destDirName)
    {
        Directory.CreateDirectory(destDirName);
        FileInfo[] fileArr = source.GetFiles();
        FileInfo fileInfo = null;
        for (int i = 0; i < fileArr.Length; i++)
        {
            fileInfo = fileArr[i];
            fileInfo.CopyTo(Path.Combine(destDirName, fileInfo.Name), true);
        }

        var dirArr = source.GetDirectories();
        foreach (var item in dirArr)
            CopyTo(item, Path.Combine(destDirName, item.Name));
    }

    /// <summary>
    /// Copy all files in the directory to the destination directory expect the spcial files.
    /// </summary>
    /// <param name="source">The directory to copy.</param>
    /// <param name="destDirName">The destination directory to copy to.</param>
    /// <param name="skipFiles">The list of file name to skip.</param>
    public static void CopyTo(this DirectoryInfo source, string destDirName, string[] skipFiles)
    {
        Directory.CreateDirectory(destDirName);
        FileInfo[] fileArr = source.GetFiles();
        FileInfo info;
        bool existsFlag;
        for (int i = 0; i < fileArr.Length; i++)
        {
            info = fileArr[i];
            existsFlag = false;

            for (int j = 0; j < skipFiles.Length; j++)
            {
                if (string.Equals(skipFiles[j], info.Name, StringComparison.OrdinalIgnoreCase))
                {
                    existsFlag = true;
                    break;
                }
            }

            if (!existsFlag)
                info.CopyTo(Path.Combine(destDirName, info.Name), true);
        }
        var dirArr = source.GetDirectories();
        foreach (var item in dirArr)
            CopyTo(item, Path.Combine(destDirName, item.Name));
    }
}