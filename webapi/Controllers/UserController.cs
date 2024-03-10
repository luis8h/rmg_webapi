using Microsoft.AspNetCore.Mvc;
using webapi.Interfaces;
using webapi.Models.Basic;

namespace webapi.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUnitOfWork _uow;

        public UserController(IUnitOfWork uow)
        {
            this._uow = uow;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _uow.UserRepository.GetUsers();
            return Ok(users);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(User user)
        {
            var user = await this.uow.UserRepository.Authenticate(user.UserName, user.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }
    }
}
