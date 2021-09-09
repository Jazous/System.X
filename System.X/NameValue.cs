using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System
{
    /// <summary>
    /// 表示 System.String 键和 System.String 值的结构。
    /// </summary>
    [global::System.Serializable, global::System.Runtime.Serialization.DataContract, global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct NameValue
    {
        /// <summary>
        /// 获取键/值对中的键。
        /// </summary>
        [global::System.Runtime.Serialization.DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 获取键/值对中的值。
        /// </summary>
        [global::System.Runtime.Serialization.DataMember]
        public string Value { get; set; }

        public NameValue(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is NameValue)
            {
                var other = (NameValue)obj;
                return other.Name == this.Name && other.Value == this.Value;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public bool IsEmpty()
        {
            return (this.Name == null || this.Name.Length == 0) && (this.Value == null || this.Value.Length == 0);
        }
        public override string ToString()
        {
            return $"{{Name:{ Name},Value:{ Value}}}";
        }
        public static bool operator ==(NameValue val1, NameValue val2)
        {
            return val1.Name == val2.Name && val1.Value == val2.Value;
        }
        public static bool operator !=(NameValue val1, NameValue val2)
        {
            return (val1.Name != val2.Name) || (val1.Value != val2.Value);
        }
        public static implicit operator NameValue(System.Collections.Generic.KeyValuePair<string, string> value)
        {
            return new NameValue(value.Key, value.Value);
        }
        public static implicit operator System.Collections.Generic.KeyValuePair<string, string>(NameValue value)
        {
            return new KeyValuePair<string, string>(value.Name, value.Value);
        }
    }
}