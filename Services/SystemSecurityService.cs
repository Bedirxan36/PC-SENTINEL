using System;
using System.Management;
using System.Threading.Tasks;

namespace SENTINEL.Services;

public class SystemSecurityService
{
    public Task<bool> CheckWindowsDefenderAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(@"root\Microsoft\Windows\Defender", "SELECT * FROM MSFT_MpComputerStatus");
                foreach (ManagementObject obj in searcher.Get())
                {
                    return (bool)(obj["AntivirusEnabled"] ?? false);
                }
            }
            catch { }
            return false;
        });
    }

    public Task<bool> CheckFirewallAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(@"root\StandardCimv2", "SELECT * FROM MSFT_NetFirewallProfile");
                foreach (ManagementObject obj in searcher.Get())
                {
                    if ((bool)(obj["Enabled"] ?? false))
                        return true;
                }
            }
            catch { }
            return false;
        });
    }

    public Task<bool> CheckWindowsUpdateAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service WHERE Name='wuauserv'");
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["State"]?.ToString() == "Running";
                }
            }
            catch { }
            return false;
        });
    }
}
