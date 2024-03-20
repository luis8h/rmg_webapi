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

        // [HttpGet("list")]
        // public async Task<ActionResult<IEnumerable<DetailRecipe>>> GetRecipes()
        // {
        //     var recipe = await _uow.RecipeRepository.GetRecipes();
        //     return Ok(recipe);
        // }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
        {
            var recipe = await _uow.RecipeRepository.GetRecipes();
            return Ok(recipe);
        }

        [HttpGet("detail-list")]
        public async Task<ActionResult<IEnumerable<DetailRecipe>>> GetRecipesDetail()
        {
            var recipe = await _uow.RecipeRepository.GetRecipesDetail();
            return Ok(recipe);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRecipe(Recipe recipe)
        {
            int recipeId = await _uow.RecipeRepository.AddRecipe(recipe);

            if (recipeId == -1) return BadRequest();

            return Ok(new { Id = recipeId });
        }

        [HttpPut("{recipeId}/update")]
        public async Task<IActionResult> UpdateRecipe(int recipeId, Recipe updatedRecipe)
        {
            var existingRecipe = await _uow.RecipeRepository.GetRecipeById(recipeId);

            if (existingRecipe == null)
            {
                return NotFound(); // Recipe with the given ID not found
            }

            // Update the properties of the existing recipe
            existingRecipe.Name = updatedRecipe.Name;
            existingRecipe.Description = updatedRecipe.Description;
            existingRecipe.Ratings = updatedRecipe.Ratings;
            existingRecipe.Tags = updatedRecipe.Tags;
            existingRecipe.Preptime = updatedRecipe.Preptime;
            existingRecipe.Cooktime = updatedRecipe.Cooktime;
            existingRecipe.Worktime = updatedRecipe.Worktime;
            // Update other properties as needed

            // Perform the update in the repository
            int error = await _uow.RecipeRepository.PutRecipe(existingRecipe);

            if (error == -1)
            {
                return BadRequest(); // Update failed
            }

            return NoContent(); // Successful update, no content to return
        }

        [HttpGet("{recipeId}/data")]
        public async Task<IActionResult> GetRecipeById(int recipeId)
        {
            var recipe = await _uow.RecipeRepository.GetRecipeByIdV2(recipeId);

            if (recipe == null)
            {
                return NotFound(); // Recipe with the given ID not found
            }

            return Ok(recipe);
        }

        [Route("{recipeId}/upload-image")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> UploadAsync(int recipeId)
        {
            Console.WriteLine("Uploading image ...");

            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "Images", "Recipes");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = "main_" + recipeId + fileExtension;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        // file.CopyTo(stream);
                        await file.CopyToAsync(stream);
                    }
                    // return Ok(new { dbPath });
                    var fileInfo = new FileInfo(fullPath);

                    // Return file size and path
                    return Ok(new { Size = fileInfo.Length, dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("{recipeId}/image")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // for instant refresh after uploading new image
        public IActionResult GetImage(int recipeId)
        {
            Console.WriteLine("Serving Image ...");

            var folderName = Path.Combine("Resources", "Images", "Recipes");
            var workingDir = Directory.GetCurrentDirectory();
            var imagePath = Path.Combine(workingDir, folderName, "main_" + recipeId + ".png"); // Assuming JPG extension

            Console.WriteLine(imagePath);

            if (!System.IO.File.Exists(imagePath))
            {
                imagePath = Path.Combine(workingDir, folderName, "main_" + recipeId + ".jpg");
            }

            // Check if the image file exists
            if (!System.IO.File.Exists(imagePath))
            {
                imagePath  = Path.Combine("Resources", "Images", "Settings", "default_recipe_main_img.png");

                if (!System.IO.File.Exists(imagePath)) return NotFound(); // Return 404 Not Found if the image doesn't exist
            }

            // Serve the image file
            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(imageBytes, "image/jpeg"); // Return the image as a FileResult
        }
    }
}
