using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using tutorials_datinApp.Api.Data;
using tutorials_datinApp.Api.Dtos;
using tutorials_datinApp.Api.Models;

namespace tutorials_datinApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repos;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repos, IConfiguration config)
        {
            _config = config;
            _repos = repos;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisteredUserDto newRegisteredUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (newRegisteredUser == null)
            {
                return BadRequest();
            }

            newRegisteredUser.Username = newRegisteredUser.Username.ToLower();

            if (await _repos.UserExists(newRegisteredUser.Username))
            {
                return BadRequest("User already exists");
            }

            var newUser = new User()
            {
                Username = newRegisteredUser.Username
            };

            var createdUser = await _repos.Register(newUser, newRegisteredUser.Password);
            return StatusCode(201);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoggedUserDto newLoggedUser)
        {
            var userInDb = await _repos.Login(newLoggedUser.Username.ToLower(), newLoggedUser.Password);

            if (userInDb == null)
            {
                return Unauthorized();
            }

            // If user is valid starts to build the JWT (Json Web Token)
            // Define claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userInDb.Id.ToString()),
                new Claim(ClaimTypes.Name, userInDb.Username)
            };

            // Get private Key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            // Encrypt private key
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}