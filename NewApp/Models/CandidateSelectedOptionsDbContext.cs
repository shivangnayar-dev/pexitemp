using System;
using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class CandidateSelectedOptionsDbContext : DbContext
    {
        public DbSet<CandidateSelectedOptions> CandidateSelectedOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CandidateSelectedOptions>().ToTable("CandidateSelectedOptions");
            modelBuilder.Entity<CandidateSelectedOptions>().HasKey(tm => tm.selected_option);

            base.OnModelCreating(modelBuilder);
        }
    }
}

