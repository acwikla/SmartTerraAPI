using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.Models
{
    public class DeviceJob
    {
        public int Id { get; set; }

        [Required]
        public DateTime ExecutionTime { get; set; }      // If date equal to null -> it means, job should be performed NOW (on the device).

        public DateTime Created{ get; set; }             // Date, when job was added to database

        [Required]
        public Device Device { get; set; }

        [Required]
        public Job Job { get; set; }
    }
}