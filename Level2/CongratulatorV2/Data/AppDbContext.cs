using CongratulatorV2.Models;
using Microsoft.EntityFrameworkCore;

namespace CongratulatorV2.Data;

public class AppDbContext : DbContext
{
    public DbSet<Birthday> Birthdays { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Birthday>()
            .HasKey(b => b.Id);

        model.Entity<Birthday>()
            .Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        model.Entity<Birthday>()
            .HasIndex(b => b.Date);
    }
}