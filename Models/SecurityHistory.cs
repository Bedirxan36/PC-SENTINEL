using System;

namespace SENTINEL.Models;

public class SecurityHistory
{
    public DateTime Timestamp { get; set; }
    public int SecurityScore { get; set; }
    public int OpenPortsCount { get; set; }
    public bool DefenderEnabled { get; set; }
    public bool FirewallEnabled { get; set; }
    public float CpuUsage { get; set; }
    public float RamUsage { get; set; }
}
