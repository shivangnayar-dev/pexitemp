using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class QuestionDbContext : DbContext
    {
        public DbSet<QuestionData> QuestionData { get; set; }

        public QuestionDbContext(DbContextOptions<QuestionDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuestionData>().ToTable("QuestionData");
            modelBuilder.Entity<QuestionData>().HasKey(qd => qd.SerialNumber);

            // Define the relationship between QuestionData and CandidateDetails
       

            base.OnModelCreating(modelBuilder);
        }
    }
}
