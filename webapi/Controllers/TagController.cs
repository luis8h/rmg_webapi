using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Interfaces;
using webapi.Models.Basic;
using webapi.Models.Dtos;

namespace webapi.Controllers
{
    [Authorize]
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

        [HttpPost("add")]
        public async Task<IActionResult> AddRecipe(AddTagDto tagDto)
        {
            int tagId = await _uow.TagRepository.AddTag(tagDto);
            if (tagId == -1) return BadRequest();
            return Ok(new { Id = tagId });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var affectedRows = await _uow.TagRepository.DeleteTag(id);
            return Ok( new { RowsDeleted = affectedRows } );
        }

    }
}
