using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StacktimApi.Models;

[Table("Competitors")]
public class Competitor
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Nickname { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string RankLevel { get; set; } = "Bronze";

    public int AccumulatedPoints { get; set; } = 0;

    public DateTime EnrollmentDate { get; set; } = DateTime.Now;

    // Navigation
    public virtual ICollection<Squad> LeadingSquads { get; set; } = new List<Squad>();
    public virtual ICollection<SquadMember> Memberships { get; set; } = new List<SquadMember>();
}