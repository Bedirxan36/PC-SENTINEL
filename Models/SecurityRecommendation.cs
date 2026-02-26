namespace SENTINEL.Models;

public class SecurityRecommendation
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty; // High, Medium, Low
    public string Category { get; set; } = string.Empty;
}
