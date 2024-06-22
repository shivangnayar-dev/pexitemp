using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class TemperamentTabletDbContext : DbContext
    {
        public DbSet<TemperamentTable> TemperamentTable { get; set; }

        public TemperamentTabletDbContext(DbContextOptions<TemperamentTabletDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TemperamentTable>().ToTable("TemperamentTable");
            modelBuilder.Entity<TemperamentTable>().HasKey(qd => qd.id);

            // Define the relationship between QuestionData and CandidateDetails


            base.OnModelCreating(modelBuilder);
        }
    }
}
