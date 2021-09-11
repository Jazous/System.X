namespace System
{
    public sealed class Assert
    {
        public static void ArgNotNull<T>(T obj, string paramName) where T : class
        {
            if (obj == null) throw new ArgumentNullException(paramName);
        }
        public static void ArgValid(bool condition, string paramName, string message)
        {
            if (condition) throw new ArgumentException(message, paramName);
        }
    }
}