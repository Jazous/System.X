namespace System.Data
{
    [global::System.Runtime.Serialization.DataContract, global::System.Serializable]
    public struct QueryItem : IEquatable<QueryItem>
    {
        [global::System.Runtime.Serialization.DataMember]
        public string Name { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public QueryMode Mode { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public string[] Values { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is QueryItem)
                return Equals(this, (QueryItem)obj);
            return false;
        }
        public bool Equals(QueryItem other)
        {
            if (this.Name != other.Name || this.Mode != other.Mode)
                return false;

            if (this.Values == null && other.Values == null)
                return true;

            if (this.Values != null && other.Values != null && this.Values.Length == other.Values.Length)
            {
                int i = 0;
                for (; i < this.Values.Length; i++)
                    if (this.Values[i] != other.Values[i])
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
            if (this.Values == null || this.Values.Length == 0)
                return $"{{\"Name\":\"{Name}\",\"Mode\":\"{Mode}\",\"Values\":[]}}";
            return $"{{\"Name\":\"{Name}\",\"Mode\":\"{Mode}\",\"Values\":[\"{string.Join("\",\"", Values)}\"]}}";
        }
    }
}