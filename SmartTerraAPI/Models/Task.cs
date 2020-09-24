using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Foolproof;

namespace SmartTerraAPI.Models
{
    public class Task
    {
        public int Id { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public bool ManageLEDStrip { get; set; }

        [RequiredIf("ManageLEDStrip", "true")]
        public string LEDBrightnessColour { get; set; } //hexnumber

        [RequiredIf("ManageLEDStrip", "true")]
        [Range(0, 100)]
        public bool LEDBrightness { get; set; }

        public bool Raining { get; set; }

    }
}
