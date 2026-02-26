using System;

namespace SENTINEL.Models;

public class PasswordHistory
{
    public string PasswordHash { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime AnalyzedAt { get; set; }
    public string Strength { get; set; } = string.Empty;
}
