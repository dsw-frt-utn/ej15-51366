using Dsw2026Ej15.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2026Ej15.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Speciality> Specialities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Doctor>(eb =>
            {
                eb.HasKey(d => d.Id);
                eb.Property(d => d.Name).IsRequired().HasMaxLength(100);
                eb.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(50);
                eb.Property(d => d.IsActive).IsRequired();
                eb.HasOne(d => d.Speciality)
                  .WithMany()
                  .HasForeignKey(d => d.SpecialityId)
                  .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Speciality>(eb =>
            {
                eb.HasKey(s => s.Id);
                eb.Property(s => s.Name).IsRequired().HasMaxLength(100);
                eb.Property(s => s.Description).HasMaxLength(500);
            });
        }
}
}
