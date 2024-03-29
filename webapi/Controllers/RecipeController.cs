using Mapster;
using Microsoft.AspNetCore.Mvc;
using webapi.Interfaces;
using webapi.Models.Basic;
using webapi.Models.Dtos;
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
        public async Task<ActionResult<IEnumerable<DetailRecipe>>> GetRecipesDetail()
        {
            var recipe = await _uow.RecipeRepository.GetRecipesDetail();
            return Ok(recipe);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRecipe(Recipe recipe)
        {
            recipe.CreatedBy = 1;
            int recipeId = await _uow.RecipeRepository.AddRecipe(recipe);
            if (recipeId == -1) return BadRequest();
            return Ok(new { Id = recipeId });
        }

        [HttpPut("{recipeId}/update")]
        public async Task<IActionResult> UpdateRecipe(int recipeId, PutRecipeDto newRecipe)
        {
            var existingRecipe = await _uow.RecipeRepository.GetRecipeById(recipeId);

            if (existingRecipe == null)
                return NotFound();

            newRecipe.Adapt(existingRecipe);
            int error = await _uow.RecipeRepository.PutRecipe(existingRecipe);

            return NoContent();
        }

        [HttpGet("{recipeId}/data")]
        public async Task<IActionResult> GetRecipeById(int recipeId)
        {
            var recipe = await _uow.RecipeRepository.GetRecipeById(recipeId);

            if (recipe == null)
                return NotFound(); // Recipe with the given ID not found

            return Ok(recipe);
        }

        [Route("{recipeId}/upload-image")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> UploadAsync(int recipeId)
        {
            var formCollection = await Request.ReadFormAsync();
            var file = formCollection.Files.First();

            // TODO: bring path outside of sourcecode directory
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Images", "Recipes");

            if (file.Length > 0)
            {
                var fileName = "main_" + recipeId + Path.GetExtension(file.FileName);
                var fullPath = Path.Combine(pathToSave, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                    await file.CopyToAsync(stream);

                var fileInfo = new FileInfo(fullPath);
                return Ok(new { Size = fileInfo.Length });
            }
            else
                return BadRequest();
        }

        [HttpGet("{recipeId}/image")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // for instant refresh after uploading new image
        public IActionResult GetImage(int recipeId)
        {
            // TODO: make file extension dynamic

            var folderName = Path.Combine("Resources", "Images", "Recipes");
            var workingDir = Directory.GetCurrentDirectory();
            var imagePath = Path.Combine(workingDir, folderName, "main_" + recipeId + ".png"); // Assuming JPG extension

            if (!System.IO.File.Exists(imagePath))
                imagePath = Path.Combine(workingDir, folderName, "main_" + recipeId + ".jpg");

            if (!System.IO.File.Exists(imagePath))
                imagePath  = Path.Combine("Resources", "Images", "Settings", "default_recipe_main_img.png");

            if (!System.IO.File.Exists(imagePath))
                return NotFound(); // Return 404 Not Found if the image doesn't exist

            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(imageBytes, "image/jpeg"); // Return the image as a FileResult
        }
    }
}
