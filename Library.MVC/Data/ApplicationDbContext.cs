using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Premises> Premises { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<FollowUp> FollowUps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Enums como texto (mais legível no banco)
            builder.Entity<Premises>()
                .Property(p => p.RiskRating)
                .HasConversion<string>();

            builder.Entity<Inspection>()
                .Property(i => i.Outcome)
                .HasConversion<string>();

            builder.Entity<FollowUp>()
                .Property(f => f.Status)
                .HasConversion<string>();
        }
    }
}