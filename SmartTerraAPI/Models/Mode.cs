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
        //[RegularExpression(@"^\d+$")] //^[0-9]*$
        public double Temperature { get; set; }

        [Required]
        [Range(0, 100)]
        public double Humidity { get; set; }

        public double HeatIndex { get; set; }

        [Required]
        [Range(0, 100)]
        public double Brightness { get; set; }
    }
}
