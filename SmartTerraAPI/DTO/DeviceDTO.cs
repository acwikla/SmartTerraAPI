using SmartTerraAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.DTO
{
    public class DeviceDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        //TODO: change Mode type to ModeDTO type(?)
        public Mode Mode { get; set; }
    }
}
