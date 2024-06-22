using System;
using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class AcademicStreamDbContext : DbContext
    {
        public DbSet<AcademicStream> AcademicStream { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AcademicStream>().ToTable("AcademicStream");
            modelBuilder.Entity<AcademicStream>().HasNoKey(); // Configure Benchmarkmodel as keyless entity
            base.OnModelCreating(modelBuilder);
        }
    }
}

