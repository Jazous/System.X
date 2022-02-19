namespace System.Data;

/// <summary>
/// Data query filter.
/// </summary>
public struct QueryFilter
{
    /// <summary>
    /// Query condition.
    /// </summary>
    public List<QueryItem> Items { get; set; }
    /// <summary>
    /// Page index.
    /// </summary>
    public int PageIndex { get; set; }
    /// <summary>
    /// Page size.
    /// </summary>
    public int PageSize { get; set; }
}
/// <summary>
/// Data query item.
/// </summary>
public struct QueryItem
{
    /// <summary>
    /// Query property name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Query mode.
    /// </summary>
    public QueryMode Mode { get; set; }
    /// <summary>
    /// Query property values.
    /// </summary>
    public string[] Values { get; set; }
}
/// <summary>
/// Data query mode.
/// </summary>
public enum QueryMode
{
    /// <summary>
    /// Equal
    /// </summary>
    EQ = 0,
    /// <summary>
    /// NotEqual
    /// </summary>
    NTEQ,
    /// <summary>
    /// StartWith
    /// </summary>
    SW,
    /// <summary>
    /// EndWith
    /// </summary>
    EW,
    /// <summary>
    /// Like
    /// </summary>
    LK,
    /// <summary>
    /// In
    /// </summary>
    IN,
    /// <summary>
    /// NotIn
    /// </summary>
    NTIN,
    /// <summary>
    /// GreaterThan
    /// </summary>
    GT,
    /// <summary>
    /// GreaterThanOrEqual
    /// </summary>
    GTEQ,
    /// <summary>
    /// LessThan
    /// </summary>
    LT,
    /// <summary>
    /// LessThanOrEqual
    /// </summary>
    LTEQ
}