using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartTerraAPI.Models
{
    public class Mode
    {
        [Required]
        [StringLength(60, MinimumLength = 1)]
        public string Title { get; set; }
        
        public int Id { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [Required]
        [Range(0, 100)]
        public double Temperature { get; set; }

        [Required]
        [Range(0, 100)]
        public double Humidity { get; set; }

        public double HeatIndex { get; set; }

        [Required] 
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid twilight hour.")]
        //[DataType(DataType.Time)]
        public string TwilightHour { get; set; }

        [Required]
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid hour of drawn.")]
        //[DataType(DataType.Time)]
        public string HourOfDawn { get; set; }
    }
}
