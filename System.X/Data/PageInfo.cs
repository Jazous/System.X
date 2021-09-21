using System.Runtime.InteropServices;

namespace System.Data
{
    [global::System.Runtime.Serialization.DataContract, global::System.Serializable]
    public struct PageInfo : IEquatable<PageInfo>
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
        public bool Equals(PageInfo other)
        {
            return this.PageIndex == other.PageIndex && this.PageSize == other.PageSize && this.TotalCount == other.TotalCount;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $"{{\"PageIndex\":{ PageIndex},\"PageSize\":{ PageSize},\"TotalCount\":{ TotalCount}}}";
        }
    }
}