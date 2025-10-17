using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacktimApi.Controllers;
using StacktimApi.Data;
using StacktimApi.DTOs;
using StacktimApi.Models;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace StacktimApi.Tests
{
    public class CompetitorsControllerTests
    {
        private StacktimDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<StacktimDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // base temporaire
                .Options;
            return new StacktimDbContext(options);
        }

        private CompetitorsController GetController(StacktimDbContext context)
        {
            return new CompetitorsController(context);
        }

        [Fact]
        public async Task GetAllCompetitors_ReturnsEmptyList_WhenNoCompetitors()
        {
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            var result = await controller.GetAllCompetitors();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var competitors = Assert.IsAssignableFrom<IEnumerable<CompetitorDto>>(okResult.Value);
            Assert.Empty(competitors);
        }

        [Fact]
        public async Task GetCompetitorById_ReturnsNotFound_WhenCompetitorDoesNotExist()
        {
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            var result = await controller.GetCompetitorById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("introuvable", notFound.Value.ToString());
        }

        [Fact]
        public async Task CreateCompetitor_ReturnsCreatedCompetitor()
        {
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            var dto = new CreateCompetitorDto
            {
                Nickname = "JohnDoe",
                EmailAddress = "john@example.com",
                RankLevel = "Bronze"
            };

            var result = await controller.CreateCompetitor(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdCompetitor = Assert.IsType<CompetitorDto>(created.Value);

            Assert.Equal("JohnDoe", createdCompetitor.Nickname);
            Assert.Equal("john@example.com", createdCompetitor.EmailAddress);
            Assert.Equal("Bronze", createdCompetitor.RankLevel);
            Assert.Equal(0, createdCompetitor.AccumulatedPoints);
        }

        [Fact]
        public async Task CreateCompetitor_ReturnsBadRequest_WhenDuplicateNickname()
        {
            var context = GetInMemoryDbContext();
            context.Competitors.Add(new Competitor
            {
                Nickname = "JohnDoe",
                EmailAddress = "john@example.com",
                RankLevel = "Bronze"
            });
            context.SaveChanges();

            var controller = GetController(context);

            var dto = new CreateCompetitorDto
            {
                Nickname = "JohnDoe",
                EmailAddress = "new@example.com",
                RankLevel = "Silver"
            };

            var result = await controller.CreateCompetitor(dto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("pseudo", badRequest.Value.ToString());
        }

        [Fact]
        public async Task UpdateCompetitor_ReturnsNoContent_WhenSuccessful()
        {
            var context = GetInMemoryDbContext();
            var competitor = new Competitor
            {
                Nickname = "Player1",
                EmailAddress = "player1@example.com",
                RankLevel = "Bronze",
                AccumulatedPoints = 100,
                EnrollmentDate = DateTime.Now
            };
            context.Competitors.Add(competitor);
            context.SaveChanges();

            var controller = GetController(context);

            var updateDto = new UpdateCompetitorDto
            {
                RankLevel = "Silver",
                AccumulatedPoints = 150
            };

            var result = await controller.UpdateCompetitor(competitor.Id, updateDto);

            Assert.IsType<NoContentResult>(result);

            var updated = await context.Competitors.FindAsync(competitor.Id);
            Assert.Equal("Silver", updated.RankLevel);
            Assert.Equal(150, updated.AccumulatedPoints);
        }

        [Fact]
        public async Task DeleteCompetitor_RemovesCompetitor()
        {
            var context = GetInMemoryDbContext();
            var competitor = new Competitor
            {
                Nickname = "ToDelete",
                EmailAddress = "delete@example.com",
                RankLevel = "Gold",
                EnrollmentDate = DateTime.Now
            };
            context.Competitors.Add(competitor);
            context.SaveChanges();

            var controller = GetController(context);

            var result = await controller.DeleteCompetitor(competitor.Id);
            Assert.IsType<NoContentResult>(result);

            Assert.Empty(context.Competitors);
        }

        [Fact]
        public async Task GetLeaderboard_ReturnsTop10Ordered()
        {
            var context = GetInMemoryDbContext();

            for (int i = 1; i <= 15; i++)
            {
                context.Competitors.Add(new Competitor
                {
                    Nickname = $"Player{i}",
                    EmailAddress = $"p{i}@mail.com",
                    RankLevel = "Bronze",
                    AccumulatedPoints = i * 10,
                    EnrollmentDate = DateTime.Now
                });
            }
            context.SaveChanges();

            var controller = GetController(context);
            var result = await controller.GetLeaderboard();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var top = Assert.IsAssignableFrom<IEnumerable<CompetitorDto>>(ok.Value).ToList();

            Assert.Equal(10, top.Count);
            Assert.True(top[0].AccumulatedPoints >= top[1].AccumulatedPoints);
        }
    }
}
