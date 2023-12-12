using Microsoft.AspNetCore.Mvc;
using webapi.Interfaces;
using webapi.Models;

namespace webapi.Controllers
{
    public class RecipeController : BaseController
    {
        private readonly IUnitOfWork _uow;

        public RecipeController(IUnitOfWork uow)
        {
            this._uow = uow;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
        {
            var recipe = await _uow.RecipeRepository.GetRecipes();
            return Ok(recipe);
        }
    }
}
