namespace System;

/// <summary>
/// 表示 System.String 键和 System.String 值的结构。
/// </summary>
public struct NameValue
{
    /// <summary>
    /// 获取键/值对中的键。
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 获取键/值对中的值。
    /// </summary>
    public string Value { get; set; }

    public override string ToString()
    {
        return $"{{\"Name\":\"{ Name}\",\"Value\":\"{ Value}\"}}";
    }
}