using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.DTO
{
    public class ModeAddDTO
    {
        [Required]
        public string Name { get; set; }

        [DefaultValue(true)]
        public bool isOn { get; set; }

        [Required]
        public double Temperature { get; set; }

        [Required]
        public double Humidity { get; set; }

        [Required]
        public TimeSpan TwilightHour { get; set; }

        [Required]
        public TimeSpan HourOfDawn { get; set; }
    }
}
