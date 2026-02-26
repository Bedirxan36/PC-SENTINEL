using System.Collections.Generic;

namespace SENTINEL.Models;

public class SystemInfo
{
    // İşletim Sistemi
    public string OsName { get; set; } = string.Empty;
    public string OsVersion { get; set; } = string.Empty;
    public string OsArchitecture { get; set; } = string.Empty;
    public string ComputerName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string SystemUptime { get; set; } = string.Empty;

    // İşlemci
    public string CpuName { get; set; } = string.Empty;
    public string CpuManufacturer { get; set; } = string.Empty;
    public int CpuCores { get; set; }
    public int CpuLogicalProcessors { get; set; }
    public string CpuMaxSpeed { get; set; } = string.Empty;
    public string CpuArchitecture { get; set; } = string.Empty;

    // Bellek
    public string TotalRam { get; set; } = string.Empty;
    public string AvailableRam { get; set; } = string.Empty;
    public string UsedRam { get; set; } = string.Empty;
    public int RamSlots { get; set; }
    public string RamSpeed { get; set; } = string.Empty;

    // Ekran Kartı
    public string GpuName { get; set; } = string.Empty;
    public string GpuDriverVersion { get; set; } = string.Empty;
    public string GpuMemory { get; set; } = string.Empty;
    public string GpuResolution { get; set; } = string.Empty;

    // Anakart
    public string MotherboardManufacturer { get; set; } = string.Empty;
    public string MotherboardProduct { get; set; } = string.Empty;
    public string BiosVersion { get; set; } = string.Empty;

    // Diskler
    public List<DiskInfo> Disks { get; set; } = new();

    // Ağ
    public string MacAddress { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string NetworkAdapter { get; set; } = string.Empty;
}

public class DiskInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string FileSystem { get; set; } = string.Empty;
    public string TotalSize { get; set; } = string.Empty;
    public string FreeSpace { get; set; } = string.Empty;
    public string UsedSpace { get; set; } = string.Empty;
    public float UsedPercentage { get; set; }
}
