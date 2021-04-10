using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTerraAPI.Models;

namespace SmartTerraAPI
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
            /*builder.Entity<DeviceJob>()
            .Property(s => s.CreatedDate)
            .HasDefaultValueSql("GETDATE()");*/      // not supported by MySQL

            builder.Entity<Job>().HasData(
            new Job
                {
                    Id = 1,
                    Name = "TurnOnLED",
                    Type = "LED",
                    Description = "Turn on the LED strip and set color of the LEDs ."
            },
            new Job
                {
                    Id = 2,
                    Name = "TurnOffLED",
                    Type = "LED",
                    Description = "Turn off the LED strip."
                },
            new Job
                {
                    Id = 3,
                    Name = "TurnOnWaterPump",
                    Type = "PUMP",
                    Description = "Turn on the water pump for given period of time."
            }
            );

            base.OnModelCreating(builder);
        }

        public DbSet<SmartTerraAPI.Models.Device> Devices { get; set; }

        public DbSet<SmartTerraAPI.Models.Job> Jobs { get; set; }

        public DbSet<SmartTerraAPI.Models.Mode> Modes { get; set; }

        public DbSet<SmartTerraAPI.Models.User> Users { get; set; }

        public DbSet<SmartTerraAPI.Models.DeviceJob> DeviceJobs { get; set; }

        public DbSet<SmartTerraAPI.Models.DeviceProperties> DeviceProperties { get; set; }
    }
}
