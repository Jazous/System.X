namespace System.Data
{
    [global::System.Runtime.Serialization.DataContract, global::System.Serializable]
    public struct QueryItem
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

            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $"Name:{Name},Mode:{Mode},Values:[{string.Join(",", Values)}]";
        }
    }
}