using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SENTINEL.Models;

namespace SENTINEL.Services;

public class ReportExportService
{
    public async Task<string> ExportToJsonAsync(SecurityReport report, SystemInfo? systemInfo)
    {
        return await Task.Run(() =>
        {
            var data = new
            {
                ExportDate = DateTime.Now,
                SecurityReport = report,
                SystemInfo = systemInfo
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(data, options);
        });
    }

    public async Task<string> ExportToCsvAsync(SecurityReport report)
    {
        return await Task.Run(() =>
        {
            var sb = new StringBuilder();
            sb.AppendLine("Metrik,Değer");
            sb.AppendLine($"Güvenlik Skoru,{report.OverallScore}");
            sb.AppendLine($"Risk Seviyesi,{report.RiskLevel}");
            sb.AppendLine($"Windows Defender,{(report.DefenderEnabled ? "Aktif" : "Devre Dışı")}");
            sb.AppendLine($"Firewall,{(report.FirewallEnabled ? "Aktif" : "Devre Dışı")}");
            sb.AppendLine($"Windows Update,{(report.UpdatesEnabled ? "Aktif" : "Devre Dışı")}");
            sb.AppendLine($"Açık Port Sayısı,{report.OpenPortsCount}");
            sb.AppendLine($"Tarih,{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            
            return sb.ToString();
        });
    }

    public async Task SaveReportAsync(string content, string fileName, string filter)
    {
        await Task.Run(() =>
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = fileName,
                Filter = filter,
                DefaultExt = Path.GetExtension(fileName)
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, content);
            }
        });
    }
}
