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
        public static void ArgNotEmpty<T>(System.Collections.Generic.IEnumerable<T> source, string paramName, string message)
        {
            if (source == null)
                throw new ArgumentNullException(paramName);
            if (!System.Linq.Enumerable.Any(source))
                throw new ArgumentException(message, paramName);
        }
    }
}