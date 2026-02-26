using System.Collections.Generic;
using System.Threading.Tasks;
using SENTINEL.Models;

namespace SENTINEL.Services;

public class RecommendationService
{
    public Task<List<SecurityRecommendation>> GenerateRecommendationsAsync(SecurityReport report)
    {
        return Task.Run(() =>
        {
            var recommendations = new List<SecurityRecommendation>();

            if (!report.DefenderEnabled)
            {
                recommendations.Add(new SecurityRecommendation
                {
                    Title = "Windows Defender'ı Etkinleştirin",
                    Description = "Gerçek zamanlı koruma için Windows Defender'ı açmanız önerilir.",
                    Priority = "Yüksek",
                    Category = "Sistem Güvenliği"
                });
            }

            if (!report.FirewallEnabled)
            {
                recommendations.Add(new SecurityRecommendation
                {
                    Title = "Firewall'u Aktif Edin",
                    Description = "Ağ saldırılarına karşı korunmak için firewall'u etkinleştirin.",
                    Priority = "Yüksek",
                    Category = "Ağ Güvenliği"
                });
            }

            if (!report.UpdatesEnabled)
            {
                recommendations.Add(new SecurityRecommendation
                {
                    Title = "Windows Update'i Çalıştırın",
                    Description = "Güvenlik yamalarını almak için otomatik güncellemeleri açın.",
                    Priority = "Yüksek",
                    Category = "Sistem Güvenliği"
                });
            }

            if (report.OpenPortsCount > 5)
            {
                recommendations.Add(new SecurityRecommendation
                {
                    Title = "Açık Portları Azaltın",
                    Description = $"{report.OpenPortsCount} açık port tespit edildi. Gereksiz servisleri kapatın.",
                    Priority = "Orta",
                    Category = "Ağ Güvenliği"
                });
            }

            if (report.WeakPasswordsCount > 0)
            {
                recommendations.Add(new SecurityRecommendation
                {
                    Title = "Güçlü Şifreler Kullanın",
                    Description = "Zayıf şifrelerinizi en az 12 karakter, büyük/küçük harf, rakam ve özel karakter içerecek şekilde değiştirin.",
                    Priority = "Yüksek",
                    Category = "Şifre Güvenliği"
                });
            }

            if (report.DuplicatePasswordsCount > 0)
            {
                recommendations.Add(new SecurityRecommendation
                {
                    Title = "Tekrar Eden Şifreleri Değiştirin",
                    Description = "Her hesap için farklı şifre kullanın. Şifre yöneticisi kullanmayı düşünün.",
                    Priority = "Orta",
                    Category = "Şifre Güvenliği"
                });
            }

            // Genel öneriler
            recommendations.Add(new SecurityRecommendation
            {
                Title = "İki Faktörlü Kimlik Doğrulama",
                Description = "Tüm önemli hesaplarınızda 2FA'yı etkinleştirin.",
                Priority = "Orta",
                Category = "Hesap Güvenliği"
            });

            recommendations.Add(new SecurityRecommendation
            {
                Title = "Düzenli Yedekleme",
                Description = "Önemli verilerinizi düzenli olarak yedekleyin.",
                Priority = "Orta",
                Category = "Veri Güvenliği"
            });

            return recommendations;
        });
    }
}
