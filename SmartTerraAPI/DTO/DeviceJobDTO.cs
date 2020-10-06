using SmartTerraAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.DTO
{
    public class DeviceJobDTO
    {
        public int Id { get; set; }

        [Required]
        public DateTime? ExecutionTime { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        [DefaultValue(false)]
        public bool Done { get; set; }

        public Device Device { get; set; }

        public Job Job { get; set; }
    }
}
