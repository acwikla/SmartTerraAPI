using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace SmartTerraAPI.Models
{
    public class Mode
    {
        public int Id { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 1)]
        public string Name { get; set; }

        [DefaultValue(true)]
        public bool IsOn { get; set; }

        [Required]
        [Range(0, 50)]
        public double Temperature { get; set; }

        [Required]
        [Range(0, 100)]         //TODO: check minimum and maximum humidity value
        public double Humidity { get; set; }

        [Required]
        public TimeSpan TwilightHour { get; set; }

        [Required]
        public TimeSpan HourOfDawn { get; set; }

        [Required]
        public Device Device { get; set; }
        [Required]
        public int DeviceId { get; set; }
    }
}
