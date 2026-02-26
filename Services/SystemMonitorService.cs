using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using SENTINEL.Models;

namespace SENTINEL.Services;

public class SystemMonitorService
{
    private PerformanceCounter? _cpuCounter;
    private PerformanceCounter? _diskCounter;

    public SystemMonitorService()
    {
        try
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            _cpuCounter.NextValue(); // İlk dummy okuma
            Thread.Sleep(100);
        }
        catch
        {
            _cpuCounter = null;
        }

        try
        {
            // C: sürücüsü için disk kullanımı
            _diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total", true);
            _diskCounter.NextValue();
        }
        catch
        {
            _diskCounter = null;
        }
    }

    public Task<SystemMetrics> GetSystemMetricsAsync()
    {
        return Task.Run(() =>
        {
            var metrics = new SystemMetrics();

            try
            {
                // CPU - PerformanceCounter ile
                if (_cpuCounter != null)
                {
                    Thread.Sleep(100); // Kısa bekleme
                    var cpuValue = _cpuCounter.NextValue();
                    metrics.CpuUsage = Math.Min(100, Math.Max(0, cpuValue));
                }

                // RAM - WMI ile (Görev Yöneticisi ile aynı)
                metrics.RamUsage = GetMemoryUsageWMI();

                // Disk - PerformanceCounter ile
                if (_diskCounter != null)
                {
                    Thread.Sleep(100);
                    var diskValue = _diskCounter.NextValue();
                    metrics.DiskUsage = Math.Min(100, Math.Max(0, diskValue));
                }

                // Aktif bağlantılar
                metrics.ActiveConnections = GetActiveConnectionsCount();
            }
            catch { }

            return metrics;
        });
    }

    private float GetMemoryUsageWMI()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                var totalKB = Convert.ToUInt64(obj["TotalVisibleMemorySize"]);
                var freeKB = Convert.ToUInt64(obj["FreePhysicalMemory"]);
                var usedKB = totalKB - freeKB;
                
                return (float)Math.Round((usedKB * 100.0) / totalKB, 1);
            }
        }
        catch { }

        return 0;
    }

    private int GetActiveConnectionsCount()
    {
        try
        {
            var connections = System.Net.NetworkInformation.IPGlobalProperties
                .GetIPGlobalProperties()
                .GetActiveTcpConnections()
                .Where(c => c.State == System.Net.NetworkInformation.TcpState.Established);
            return connections.Count();
        }
        catch
        {
            return 0;
        }
    }
}
