using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTerraAPI.Models;

namespace SmartTerraAPI.Models
{
    public class SmartAPIDbContext : DbContext
    {
        public SmartAPIDbContext()
        {

        }

        public SmartAPIDbContext(DbContextOptions options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<DeviceJob>().HasKey(i => new { i.DeviceId, i.JobId });
        }
    }
}
