using Microsoft.EntityFrameworkCore;
using OK.Siemens.Models;

namespace OK.Siemens.DataProviders;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions options): base(options)
    { }

    public virtual DbSet<DataRecord> DataRecords { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataRecord>().Property(f => f.Id).ValueGeneratedOnAdd();
        //modelBuilder.Entity<DataRecord>().HasKey(table => new {table.Id, table.TimeStamp});
    }
}