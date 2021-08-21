namespace System.Collections.Generic
{
    [global::System.Serializable]
    [global::System.Runtime.Serialization.DataContract]
    public sealed class PagedList<T> : List<T>, System.Data.IPageInfo
    {
        public PagedList() : base(0x64) { }

        public PagedList(IEnumerable<T> collection) : base(collection) { }

        /// <summary>
        /// 分页数据的页索引值。
        /// </summary>
        [global::System.Runtime.Serialization.DataMember]
        public int PageIndex { get; set; }
        /// <summary>
        /// 分页数据的页面大小。
        /// </summary>
        [global::System.Runtime.Serialization.DataMember]
        public int PageSize { get; set; }
        /// <summary>
        /// 数据记录的总个数。
        /// </summary>
        [global::System.Runtime.Serialization.DataMember]
        public int TotalCount { get; set; }
    }
}