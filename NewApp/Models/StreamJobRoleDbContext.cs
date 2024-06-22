using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class StreamJobRoleDbContext : DbContext
    {
        public DbSet<StreamJobRole> StreamJobRole { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<bechcomen>().ToTable("streamjobrole");
            modelBuilder.Entity<bechcomen>().HasNoKey(); // Configure Benchmarkmodel as keyless entity
            base.OnModelCreating(modelBuilder);

        }
    }
}
