namespace SENTINEL.Models;

public class PortScanResult
{
    public int Port { get; set; }
    public bool IsOpen { get; set; }
    public string ServiceName { get; set; } = string.Empty;
}
