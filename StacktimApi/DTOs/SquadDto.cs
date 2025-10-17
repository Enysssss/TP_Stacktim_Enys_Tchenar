namespace StacktimApi.DTOs;

public class SquadDto
{
    public int Id { get; set; }
    public string SquadName { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public int LeaderId { get; set; }
    public string LeaderNickname { get; set; } = string.Empty;
    public DateTime FoundationDate { get; set; }
}