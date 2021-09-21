using System.Collections.Generic;
using System.Linq;

namespace System.Data
{
    [global::System.Runtime.Serialization.DataContract, global::System.Serializable]
    public struct QueryFilter : IEquatable<QueryFilter>
    {
        [global::System.Runtime.Serialization.DataMember]
        public List<QueryItem> Items { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public int PageIndex { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public int PageSize { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is QueryFilter)
                return this.Equals((QueryFilter)obj);
            return false;
        }
        public bool Equals(QueryFilter other)
        {
            if (this.PageIndex != other.PageIndex || this.PageSize != other.PageSize)
                return false;

            if (this.Items == null && other.Items == null)
                return true;

            if (this.Items != null && other.Items != null && this.Items.Count == other.Items.Count)
            {
                int i = 0;
                for (; i < this.Items.Count; i++)
                    if (!this.Items[i].Equals(other.Items[i]))
                        return false;
                return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            if (this.Items == null || this.Items.Count == 0)
                return $"{{\"pageIndex\":{this.PageIndex},\"pageSize\":{this.PageSize},\"items\":[]}}";
            return $"{{\"pageIndex\":{this.PageIndex},\"pageSize\":{this.PageSize},\"items\":[\"{string.Join("\",\"", this.Items.Select(c => c.ToString()).ToArray())}\"]}}";
        }
    }
}