using System;
using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{

    public class  DatabaseTableDbContext: DbContext
    {
        public DbSet<DatabaseTable> DatabaseTable { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Core>().ToTable("DatabaseTable");
            modelBuilder.Entity<Core>().HasKey(qt => qt.Id);

            // Other configurations if needed

            base.OnModelCreating(modelBuilder);
        }
    }
}

