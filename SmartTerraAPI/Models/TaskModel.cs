using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace SmartTerraAPI.Models
{
    public class TaskModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public bool ManageLEDStrip { get; set; }

        //[RequiredIf("ManageLEDStrip", "true")]
        public string LEDColor { get; set; } //hexnumber

        //[RequiredIf("ManageLEDStrip", "true")]
        [Range(0, 100)]
        public double LEDBrightness { get; set; }  

        public bool Raining { get; set; }

    }
}
