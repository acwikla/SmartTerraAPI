using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.DTO
{
    public class DeviceJobAddDTO
    {
        [Required]
        public DateTime? ExecutionTime { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
