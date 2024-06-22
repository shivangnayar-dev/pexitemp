using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class bechcomenDbContext : DbContext
    {
        public DbSet<bechcomen> bechcomen { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<bechcomen>().ToTable("bechcomen");
            modelBuilder.Entity<bechcomen>().HasNoKey(); // Configure Benchmarkmodel as keyless entity
            base.OnModelCreating(modelBuilder);

        }
    }
}
