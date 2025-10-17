using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacktimApi.Data;
using StacktimApi.DTOs;
using StacktimApi.Models;

namespace StacktimApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompetitorsController : ControllerBase
{
    private readonly StacktimDbContext _context;

    public CompetitorsController(StacktimDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompetitorDto>>> GetAllCompetitors()
    {
        var competitors = await _context.Competitors
            .Select(c => new CompetitorDto
            {
                Id = c.Id,
                Nickname = c.Nickname,
                EmailAddress = c.EmailAddress,
                RankLevel = c.RankLevel,
                AccumulatedPoints = c.AccumulatedPoints,
                EnrollmentDate = c.EnrollmentDate
            })
            .ToListAsync();

        return Ok(competitors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompetitorDto>> GetCompetitorById(int id)
    {
        var competitor = await _context.Competitors.FindAsync(id);

        if (competitor == null)
        {
            return NotFound(new { message = $"Compétiteur avec ID {id} introuvable" });
        }

        var dto = new CompetitorDto
        {
            Id = competitor.Id,
            Nickname = competitor.Nickname,
            EmailAddress = competitor.EmailAddress,
            RankLevel = competitor.RankLevel,
            AccumulatedPoints = competitor.AccumulatedPoints,
            EnrollmentDate = competitor.EnrollmentDate
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<CompetitorDto>> CreateCompetitor(CreateCompetitorDto dto)
    {
        if (await _context.Competitors.AnyAsync(c => c.Nickname == dto.Nickname))
        {
            return BadRequest(new { message = "Ce pseudo est déjà utilisé" });
        }

        if (await _context.Competitors.AnyAsync(c => c.EmailAddress == dto.EmailAddress))
        {
            return BadRequest(new { message = "Cet email est déjà utilisé" });
        }

        var competitor = new Competitor
        {
            Nickname = dto.Nickname,
            EmailAddress = dto.EmailAddress,
            RankLevel = dto.RankLevel,
            AccumulatedPoints = 0,
            EnrollmentDate = DateTime.Now
        };

        _context.Competitors.Add(competitor);
        await _context.SaveChangesAsync();

        var resultDto = new CompetitorDto
        {
            Id = competitor.Id,
            Nickname = competitor.Nickname,
            EmailAddress = competitor.EmailAddress,
            RankLevel = competitor.RankLevel,
            AccumulatedPoints = competitor.AccumulatedPoints,
            EnrollmentDate = competitor.EnrollmentDate
        };

        return CreatedAtAction(nameof(GetCompetitorById), new { id = competitor.Id }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompetitor(int id, UpdateCompetitorDto dto)
    {
        var competitor = await _context.Competitors.FindAsync(id);

        if (competitor == null)
        {
            return NotFound(new { message = $"Compétiteur avec ID {id} introuvable" });
        }

        if (!string.IsNullOrEmpty(dto.Nickname) && dto.Nickname != competitor.Nickname)
        {
            if (await _context.Competitors.AnyAsync(c => c.Nickname == dto.Nickname))
            {
                return BadRequest(new { message = "Ce pseudo est déjà utilisé" });
            }
            competitor.Nickname = dto.Nickname;
        }

        if (!string.IsNullOrEmpty(dto.EmailAddress) && dto.EmailAddress != competitor.EmailAddress)
        {
            if (await _context.Competitors.AnyAsync(c => c.EmailAddress == dto.EmailAddress))
            {
                return BadRequest(new { message = "Cet email est déjà utilisé" });
            }
            competitor.EmailAddress = dto.EmailAddress;
        }

        if (!string.IsNullOrEmpty(dto.RankLevel))
        {
            competitor.RankLevel = dto.RankLevel;
        }

        if (dto.AccumulatedPoints.HasValue)
        {
            competitor.AccumulatedPoints = dto.AccumulatedPoints.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompetitor(int id)
    {
        var competitor = await _context.Competitors.FindAsync(id);

        if (competitor == null)
        {
            return NotFound(new { message = $"Compétiteur avec ID {id} introuvable" });
        }

        _context.Competitors.Remove(competitor);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<IEnumerable<CompetitorDto>>> GetLeaderboard()
    {
        var topCompetitors = await _context.Competitors
            .OrderByDescending(c => c.AccumulatedPoints)
            .Take(10)
            .Select(c => new CompetitorDto
            {
                Id = c.Id,
                Nickname = c.Nickname,
                EmailAddress = c.EmailAddress,
                RankLevel = c.RankLevel,
                AccumulatedPoints = c.AccumulatedPoints,
                EnrollmentDate = c.EnrollmentDate
            })
            .ToListAsync();

        return Ok(topCompetitors);
    }
}