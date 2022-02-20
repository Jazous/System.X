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
    NTEQ = 1,
    /// <summary>
    /// GreaterThan
    /// </summary>
    GT = 2,
    /// <summary>
    /// GreaterThanOrEqual
    /// </summary>
    GTEQ = 3,
    /// <summary>
    /// LessThan
    /// </summary>
    LT = 4,
    /// <summary>
    /// LessThanOrEqual
    /// </summary>
    LTEQ = 5,
    /// <summary>
    /// In
    /// </summary>
    IN = 6,
    /// <summary>
    /// NotIn
    /// </summary>
    NTIN = 7,
    /// <summary>
    /// Like
    /// </summary>
    LK = 8,
    /// <summary>
    /// StartWith
    /// </summary>
    /// 
    SW = 9,
    /// <summary>
    /// EndWith
    /// </summary>
    EW = 10
}