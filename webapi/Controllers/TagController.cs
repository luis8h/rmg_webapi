using Microsoft.AspNetCore.Mvc;
using webapi.Interfaces;
using webapi.Models;

namespace webapi.Controllers
{
    public class TagController : BaseController
    {
        private readonly IUnitOfWork _uow;

        public TagController(IUnitOfWork uow)
        {
            this._uow = uow;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            var tag = await _uow.TagRepository.GetTags();
            return Ok(tag);
        }

    }
}
