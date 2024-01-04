using System.Net.Http.Headers;
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

        [Route("{recipeId}/upload-image")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> UploadAsync(int recipeId)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "Images", "Recipes");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = "main_" + recipeId;
                    var fullPath = Path.Combine(pathToSave, "main_" + recipeId);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
