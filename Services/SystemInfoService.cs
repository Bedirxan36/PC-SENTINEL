using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using SENTINEL.Models;

namespace SENTINEL.Services;

public class SystemInfoService
{
    public Task<SystemInfo> GetSystemInfoAsync()
    {
        return Task.Run(() =>
        {
            var info = new SystemInfo();

            try
            {
                GetOperatingSystemInfo(info);
                GetProcessorInfo(info);
                GetMemoryInfo(info);
                GetGraphicsInfo(info);
                GetMotherboardInfo(info);
                GetDiskInfo(info);
                GetNetworkInfo(info);
            }
            catch { }

            return info;
        });
    }

    private void GetOperatingSystemInfo(SystemInfo info)
    {
        try
        {
            info.ComputerName = Environment.MachineName;
            info.UserName = Environment.UserName;
            info.OsArchitecture = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";

            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                info.OsName = obj["Caption"]?.ToString() ?? "Unknown";
                info.OsVersion = obj["Version"]?.ToString() ?? "Unknown";
                
                var lastBootTime = ManagementDateTimeConverter.ToDateTime(obj["LastBootUpTime"]?.ToString() ?? "");
                var uptime = DateTime.Now - lastBootTime;
                info.SystemUptime = $"{uptime.Days} gün, {uptime.Hours} saat, {uptime.Minutes} dakika";
            }
        }
        catch { }
    }

    private void GetProcessorInfo(SystemInfo info)
    {
        try
        {
            info.CpuCores = Environment.ProcessorCount;
            
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                info.CpuName = obj["Name"]?.ToString()?.Trim() ?? "Unknown";
                info.CpuManufacturer = obj["Manufacturer"]?.ToString() ?? "Unknown";
                info.CpuLogicalProcessors = Convert.ToInt32(obj["NumberOfLogicalProcessors"] ?? 0);
                
                var maxSpeed = Convert.ToInt32(obj["MaxClockSpeed"] ?? 0);
                info.CpuMaxSpeed = $"{maxSpeed / 1000.0:F2} GHz";
                
                var arch = obj["Architecture"]?.ToString();
                info.CpuArchitecture = arch switch
                {
                    "0" => "x86",
                    "9" => "x64",
                    "12" => "ARM64",
                    _ => "Unknown"
                };
            }
        }
        catch { }
    }

    private void GetMemoryInfo(SystemInfo info)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                var totalKB = Convert.ToUInt64(obj["TotalVisibleMemorySize"]);
                var freeKB = Convert.ToUInt64(obj["FreePhysicalMemory"]);
                var usedKB = totalKB - freeKB;

                info.TotalRam = FormatBytes(totalKB * 1024);
                info.AvailableRam = FormatBytes(freeKB * 1024);
                info.UsedRam = FormatBytes(usedKB * 1024);
            }

            // RAM modülleri
            using var memSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            var memCount = 0;
            foreach (ManagementObject obj in memSearcher.Get())
            {
                memCount++;
                if (memCount == 1)
                {
                    var speed = obj["Speed"]?.ToString();
                    info.RamSpeed = speed != null ? $"{speed} MHz" : "Unknown";
                }
            }
            info.RamSlots = memCount;
        }
        catch { }
    }

    private void GetGraphicsInfo(SystemInfo info)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (ManagementObject obj in searcher.Get())
            {
                info.GpuName = obj["Name"]?.ToString() ?? "Unknown";
                info.GpuDriverVersion = obj["DriverVersion"]?.ToString() ?? "Unknown";
                
                var adapterRam = obj["AdapterRAM"];
                if (adapterRam != null)
                {
                    var ramBytes = Convert.ToUInt64(adapterRam);
                    info.GpuMemory = FormatBytes(ramBytes);
                }

                var width = obj["CurrentHorizontalResolution"];
                var height = obj["CurrentVerticalResolution"];
                if (width != null && height != null)
                {
                    info.GpuResolution = $"{width} x {height}";
                }
                
                break; // İlk GPU'yu al
            }
        }
        catch { }
    }

    private void GetMotherboardInfo(SystemInfo info)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            foreach (ManagementObject obj in searcher.Get())
            {
                info.MotherboardManufacturer = obj["Manufacturer"]?.ToString() ?? "Unknown";
                info.MotherboardProduct = obj["Product"]?.ToString() ?? "Unknown";
            }

            using var biosSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            foreach (ManagementObject obj in biosSearcher.Get())
            {
                info.BiosVersion = obj["SMBIOSBIOSVersion"]?.ToString() ?? "Unknown";
            }
        }
        catch { }
    }

    private void GetDiskInfo(SystemInfo info)
    {
        try
        {
            var drives = System.IO.DriveInfo.GetDrives().Where(d => d.IsReady);
            
            foreach (var drive in drives)
            {
                var diskInfo = new DiskInfo
                {
                    Name = drive.Name,
                    Type = drive.DriveType.ToString(),
                    FileSystem = drive.DriveFormat,
                    TotalSize = FormatBytes(drive.TotalSize),
                    FreeSpace = FormatBytes(drive.AvailableFreeSpace),
                    UsedSpace = FormatBytes(drive.TotalSize - drive.AvailableFreeSpace),
                    UsedPercentage = (float)((drive.TotalSize - drive.AvailableFreeSpace) * 100.0 / drive.TotalSize)
                };
                
                info.Disks.Add(diskInfo);
            }
        }
        catch { }
    }

    private void GetNetworkInfo(SystemInfo info)
    {
        try
        {
            var adapters = NetworkInterface.GetAllNetworkInterfaces()
                .Where(a => a.OperationalStatus == OperationalStatus.Up && 
                           a.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .ToList();

            if (adapters.Any())
            {
                var adapter = adapters.First();
                info.NetworkAdapter = adapter.Name;
                info.MacAddress = adapter.GetPhysicalAddress().ToString();
                
                var ipProps = adapter.GetIPProperties();
                var ipv4 = ipProps.UnicastAddresses
                    .FirstOrDefault(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                
                if (ipv4 != null)
                {
                    info.IpAddress = ipv4.Address.ToString();
                }
            }
        }
        catch { }
    }

    private string FormatBytes(ulong bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        
        return $"{len:F2} {sizes[order]}";
    }

    private string FormatBytes(long bytes)
    {
        return FormatBytes((ulong)bytes);
    }
}
