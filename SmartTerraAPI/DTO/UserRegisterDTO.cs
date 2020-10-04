using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.DTO
{
    public class UserRegisterDTO
    {
        [Required]
        [StringLength(30)]
        public string Login { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must have at least 6 characters.")]
        //TODO:hasło powinno byc zaszyfrowane (do zrobienia na później)
        public string Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email adress.")]
        public string Email { get; set; }
    }
}
