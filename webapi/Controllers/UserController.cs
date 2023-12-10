using Microsoft.AspNetCore.Mvc;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ContextDb _context;

        public UserController()
        {
            _context = new ContextDb(); // Assuming ContextDb has been properly configured
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            List<User> users = _context.GetUsers();
            return Ok(users);
        }
    }
}
