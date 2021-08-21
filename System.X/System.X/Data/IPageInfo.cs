namespace System.Data
{
    /// <summary>
    /// 提供列表数据分页信息的接口。
    /// </summary>
    public interface IPageInfo
    {
        /// <summary>
        /// 分页数据的页索引值。
        /// </summary>
        int PageIndex { get; set; }
        /// <summary>
        /// 分页数据的页面大小。
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// 数据记录的总个数。
        /// </summary>
        int TotalCount { get; set; }
    }
}