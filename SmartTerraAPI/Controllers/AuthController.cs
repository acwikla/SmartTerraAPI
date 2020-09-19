using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SmartTerraAPI.Models;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace SmartTerraAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private SmartTerraDBContext _context;
        private IConfiguration _config;
        public AuthController(SmartTerraDBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("login")]
        public IActionResult Login (string login, string password)
        {
            User _login = new User();
            _login.Login = login;
            _login.Password = password;

            IActionResult response = Unauthorized();

            var user = AuthenticateUser(_login);
            if(user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }
            return response;
        }

        private User AuthenticateUser(User _login)
        {
            User userToLogin = null;
            //var user = await _context.Users.Include(n => n.Modes).Where(user => user.Login == _login.Login || user.Email == _login.Email).FirstOrDefaultAsync();

            if (_login.Login== "user1" &&_login.Password == "bubu")
            {
                userToLogin = new User() ;
                userToLogin.Login = _login.Login;
                userToLogin.Password = _login.Password;
            }
            return userToLogin;
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim (JwtRegisteredClaimNames.Sub, userInfo.Login ),
                new Claim (JwtRegisteredClaimNames.Email, userInfo.Password ),
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
                );
            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodeToken;
        }

        [Authorize]
        [HttpPost("post")]
        public string Post()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var userName = claim[0].Value;
            return "Welcome: " + userName;
        }
    }
}
