using JWTTokenImplementation.Context;
using JWTTokenImplementation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace JWTTokenImplementation.Controllers
{
    [Route("JWTTokenContoller")]
    [ApiController]
    public class JWTTokenContoller : ControllerBase
    {
        public IConfiguration _configuration;
        public readonly ApplicationDBContext _dBContext;

        public JWTTokenContoller(IConfiguration configuration, ApplicationDBContext dBContext)
        {
            _configuration = configuration;
            _dBContext = dBContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Users users)
        {
            if(users.Password != null && users.UserName != null)
            {
                var userData = await GetUser(users.UserName, users.Password);
                var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

                if(userData !=null)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", users.Id.ToString()),
                        new Claim("UserName", Encoding.UTF8.GetBytes(users.UserName).ToString()),
                        new Claim("Password", Encoding.UTF8.GetBytes(users.Password).ToString())
                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.key));
                    var signin = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                            jwt.Issuer,
                            jwt.Audience,
                            claims,
                            expires: DateTime.Now.AddMinutes(20),
                            signingCredentials: signin
                        );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            else
            {
                return BadRequest("Invalid Credentials");
            }
            return BadRequest("Invalid Credentials");
        }

        private async Task<Users> GetUser(string userName, string password)
        {
            return await _dBContext.Users.FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);
        }
    }
}
