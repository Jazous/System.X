namespace System.Data;

public struct QueryFilter
{
    public List<QueryItem> Items { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}
public struct QueryItem
{
    public string Name { get; set; }
    public QueryMode Mode { get; set; }
    public string[] Values { get; set; }
}
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