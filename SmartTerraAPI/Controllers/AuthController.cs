using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartTerraAPI.Models;

namespace SmartTerraAPI.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private SmartTerraDBContext _context;

        public AuthController(SmartTerraDBContext context)
        {
            _context = context;
        }

        [HttpPost("token")]
        public async Task<ActionResult> GetToken()
        {
            var header = Request.Headers["Authorization"];
            if (header.ToString().StartsWith("Basic"))
            {
                var credValue = header.ToString().Substring("Basic ".Length).Trim();
                var userNameAndPasswordd = Encoding.UTF8.GetString(Convert.FromBase64String(credValue));
                var userNameAndPassword = userNameAndPasswordd.Split(":");
                var user = await _context.Users.Include(n => n.Modes).Where(user => user.Login == userNameAndPassword[0] || user.Email == userNameAndPassword[0]).FirstOrDefaultAsync();

                if (user != null && userNameAndPassword[1] == user.Password)
                {
                    var claimData = new[] { new Claim(ClaimTypes.Name, userNameAndPassword[0]) };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dxfcdgasdfghjkljhugtfdsghjkuyjtrgedfsxcvbnmjhkhuyjthcvbnhjgfcbvbhnyj"));
                    var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                    var token = new JwtSecurityToken(
                            issuer: "mysite.com",
                            audience: "mysite.com",
                            expires: DateTime.Now.AddMinutes(1),
                            claims: claimData,
                            signingCredentials: signInCred
                        );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(tokenString);
                }
                else
                {
                    return BadRequest("Wrong email or password.");
                 }
            }
            return BadRequest("Wrong request.");
        }
    }
}
