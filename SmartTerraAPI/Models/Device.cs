using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.Models
{
    public class Device
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public User User { get; set; }

        public Mode Mode { get; set; }

        public ICollection<DeviceJob> DeviceJobs { get; set; }
    }
}
