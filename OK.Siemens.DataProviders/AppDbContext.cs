using Microsoft.EntityFrameworkCore;
using OK.Siemens.Models;

namespace OK.Siemens.DataProviders;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions options): base(options)
    { }

    public virtual DbSet<DataRecord> DataRecords { get; set; } = null!;
    public virtual DbSet<PlcTag> Tags { get; set; } = null!;
    public virtual DbSet<Category> Categories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataRecord>().Property(f => f.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<DataRecord>().HasKey(t => new {t.Id, t.TimeStamp});
        modelBuilder.Entity<PlcTag>().Property(f => f.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Category>().Property(f => f.Id).ValueGeneratedOnAdd();
    }
}