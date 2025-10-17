using Microsoft.EntityFrameworkCore;
using StacktimApi.Models;

namespace StacktimApi.Data;

public class StacktimDbContext : DbContext
{
    public StacktimDbContext(DbContextOptions<StacktimDbContext> options) : base(options) { }

    public DbSet<Competitor> Competitors { get; set; }
    public DbSet<Squad> Squads { get; set; }
    public DbSet<SquadMember> SquadMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Competitor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Nickname).IsUnique();
            entity.HasIndex(e => e.EmailAddress).IsUnique();
            entity.Property(e => e.AccumulatedPoints).HasDefaultValue(0);
            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Squad>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Abbreviation).IsUnique();
            entity.Property(e => e.FoundationDate).HasDefaultValueSql("GETDATE()");
            
            entity.HasOne(e => e.Leader)
                .WithMany(c => c.LeadingSquads)
                .HasForeignKey(e => e.LeaderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SquadMember>(entity =>
        {
            entity.HasKey(e => new { e.SquadId, e.CompetitorId });
            
            entity.HasOne(e => e.Squad)
                .WithMany(s => s.Members)
                .HasForeignKey(e => e.SquadId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Competitor)
                .WithMany(c => c.Memberships)
                .HasForeignKey(e => e.CompetitorId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.Property(e => e.MembershipDate).HasDefaultValueSql("GETDATE()");
        });

        base.OnModelCreating(modelBuilder);
    }
}