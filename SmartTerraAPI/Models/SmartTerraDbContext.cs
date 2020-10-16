using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTerraAPI.Models;

namespace SmartTerraAPI.Models
{
    public class SmartTerraDbContext : DbContext
    {
        public SmartTerraDbContext()
        {

        }

        public SmartTerraDbContext(DbContextOptions options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DeviceJob>()
            .Property(s => s.CreatedDate)
            .HasDefaultValueSql("GETDATE()");

            builder.Entity<Job>().HasData(
            new Job
                {
                    Id = 1,
                    Name = "ManageLedColor",
                    Type = "LED",
                    Description = "Change LED color."
                },
            new Job
                {
                    Id = 2,
                    Name = "ManageLedBrightness",
                    Type = "LED",
                    Description = "Change LED brightness."
                },
            new Job
                {
                    Id = 3,
                    Name = "Pump",
                    Type = "PUMP",
                    Description = "Turn on the water pump."
                }
            );

            base.OnModelCreating(builder);
        }

        public DbSet<SmartTerraAPI.Models.Device> Devices { get; set; }

        public DbSet<SmartTerraAPI.Models.Job> Jobs { get; set; }

        public DbSet<SmartTerraAPI.Models.Mode> Modes { get; set; }

        public DbSet<SmartTerraAPI.Models.User> Users { get; set; }

        public DbSet<SmartTerraAPI.Models.DeviceJob> DeviceJobs { get; set; }
    }
}
