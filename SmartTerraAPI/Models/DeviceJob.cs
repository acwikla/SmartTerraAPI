using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.Models
{
    public class DeviceJob
    {
        public int Id { get; set; }

        [Required]
        public DateTime? ExecutionTime { get; set; }      // If date equal to null -> it means, job should be performed NOW (on the device).

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate{ get; set; }          // Date, when job was added to database

        [DefaultValue(false)]
        public bool Done { get; set; }

        [Required]
        public Device Device { get; set; }

        [Required]
        public Job Job { get; set; }
    }
}