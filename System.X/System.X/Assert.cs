namespace System
{
    /// <summary>
    /// 断言。
    /// </summary>
    public sealed class Assert
    {
        public static void IsNotNull<T>(T obj) where T : class
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
        }
    }
}