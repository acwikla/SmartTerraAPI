using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartTerra.Core.DTO
{
    public class UserToLogInDTO
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
    }
}