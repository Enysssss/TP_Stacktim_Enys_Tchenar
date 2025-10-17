namespace StacktimApi.DTOs;

public class CompetitorDto
{
    public int Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string RankLevel { get; set; } = string.Empty;
    public int AccumulatedPoints { get; set; }
    public DateTime EnrollmentDate { get; set; }
}