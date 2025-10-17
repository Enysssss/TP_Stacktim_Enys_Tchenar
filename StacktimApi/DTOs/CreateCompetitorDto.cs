using System.ComponentModel.DataAnnotations;

namespace StacktimApi.DTOs;

public class CreateCompetitorDto
{
    [Required(ErrorMessage = "Le pseudo est obligatoire")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Le pseudo doit contenir entre 3 et 50 caractères")]
    public string Nickname { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est obligatoire")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    [StringLength(100)]
    public string EmailAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le rank est obligatoire")]
    [RegularExpression("^(Bronze|Silver|Gold|Platinum|Diamond|Master)$", 
        ErrorMessage = "Le rank doit être : Bronze, Silver, Gold, Platinum, Diamond ou Master")]
    public string RankLevel { get; set; } = "Bronze";
}