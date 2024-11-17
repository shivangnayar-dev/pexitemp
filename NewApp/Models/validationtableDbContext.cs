using System;
using Microsoft.EntityFrameworkCore;

namespace NewApp.Models
{
    public class ValidationtableDbContext : DbContext
    {
        // Corrected the DbSet type to be Validationtable, not ValidationtableDbContext
        public DbSet<Validationtable> Validationtable { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapping the Validationtable entity to the "Validation_table" table in the database
            modelBuilder.Entity<Validationtable>().ToTable("validation_table");

            base.OnModelCreating(modelBuilder);
        }

        // Optionally configure the connection string in this method or in the Startup.cs file
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseMySQL("YourConnectionStringHere");
        // }
    }
}
