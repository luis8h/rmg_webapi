using Microsoft.AspNetCore.Mvc;
using webapi.Interfaces;
using webapi.Models.Basic;
using webapi.Models.Extended;

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
        public async Task<ActionResult<IEnumerable<DetailRecipe>>> GetRecipes()
        {
            var recipe = await _uow.RecipeRepository.GetRecipes();
            return Ok(recipe);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRecipe(Recipe recipe)
        {
            await _uow.RecipeRepository.AddRecipe(recipe);
            // await uow.SaveAsync();
            return StatusCode(201);
        }
    }
}
