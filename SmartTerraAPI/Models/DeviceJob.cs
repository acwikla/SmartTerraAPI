using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.Models
{
    public class DeviceJob
    {
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }
    }
}