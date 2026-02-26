using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using SENTINEL.Models;

namespace SENTINEL.Services;

public class PortScannerService
{
    private readonly Dictionary<int, string> _commonPorts = new()
    {
        { 21, "FTP" }, { 22, "SSH" }, { 23, "Telnet" }, { 25, "SMTP" },
        { 53, "DNS" }, { 80, "HTTP" }, { 110, "POP3" }, { 143, "IMAP" },
        { 443, "HTTPS" }, { 445, "SMB" }, { 3306, "MySQL" }, { 3389, "RDP" },
        { 5432, "PostgreSQL" }, { 5900, "VNC" }, { 8080, "HTTP-Alt" }
    };

    public async Task<List<PortScanResult>> ScanPortsAsync(int[] ports, int timeoutMs = 1000)
    {
        var results = new List<PortScanResult>();
        var tasks = new List<Task<PortScanResult>>();

        foreach (var port in ports)
        {
            tasks.Add(ScanPortAsync(port, timeoutMs));
        }

        var scanResults = await Task.WhenAll(tasks);
        results.AddRange(scanResults);

        return results;
    }

    public Task<List<PortScanResult>> ScanCommonPortsAsync()
    {
        return ScanPortsAsync(_commonPorts.Keys.ToArray());
    }

    private async Task<PortScanResult> ScanPortAsync(int port, int timeoutMs)
    {
        using var client = new TcpClient();
        var result = new PortScanResult
        {
            Port = port,
            ServiceName = _commonPorts.GetValueOrDefault(port, "Unknown")
        };

        try
        {
            var connectTask = client.ConnectAsync("127.0.0.1", port);
            var timeoutTask = Task.Delay(timeoutMs);

            var completedTask = await Task.WhenAny(connectTask, timeoutTask);

            result.IsOpen = completedTask == connectTask && client.Connected;
        }
        catch
        {
            result.IsOpen = false;
        }

        return result;
    }
}
