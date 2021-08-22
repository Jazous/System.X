using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data
{
    [global::System.Serializable]
    [global::System.Runtime.Serialization.DataContract]
    public sealed class QueryItem
    {
        [global::System.Runtime.Serialization.DataMember]
        public string Name { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public QueryMode Mode { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public string[] Values { get; set; }

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
            return $"Name:{Name},Mode:{Mode},Values:[{string.Join(",", Values)}]";
        }
    }
}