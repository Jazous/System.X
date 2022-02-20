namespace System.SDKs.Thermal;

public sealed class AreaTemperature
{
    public float MinTemp { get; set; }
    public float MinX { get; set; }
    public float MinY { get; set; }
    public float MaxTemp { get; set; }
    public int MaxX { get; set; }
    public int MaxY { get; set; }
    public float AvgTemp { get; set; }
}