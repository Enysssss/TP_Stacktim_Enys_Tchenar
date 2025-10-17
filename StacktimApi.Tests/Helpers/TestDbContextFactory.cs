using Microsoft.EntityFrameworkCore;
using StacktimApi.Data;

namespace StacktimApi.Tests
{
    public static class TestDbContextFactory
    {
        public static StacktimDbContext Create()
        {
            var options = new DbContextOptionsBuilder<StacktimDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // base unique pour chaque test
                .Options;

            var context = new StacktimDbContext(options);

            SeedData(context);

            return context;
        }

        private static void SeedData(StacktimDbContext context)
        {
            var competitor = new StacktimApi.Models.Competitor
            {
                Nickname = "TestUser",
                EmailAddress = "test@example.com"
            };

            context.Competitors.Add(competitor);
            context.SaveChanges();
        }

        public static void Destroy(StacktimDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
