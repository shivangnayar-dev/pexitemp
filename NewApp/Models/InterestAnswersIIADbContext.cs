using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class InterestAnswersIIADbContext : DbContext
    {
        public DbSet<InterestAnswersIIA> InterestAnswersIIA { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InterestAnswersIIA>().ToTable("InterestAnswersIIA");
            modelBuilder.Entity<InterestAnswersIIA>().HasNoKey(); // Configure Benchmarkmodel as keyless entity
            base.OnModelCreating(modelBuilder);

        }
    }
}
