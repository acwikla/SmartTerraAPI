using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.DTO
{
    public class ModeIsOnDTO
    {
        [Required]
        public bool isOn { get; set; }
    }
}
