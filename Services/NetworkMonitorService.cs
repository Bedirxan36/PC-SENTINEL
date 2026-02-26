using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using SENTINEL.Models;

namespace SENTINEL.Services;

public class NetworkMonitorService
{
    public Task<List<NetworkConnection>> GetActiveConnectionsAsync()
    {
        return Task.Run(() =>
        {
            var connections = new List<NetworkConnection>();

            try
            {
                var tcpConnections = IPGlobalProperties.GetIPGlobalProperties()
                    .GetActiveTcpConnections()
                    .Take(20); // İlk 20 bağlantı

                foreach (var conn in tcpConnections)
                {
                    connections.Add(new NetworkConnection
                    {
                        Protocol = "TCP",
                        LocalAddress = $"{conn.LocalEndPoint.Address}:{conn.LocalEndPoint.Port}",
                        RemoteAddress = $"{conn.RemoteEndPoint.Address}:{conn.RemoteEndPoint.Port}",
                        State = conn.State.ToString(),
                        ProcessName = "Unknown"
                    });
                }
            }
            catch { }

            return connections;
        });
    }

    public Task<(long sent, long received)> GetNetworkStatisticsAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                                 ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                long totalSent = 0;
                long totalReceived = 0;

                foreach (var ni in interfaces)
                {
                    var stats = ni.GetIPv4Statistics();
                    totalSent += stats.BytesSent;
                    totalReceived += stats.BytesReceived;
                }

                return (totalSent, totalReceived);
            }
            catch
            {
                return (0L, 0L);
            }
        });
    }
}
