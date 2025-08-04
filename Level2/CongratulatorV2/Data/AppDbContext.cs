using CongratulatorV2.Models;
using Microsoft.EntityFrameworkCore;

namespace CongratulatorV2.Data;

public class AppDbContext : DbContext
{
    public DbSet<Birthday> Birthdays { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Birthday>(entity =>
        {
            entity.HasKey(b => b.Id);
            
            entity.Property(b => b.Id)
                .ValueGeneratedOnAdd();
          
            entity.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(b => b.Date)
                .IsRequired();

            entity.HasIndex(b => b.Date);
            
            entity.HasIndex(b => b.Name);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}