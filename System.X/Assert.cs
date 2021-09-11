namespace System
{
    /// <summary>
    /// 断言。
    /// </summary>
    public sealed class Assert
    {
        public static void ArgNotNull<T>(T obj, string name) where T : class
        {
            if (obj == null) throw new ArgumentNullException(name);
        }
        public static void ArgValid(bool flag, string name, string message)
        {
            if (flag) throw new ArgumentException(name);
        }
    }
}