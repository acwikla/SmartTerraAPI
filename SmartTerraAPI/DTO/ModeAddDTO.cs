using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.DTO
{
    public class ModeAddDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public double Temperature { get; set; }

        [Required]
        public double Humidity { get; set; }

        public double HeatIndex { get; set; }

        [Required]
        public TimeSpan TwilightHour { get; set; }

        [Required]
        public TimeSpan HourOfDawn { get; set; }
    }
}
