using System.Runtime.InteropServices;

namespace System.Data
{
    [global::System.Runtime.Serialization.DataContract, global::System.Serializable]
    public struct PageInfo
    {
        [global::System.Runtime.Serialization.DataMember]
        public int PageIndex { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public int PageSize { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public int TotalCount { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is PageInfo)
                return this.Equals((PageInfo)obj);
            return false;
        }
        public bool Equals(PageInfo pageInfo)
        {
            return this.PageIndex == pageInfo.PageIndex && this.PageSize == pageInfo.PageSize && this.TotalCount == pageInfo.TotalCount;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $"{{\"pageIndex\":{ PageIndex},\"pageSize\":{ PageSize},\"totalCount\":{ TotalCount}}}";
        }
    }
}