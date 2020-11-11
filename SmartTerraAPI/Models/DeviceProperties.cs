using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.Models
{
    public class DeviceProperties
    {
        public int Id { get; set; }

        [Required]
        public Device Device { get; set; }

        [Required]
        public int DeviceId { get; set; }

        [Required]
        public bool isWaterLevelSufficient { get; set; }

        [Required]
        [Range(0, 50)]
        public double Temperature { get; set; }

        [Required]
        [Range(0, 100)]
        public double Humidity { get; set; }

        [Required]
        public double HeatIndex { get; set; }

        [Required]
        public string LEDHexColor { get; set; }

        [Required]
        public double LEDBrightness { get; set; }

        //dodac dane o godzinach, albo nie
    }
}
