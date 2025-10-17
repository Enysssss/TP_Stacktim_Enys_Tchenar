using System.ComponentModel.DataAnnotations;

namespace StacktimApi.DTOs;

public class UpdateCompetitorDto
{
    [StringLength(50, MinimumLength = 3)]
    public string? Nickname { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? EmailAddress { get; set; }

    [RegularExpression("^(Bronze|Silver|Gold|Platinum|Diamond|Master)$")]
    public string? RankLevel { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Le score ne peut pas être négatif")]
    public int? AccumulatedPoints { get; set; }
}