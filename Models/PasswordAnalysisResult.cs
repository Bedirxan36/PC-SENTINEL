namespace SENTINEL.Models;

public class PasswordAnalysisResult
{
    public double Entropy { get; set; }
    public int Length { get; set; }
    public bool HasUpperCase { get; set; }
    public bool HasLowerCase { get; set; }
    public bool HasDigits { get; set; }
    public bool HasSpecialChars { get; set; }
    public int Score { get; set; }
    public string Strength { get; set; } = string.Empty;
}
