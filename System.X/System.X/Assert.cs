namespace System
{
    /// <summary>
    /// 断言。
    /// </summary>
    public sealed class Assert
    {
        public static void IsNotNull<T>(T value, string paramName) where T : class
        {
            if (value == null) throw new ArgumentNullException(paramName);
        }
    }
}