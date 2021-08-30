using System.Runtime.InteropServices;

namespace System.Data
{
    /// <summary>
    /// 提供列表数据分页信息的类。
    /// </summary>
    [global::System.Runtime.Serialization.DataContract, global::System.Serializable]
    public struct PageInfo
    {
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

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $"{{PageIndex:{ PageIndex},PageSize:{ PageSize},TotalCount:{ TotalCount}}}";
        }
    }
}