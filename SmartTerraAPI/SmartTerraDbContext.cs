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

            // jobs

            builder.Entity<Job>().HasData(
                new Job {
                        Id = 1,
                        Name = "TurnOnLED",
                        Type = "LED",
                        Description = "Turn on the LED strip and set color of the LEDs ."
                },
                new Job {
                        Id = 2,
                        Name = "TurnOffLED",
                        Type = "LED",
                        Description = "Turn off the LED strip."
                },
                new Job {
                        Id = 3,
                        Name = "TurnOnWaterPump",
                        Type = "PUMP",
                        Description = "Turn on the water pump for given period of time."
                },
                new Job {
                    Id = 4,
                    Name = "Rainbow",
                    Type = "LED",
                    Description = "Turn on rainbow."
                },
                new Job
                {
                    Id = 5,
                    Name = "Turn on right",
                    Type = "Robotic arm",
                    Description = "Turn on right description."
                }
            );

            // users

            var userOla = new User
            {
                Id = 1,
                Email = "ola@email.com",
                Login = "ola",
                Password = "pass1"
            };

            var userROBOLab = new User
            {
                Id = 2,
                Email = "robolab@email.com",
                Login = "robolab",
                Password = "pass1"
            };

            builder.Entity<User>().HasData(
                userOla,
                userROBOLab
            );

            // device

            builder.Entity<Device>().HasData(
                new {
                    Id = 101,
                    Name = "ROBOLab test device 1",
                    UserId = userROBOLab.Id
                }
            );

            // device job

            builder.Entity<DeviceJob>().HasData(
                new
                {
                    Id = 11,
                    DeviceId = 101,
                    Done = false,
                    JobId = 5,
                    Body = "{angle = 10}",
                    ExecutionTime = DateTime.Now,
                    CreatedDate = DateTime.Now
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
