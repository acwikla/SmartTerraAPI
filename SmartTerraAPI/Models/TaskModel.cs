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

        //[RequiredIf("ManageLEDStrip", Operator.EqualTo, true, ErrorMessage = "Please enter LED color.")]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "LED color must be hexadecimal color code.")]
        public string LEDColor { get; set; } //hexnumber

        //[RequiredIf("ManageLEDStrip", Operator.EqualTo, true, ErrorMessage = "Please enter LED brightness.")]
        [Range(0, 100)]
        public double LEDBrightness { get; set; }  

        public bool Raining { get; set; }

    }
}
