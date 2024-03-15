using Microsoft.AspNetCore.Mvc;
using webapi.Interfaces;
using webapi.Models.Basic;
using webapi.Models.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace webapi.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;

        public UserController(IUnitOfWork uow, IConfiguration configuration)
        {
            this._uow = uow;
            this._configuration = configuration;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _uow.UserRepository.GetUsers();
            return Ok(users);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(LoginReqDto loginReq)
        {
            if (await _uow.UserRepository.UserAlreadyExists(loginReq.Username))
                return BadRequest("User already exists, please try something else");

            _uow.UserRepository.Register(loginReq.Username!, loginReq.Password!);
            return StatusCode(201);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(User loginuser)
        {
            User user = await _uow!.UserRepository!.Authenticate(loginuser!.Username!, loginuser!.Password!);

            if (user == null)
            {
                return Unauthorized();
            }

            var loginRes = new LoginResDto();
            loginRes.Username = user.Username;
            loginRes.Token = CreateJWT(user);

            return Ok(loginRes);
        }

        private string CreateJWT(User user)
        {
            var secretKey = _configuration.GetSection("AppSettings:Key").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

            var claims = new Claim[] {
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!)
            };

            var signingCredentials = new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
