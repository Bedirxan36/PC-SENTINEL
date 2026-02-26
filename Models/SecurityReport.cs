namespace SENTINEL.Models;

public class SecurityReport
{
    public int OverallScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public bool DefenderEnabled { get; set; }
    public bool FirewallEnabled { get; set; }
    public bool UpdatesEnabled { get; set; }
    public int OpenPortsCount { get; set; }
    public int WeakPasswordsCount { get; set; }
    public int DuplicatePasswordsCount { get; set; }
}
