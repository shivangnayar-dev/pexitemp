using System;
using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class AssessmentResultsDbContext : DbContext
    {
        public DbSet<AssessmentResults> AssessmentResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AssessmentResults>().ToTable("AssessmentResults");
            modelBuilder.Entity<AssessmentResults>().HasKey(qd => qd.Id);




            base.OnModelCreating(modelBuilder);
        }
    }
}

