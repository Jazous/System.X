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
    }
}