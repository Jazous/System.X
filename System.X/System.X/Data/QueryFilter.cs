namespace System.Data
{
    [global::System.Runtime.Serialization.DataContract, global::System.Serializable]
    public sealed class QueryFilter
    {
        public QueryFilter()
        {
            this.Items = new Collections.Generic.List<QueryItem>();
            this.PageIndex = 0;
            this.PageSize = 50;
        }

        [global::System.Runtime.Serialization.DataMember]
        public Collections.Generic.List<QueryItem> Items { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public int PageIndex { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public int PageSize { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public string SortProperty { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public bool Ascending { get; set; }

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
            return base.ToString();
        }
    }
}