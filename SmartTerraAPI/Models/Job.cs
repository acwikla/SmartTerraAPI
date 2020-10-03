using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FoolproofWebApi;

namespace SmartTerraAPI.Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Body { get; set; }

        public User User { get; set; }

        public ICollection<DeviceJob> DeviceJobs { get; set; }
    }
}
