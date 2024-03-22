using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class BenchmarkDbContext : DbContext
    {
        public DbSet<Benchmarkmodel> Benchmarkmodel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Benchmarkmodel>().ToTable("benchmarkkkkkkk");
            modelBuilder.Entity<Benchmarkmodel>().HasNoKey(); // Configure Benchmarkmodel as keyless entity
            base.OnModelCreating(modelBuilder);
          
        }
    }
}
