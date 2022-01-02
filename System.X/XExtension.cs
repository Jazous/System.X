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
    public static TSource? Get<TKey, TSource>(this IDictionary<TKey, TSource> source, TKey key)
    {
        TSource? result;
        if (source.TryGetValue(key, out result))
            return result;
        return default;
    }
}