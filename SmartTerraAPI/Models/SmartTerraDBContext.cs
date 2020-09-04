using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartTerraAPI.Models
{
    public class SmartTerraDBContext : DbContext
    {
        public SmartTerraDBContext(DbContextOptions<SmartTerraDBContext> options)
            : base(options)
        {
        }

        public DbSet<SmartTerraAPI.Models.Mode> Modes { get; set; }
        public DbSet<SmartTerraAPI.Models.User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(c => c.Modes)
                .WithOne(e => e.User);//.HasForeignKey(c => c.Id);

            modelBuilder.Entity<User>().HasKey(c => c.Id);
            modelBuilder.Entity<Mode>().HasKey(c => c.Id);
        }
    }
}
