using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class IIAIndustriessubDbContext : DbContext
    {
        public DbSet<IIAIndustriessub> IIAIndustriessub { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IIAIndustriessub>().ToTable("IIAIndustriessub");
            modelBuilder.Entity<IIAIndustriessub>().HasNoKey(); // Configure Benchmarkmodel as keyless entity
            base.OnModelCreating(modelBuilder);

        }
    }
}
