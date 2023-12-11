using Microsoft.AspNetCore.Mvc;
using webapi.Interfaces;
using webapi.Models;

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
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            List<User> users = _uow.UserRepository.GetUsers();
            return Ok(users);
        }
    }
}
