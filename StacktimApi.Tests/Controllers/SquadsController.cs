using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacktimApi.Controllers;
using StacktimApi.Data;
using StacktimApi.DTOs;
using StacktimApi.Models;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StacktimApi.Tests
{
    public class SquadsControllerTests
    {
        private StacktimDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<StacktimDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new StacktimDbContext(options);
        }

        private SquadsController GetController(StacktimDbContext context)
        {
            return new SquadsController(context);
        }

        [Fact]
        public async Task GetAllSquads_ReturnsEmptyList_WhenNoSquadsExist()
        {
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            var result = await controller.GetAllSquads();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var squads = Assert.IsAssignableFrom<IEnumerable<SquadDto>>(okResult.Value);
            Assert.Empty(squads);
        }

        [Fact]
        public async Task GetSquadById_ReturnsNotFound_WhenSquadDoesNotExist()
        {
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            var result = await controller.GetSquadById(99);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("introuvable", notFound.Value.ToString());
        }

        [Fact]
        public async Task CreateSquad_ReturnsCreated_WhenValid()
        {
            var context = GetInMemoryDbContext();
            var leader = new Competitor
            {
                Nickname = "Leader1",
                EmailAddress = "leader1@mail.com",
                RankLevel = "Gold"
            };
            context.Competitors.Add(leader);
            await context.SaveChangesAsync();

            var controller = GetController(context);

            var dto = new CreateSquadDto
            {
                SquadName = "TeamAlpha",
                Abbreviation = "TA",
                LeaderId = leader.Id
            };

            var result = await controller.CreateSquad(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var squadDto = Assert.IsType<SquadDto>(created.Value);

            Assert.Equal("TeamAlpha", squadDto.SquadName);
            Assert.Equal("TA", squadDto.Abbreviation);
            Assert.Equal(leader.Id, squadDto.LeaderId);

            // Le leader doit être ajouté comme membre
            var membership = await context.SquadMembers.FirstOrDefaultAsync(m => m.SquadId == squadDto.Id && m.CompetitorId == leader.Id);
            Assert.NotNull(membership);
            Assert.Equal(0, membership.Position); // leader
        }

        [Fact]
        public async Task CreateSquad_ReturnsBadRequest_WhenNameAlreadyExists()
        {
            var context = GetInMemoryDbContext();
            var leader = new Competitor { Nickname = "Leader", EmailAddress = "lead@mail.com" };
            context.Competitors.Add(leader);
            context.Squads.Add(new Squad { SquadName = "TeamX", Abbreviation = "TX", LeaderId = leader.Id });
            context.SaveChanges();

            var controller = GetController(context);

            var dto = new CreateSquadDto
            {
                SquadName = "TeamX",
                Abbreviation = "ZZ",
                LeaderId = leader.Id
            };

            var result = await controller.CreateSquad(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("déjà utilisé", badRequest.Value.ToString());
        }

        [Fact]
        public async Task CreateSquad_ReturnsBadRequest_WhenLeaderDoesNotExist()
        {
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            var dto = new CreateSquadDto
            {
                SquadName = "GhostSquad",
                Abbreviation = "GS",
                LeaderId = 999 
            };

            var result = await controller.CreateSquad(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("n'existe pas", badRequest.Value.ToString());
        }

        [Fact]
        public async Task UpdateSquad_ReturnsNoContent_WhenSuccessful()
        {
            var context = GetInMemoryDbContext();
            var leader = new Competitor { Nickname = "Leader", EmailAddress = "leader@mail.com" };
            var newLeader = new Competitor { Nickname = "NewLeader", EmailAddress = "newleader@mail.com" };
            context.Competitors.AddRange(leader, newLeader);
            await context.SaveChangesAsync();

            var squad = new Squad
            {
                SquadName = "OldSquad",
                Abbreviation = "OS",
                LeaderId = leader.Id
            };
            context.Squads.Add(squad);
            await context.SaveChangesAsync();

            var controller = GetController(context);

            var updateDto = new UpdateSquadDto
            {
                SquadName = "NewSquad",
                Abbreviation = "NS",
                LeaderId = newLeader.Id
            };

            var result = await controller.UpdateSquad(squad.Id, updateDto);

            Assert.IsType<NoContentResult>(result);

            var updated = await context.Squads.FindAsync(squad.Id);
            Assert.Equal("NewSquad", updated.SquadName);
            Assert.Equal("NS", updated.Abbreviation);
            Assert.Equal(newLeader.Id, updated.LeaderId);
        }

        [Fact]
        public async Task UpdateSquad_ReturnsBadRequest_WhenNewLeaderDoesNotExist()
        {
            var context = GetInMemoryDbContext();
            var leader = new Competitor { Nickname = "Leader", EmailAddress = "lead@mail.com" };
            context.Competitors.Add(leader);
            var squad = new Squad
            {
                SquadName = "MySquad",
                Abbreviation = "MS",
                LeaderId = leader.Id
            };
            context.Squads.Add(squad);
            context.SaveChanges();

            var controller = GetController(context);

            var dto = new UpdateSquadDto
            {
                SquadName = "MySquad",
                Abbreviation = "MS",
                LeaderId = 999 
            };

            var result = await controller.UpdateSquad(squad.Id, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("n'existe pas", badRequest.Value.ToString());
        }

        [Fact]
        public async Task DeleteSquad_RemovesSquadAndMembers()
        {
            var context = GetInMemoryDbContext();
            var leader = new Competitor { Nickname = "Leader", EmailAddress = "lead@mail.com" };
            context.Competitors.Add(leader);
            await context.SaveChangesAsync();

            var squad = new Squad { SquadName = "SquadA", Abbreviation = "SA", LeaderId = leader.Id };
            context.Squads.Add(squad);
            await context.SaveChangesAsync();

            var member = new SquadMember
            {
                SquadId = squad.Id,
                CompetitorId = leader.Id,
                Position = 0
            };
            context.SquadMembers.Add(member);
            await context.SaveChangesAsync();

            var controller = GetController(context);

            var result = await controller.DeleteSquad(squad.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Squads);
            Assert.Empty(context.SquadMembers);
        }
    }
}
