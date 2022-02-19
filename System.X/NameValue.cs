namespace System;

/// <summary>
/// Defines a name/value pair that can be set or retrieved.
/// </summary>
public struct NameValue
{
    /// <summary>
    /// The type of the name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The type of the value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Returns the fully qualified type name of this instance.
    /// </summary>
    public override string ToString()
    {
        return $"{{\"Name\":\"{Name}\",\"Value\":\"{Value}\"}}";
    }
}