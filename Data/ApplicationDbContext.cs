using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace caseManageMentSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Case> Cases { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<CaseHistory> CaseHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Case>()
                .HasOne(c => c.Client)
                .WithMany(u => u.ClientCases)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Case>()
                .HasOne(c => c.CaseManager)
                .WithMany(u => u.ManagedCases)
                .HasForeignKey(c => c.CaseManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Case>()
                .HasIndex(c => c.CaseNumber)
                .IsUnique();

            builder.Entity<CaseHistory>()
                .HasOne(h => h.User)
                .WithMany()
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CaseHistory>()
                .HasOne(h => h.Case)
                .WithMany(c => c.Histories)
                .HasForeignKey(h => h.CaseId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Note>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Note>()
                .HasOne(n => n.Case)
                .WithMany(c => c.Notes)
                .HasForeignKey(n => n.CaseId);
        }
    }
}
