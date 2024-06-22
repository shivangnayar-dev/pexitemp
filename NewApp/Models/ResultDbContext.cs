using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class ResultDbContext : DbContext
    {
        public DbSet<Result> Result { get; set; }

        public ResultDbContext(DbContextOptions<ResultDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Result>().ToTable("Result");
            modelBuilder.Entity<Result>().HasKey(qd => qd.Id);

            // Define the relationship between QuestionData and CandidateDetails


            base.OnModelCreating(modelBuilder);
        }
    }
}
