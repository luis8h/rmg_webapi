using Microsoft.AspNetCore.Mvc;
using webapi.Interfaces;
using webapi.Models.Basic;
using webapi.Models.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Authentication;
using System.Security.Cryptography;
using Mapster;

namespace webapi.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;

        public UserController(IUnitOfWork uow, IConfiguration configuration, ILogger<UserController> logger)
        {
            this._uow = uow;
            this._configuration = configuration;
            this._logger = logger;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _uow.UserRepository.GetUsers();
            var usersDto = users.Adapt<List<UserDto>>();
            return Ok(usersDto);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginReqDto loginReq)
        {
            if (await UserExists(loginReq.Username))
                return BadRequest("User already exists, please try something else");

            Register(loginReq.Username!, loginReq.Password!);
            return StatusCode(201);
        }

        [HttpGet("get/{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await _uow.UserRepository.GetUser(username);
            var userDto = user.Adapt<UserDto>();
            return Ok(userDto);
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(User loginuser)
        {
            User user = await Authenticate(loginuser!.Username!, loginuser!.Password!);

            if (user == null)
                return Unauthorized();

            var loginRes = new LoginResDto();
            loginRes.Username = user.Username;
            loginRes.Token = CreateJWT(user);

            return Ok(loginRes);
        }

        private async Task<bool> UserExists(string username)
        {
            var user = await _uow.UserRepository.GetUser(username);

            if (user != null)
                return true;

            return false;
        }

        private void Register(string username, string password)
        {
            byte[] passwordHash, passwordKey;
            using (var hmac = new HMACSHA256())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            User user = new User();
            user.Username = username;
            user.PasswordHashed = passwordHash;
            user.PasswordKey = passwordKey;

            _uow.UserRepository.addUser(user);
        }

        private async Task<User> Authenticate(string username, string passwordText)
        {
            User? user = await _uow.UserRepository.GetUser(username);

            if (user == null)
                throw new AuthenticationException();

            if (!MatchPasswordHash(passwordText, user?.PasswordHashed, user?.PasswordKey))
                throw new AuthenticationException();

            return user!;
        }

        private bool MatchPasswordHash(string passwordText, byte[] ?passwordHashed = null, byte[] ?passwordKey = null)
        {
            if (passwordHashed == null || passwordKey == null)
                return false;

            using (var hmac = new HMACSHA256(passwordKey))
            {
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordText));

                for (int i = 0; i < passwordHash.Length; i++)
                {
                    if (passwordHash[i] != passwordHashed[i])
                    {
                        return false;
                    }
                }

                return true;
            }
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
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
