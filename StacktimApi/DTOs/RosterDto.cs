namespace StacktimApi.DTOs;

public class RosterDto
{
    public string SquadName { get; set; } = string.Empty;
    public List<RosterMemberDto> Members { get; set; } = new();
}

public class RosterMemberDto
{
    public string Nickname { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; }
}