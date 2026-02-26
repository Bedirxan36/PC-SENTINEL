namespace SENTINEL.Models;

public class NetworkConnection
{
    public string Protocol { get; set; } = string.Empty;
    public string LocalAddress { get; set; } = string.Empty;
    public string RemoteAddress { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
}
