namespace SENTINEL.Models;

public class SystemMetrics
{
    public float CpuUsage { get; set; }
    public float RamUsage { get; set; }
    public float DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public long NetworkUpload { get; set; }
    public long NetworkDownload { get; set; }
}
