using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacktimApi.Data;
using StacktimApi.DTOs;
using StacktimApi.Models;

namespace StacktimApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SquadsController : ControllerBase
{
    private readonly StacktimDbContext _context;

    public SquadsController(StacktimDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SquadDto>>> GetAllSquads()
    {
        var squads = await _context.Squads
            .Include(s => s.Leader)
            .Select(s => new SquadDto
            {
                Id = s.Id,
                SquadName = s.SquadName,
                Abbreviation = s.Abbreviation,
                LeaderId = s.LeaderId,
                LeaderNickname = s.Leader!.Nickname,
                FoundationDate = s.FoundationDate
            })
            .ToListAsync();

        return Ok(squads);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SquadDto>> GetSquadById(int id)
    {
        var squad = await _context.Squads
            .Include(s => s.Leader)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (squad == null)
        {
            return NotFound(new { message = $"Squad avec ID {id} introuvable" });
        }

        var dto = new SquadDto
        {
            Id = squad.Id,
            SquadName = squad.SquadName,
            Abbreviation = squad.Abbreviation,
            LeaderId = squad.LeaderId,
            LeaderNickname = squad.Leader!.Nickname,
            FoundationDate = squad.FoundationDate
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<SquadDto>> CreateSquad(CreateSquadDto dto)
    {
        if (await _context.Squads.AnyAsync(s => s.SquadName == dto.SquadName))
        {
            return BadRequest(new { message = "Ce nom d'équipe est déjà utilisé" });
        }

        if (await _context.Squads.AnyAsync(s => s.Abbreviation == dto.Abbreviation))
        {
            return BadRequest(new { message = "Cette abréviation est déjà utilisée" });
        }

        var leader = await _context.Competitors.FindAsync(dto.LeaderId);
        if (leader == null)
        {
            return BadRequest(new { message = "Le leader spécifié n'existe pas" });
        }

        var squad = new Squad
        {
            SquadName = dto.SquadName,
            Abbreviation = dto.Abbreviation.ToUpper(),
            LeaderId = dto.LeaderId,
            FoundationDate = DateTime.Now
        };

        _context.Squads.Add(squad);
        await _context.SaveChangesAsync();

        var memberEntry = new SquadMember
        {
            SquadId = squad.Id,
            CompetitorId = dto.LeaderId,
            Position = 0, 
            MembershipDate = DateTime.Now
        };

        _context.SquadMembers.Add(memberEntry);
        await _context.SaveChangesAsync();

        var squadDto = new SquadDto
        {
            Id = squad.Id,
            SquadName = squad.SquadName,
            Abbreviation = squad.Abbreviation,
            LeaderId = squad.LeaderId,
            LeaderNickname = leader.Nickname,
            FoundationDate = squad.FoundationDate
        };

        return CreatedAtAction(nameof(GetSquadById), new { id = squad.Id }, squadDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSquad(int id, UpdateSquadDto dto)
    {
        var squad = await _context.Squads.FindAsync(id);
        if (squad == null)
        {
            return NotFound(new { message = $"Squad avec ID {id} introuvable" });
        }

        if (await _context.Squads.AnyAsync(s => s.SquadName == dto.SquadName && s.Id != id))
        {
            return BadRequest(new { message = "Ce nom d'équipe est déjà utilisé par une autre équipe" });
        }

        if (await _context.Squads.AnyAsync(s => s.Abbreviation == dto.Abbreviation && s.Id != id))
        {
            return BadRequest(new { message = "Cette abréviation est déjà utilisée par une autre équipe" });
        }

        squad.SquadName = dto.SquadName;
        squad.Abbreviation = dto.Abbreviation.ToUpper();

        if (dto.LeaderId != squad.LeaderId)
        {
            var leader = await _context.Competitors.FindAsync(dto.LeaderId);
            if (leader == null)
            {
                return BadRequest(new { message = "Le nouveau leader spécifié n'existe pas" });
            }

            squad.LeaderId = dto.LeaderId;
        }

        _context.Entry(squad).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSquad(int id)
    {
        var squad = await _context.Squads.FindAsync(id);
        if (squad == null)
        {
            return NotFound(new { message = $"Squad avec ID {id} introuvable" });
        }

        var members = _context.SquadMembers.Where(m => m.SquadId == id);
        _context.SquadMembers.RemoveRange(members);

        _context.Squads.Remove(squad);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
