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
                    Name = "TurnRight",
                    Type = "ROBOTIC_ARM",
                    Description = "Turn right description."
                },
                new Job
                {
                    Id = 6,
                    Name = "TurnLeft",
                    Type = "ROBOTIC_ARM",
                    Description = "Turn left description."
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
                new {
                    Id = 11,
                    DeviceId = 101,
                    Done = false,
                    JobId = 5,
                    Body = "angle: 10, speed: 2",
                    ExecutionTime = DateTime.Now - TimeSpan.FromDays(5),
                    CreatedDate = DateTime.Now - TimeSpan.FromDays(5)
                },
                new {
                    Id = 12,
                    DeviceId = 101,
                    Done = false,
                    JobId = 6,
                    Body = "angle: 50, speed: 3",
                    ExecutionTime = DateTime.Now - TimeSpan.FromDays(5),
                    CreatedDate = DateTime.Now - TimeSpan.FromDays(5)
                },
                new {
                    Id = 13,
                    DeviceId = 101,
                    Done = false,
                    JobId = 6,
                    Body = "angle: 25, speed: 1",
                    ExecutionTime = DateTime.Now - TimeSpan.FromDays(5),
                    CreatedDate = DateTime.Now - TimeSpan.FromDays(5)
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
