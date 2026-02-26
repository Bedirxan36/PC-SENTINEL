using System.Threading.Tasks;
using SENTINEL.Models;

namespace SENTINEL.Services;

public class SecurityScoreService
{
    public Task<SecurityReport> CalculateSecurityScoreAsync(
        bool defenderEnabled,
        bool firewallEnabled,
        bool updatesEnabled,
        int openPortsCount,
        int weakPasswordsCount,
        int duplicatePasswordsCount)
    {
        return Task.Run(() =>
        {
            int score = 100;

            // Sistem güvenliği (45 puan)
            if (!defenderEnabled) score -= 15;
            if (!firewallEnabled) score -= 15;
            if (!updatesEnabled) score -= 15;

            // Port güvenliği (25 puan)
            score -= openPortsCount * 5;

            // Şifre güvenliği (30 puan)
            score -= weakPasswordsCount * 10;
            score -= duplicatePasswordsCount * 10;

            score = System.Math.Max(0, System.Math.Min(100, score));

            var report = new SecurityReport
            {
                OverallScore = score,
                RiskLevel = GetRiskLevel(score),
                DefenderEnabled = defenderEnabled,
                FirewallEnabled = firewallEnabled,
                UpdatesEnabled = updatesEnabled,
                OpenPortsCount = openPortsCount,
                WeakPasswordsCount = weakPasswordsCount,
                DuplicatePasswordsCount = duplicatePasswordsCount
            };

            return report;
        });
    }

    private string GetRiskLevel(int score)
    {
        return score switch
        {
            >= 80 => "Düşük Risk",
            >= 60 => "Orta Risk",
            >= 40 => "Yüksek Risk",
            _ => "Kritik Risk"
        };
    }
}
