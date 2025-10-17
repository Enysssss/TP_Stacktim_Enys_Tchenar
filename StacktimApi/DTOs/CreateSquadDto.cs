using System.ComponentModel.DataAnnotations;

namespace StacktimApi.DTOs;

public class CreateSquadDto
{
    [Required(ErrorMessage = "Le nom de l'équipe est obligatoire")]
    [StringLength(100, MinimumLength = 3)]
    public string SquadName { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'abréviation est obligatoire")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "L'abréviation doit contenir exactement 3 caractères")]
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "L'abréviation doit contenir 3 lettres majuscules")]
    public string Abbreviation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le leader est obligatoire")]
    public int LeaderId { get; set; }
}