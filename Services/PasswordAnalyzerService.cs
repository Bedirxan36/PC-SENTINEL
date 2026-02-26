using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SENTINEL.Models;

namespace SENTINEL.Services;

public class PasswordAnalyzerService
{
    public Task<PasswordAnalysisResult> AnalyzePasswordAsync(string password)
    {
        return Task.Run(() =>
        {
            var result = new PasswordAnalysisResult
            {
                Length = password.Length,
                HasUpperCase = password.Any(char.IsUpper),
                HasLowerCase = password.Any(char.IsLower),
                HasDigits = password.Any(char.IsDigit),
                HasSpecialChars = password.Any(c => !char.IsLetterOrDigit(c)),
                Entropy = CalculateEntropy(password)
            };

            result.Score = CalculateScore(result);
            result.Strength = GetStrength(result.Score);

            return result;
        });
    }

    public Task<bool> CheckDuplicatePasswordsAsync(string[] passwords)
    {
        return Task.Run(() =>
        {
            var hashes = passwords.Select(HashPassword).ToList();
            return hashes.Count != hashes.Distinct().Count();
        });
    }

    private double CalculateEntropy(string password)
    {
        if (string.IsNullOrEmpty(password)) return 0;

        var charSet = 0;
        if (password.Any(char.IsLower)) charSet += 26;
        if (password.Any(char.IsUpper)) charSet += 26;
        if (password.Any(char.IsDigit)) charSet += 10;
        if (password.Any(c => !char.IsLetterOrDigit(c))) charSet += 32;

        return password.Length * Math.Log2(charSet);
    }

    private int CalculateScore(PasswordAnalysisResult result)
    {
        int score = 0;

        // Uzunluk skoru (0-40)
        score += Math.Min(result.Length * 4, 40);

        // Karakter çeşitliliği (0-40)
        if (result.HasUpperCase) score += 10;
        if (result.HasLowerCase) score += 10;
        if (result.HasDigits) score += 10;
        if (result.HasSpecialChars) score += 10;

        // Entropy skoru (0-20)
        score += (int)Math.Min(result.Entropy / 5, 20);

        return Math.Min(score, 100);
    }

    private string GetStrength(int score)
    {
        return score switch
        {
            >= 80 => "Çok Güçlü",
            >= 60 => "Güçlü",
            >= 40 => "Orta",
            >= 20 => "Zayıf",
            _ => "Çok Zayıf"
        };
    }

    private string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
