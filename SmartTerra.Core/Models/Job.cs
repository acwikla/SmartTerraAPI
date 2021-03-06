using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartTerraAPI.Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string DeviceType { get; set; }

        [Required]
        public string Type { get; set; }                    // Type of the job (LED, PUMP, ...
        
        public string Description { get; set; }

        [Required]
        public string Properties { get; set; }

        public ICollection<DeviceJob> DeviceJobs { get; set; }
    }
}
