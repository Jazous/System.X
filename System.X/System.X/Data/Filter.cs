namespace System.Data
{
    /// <summary>
    /// 提供自定义查询参数过滤器。
    /// </summary>
    public struct Filter
    {
        /// <summary>
        /// 过滤条件。
        /// </summary>
        [global::System.Runtime.Serialization.DataMember]
        public System.Linq.Expressions.Expression<System.Func<dynamic, bool>> Expression { get; set; }
        /// <summary>
        /// 分页数据的页索引值。
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 分页数据的页面大小。
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SortProperty { get; set; }
        public bool Ascending { get; set; }
    }
}