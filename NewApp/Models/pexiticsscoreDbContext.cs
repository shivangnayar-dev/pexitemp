

using System;
using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class pexiticsscoresDbContext : DbContext
    {
        public DbSet<pexiticsscore> pexiticsscore { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<pexiticsscore>().ToTable("pexiticsscore");
            modelBuilder.Entity<pexiticsscore>().HasKey(tm => tm.id);

            base.OnModelCreating(modelBuilder);
        }
    }
}

