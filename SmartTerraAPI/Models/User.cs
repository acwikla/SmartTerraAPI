﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SmartTerraAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public virtual IEnumerable <Mode> Modes { get; set; }

        public TaskModel Tasks { get; set; }

        [Required]
        [StringLength(30)]
        public string Login { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must have at least 6 characters.")]
        public string Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage ="Invalid email adress.")]
        public string Email { get; set; }
    }
}
