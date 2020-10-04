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
            base.OnModelCreating(builder);
        }

        public DbSet<SmartTerraAPI.Models.Device> Devices { get; set; }

        public DbSet<SmartTerraAPI.Models.Job> Jobs { get; set; }

        public DbSet<SmartTerraAPI.Models.Mode> Modes { get; set; }

        public DbSet<SmartTerraAPI.Models.User> Users { get; set; }

        public DbSet<SmartTerraAPI.Models.DeviceJob> DeviceJob { get; set; }
    }
}
