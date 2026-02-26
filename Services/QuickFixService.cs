using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SENTINEL.Services;

public class QuickFixService
{
    public async Task<bool> EnableWindowsDefenderAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"Set-MpPreference -DisableRealtimeMonitoring $false\"",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };
                
                var process = Process.Start(psi);
                process?.WaitForExit();
                return process?.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        });
    }

    public async Task<bool> EnableFirewallAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "advfirewall set allprofiles state on",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };
                
                var process = Process.Start(psi);
                process?.WaitForExit();
                return process?.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        });
    }

    public async Task<bool> CheckWindowsUpdatesAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "ms-settings:windowsupdate",
                    UseShellExecute = true
                };
                
                Process.Start(psi);
                return true;
            }
            catch
            {
                return false;
            }
        });
    }

    public async Task<bool> OpenWindowsSecurityAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "windowsdefender:",
                    UseShellExecute = true
                };
                
                Process.Start(psi);
                return true;
            }
            catch
            {
                return false;
            }
        });
    }
}
