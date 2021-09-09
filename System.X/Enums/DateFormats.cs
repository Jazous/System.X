namespace System
{
    public enum DateFormats
    {
        /// <summary>
        /// yyyy-MM-dd.
        /// </summary>
        Date = 0,
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss.
        /// </summary>
        DateTime = 1,
        /// <summary>
        /// MM-dd.
        /// </summary>
        SDate = 3,
        /// <summary>
        /// MM-dd HH:mm.
        /// </summary>
        SDateSTime = 6,
        /// <summary>
        /// yyyy-MM-dd HH:mm.
        /// </summary>
        SDateTime = 4,
        /// <summary>
        /// HH:mm.
        /// </summary>
        STime = 5,
        /// <summary>
        /// HH:mm:ss.
        /// </summary>
        Time = 2,
        /// <summary>
        /// yyyyMMddHHmmssttt.
        /// </summary>
        TimeSpan = 7,
    }
}