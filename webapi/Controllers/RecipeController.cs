using Mapster;
using Microsoft.AspNetCore.Mvc;
using webapi.Interfaces;
using webapi.Models.Basic;
using webapi.Models.Dtos;
using webapi.Models.Extended;
using webapi.Services;

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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var rowsDeleted = await _uow.RecipeRepository.DeleteRecipe(id);
            return Ok( new { DeletedRows = rowsDeleted } );
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
                var fileName = "main_" + recipeId + ".webp";
                var fileNameOriginal = "original_main_" + recipeId + Path.GetExtension(file.FileName);
                var fullPath = Path.Combine(pathToSave, fileName);
                var fullPathOriginal = Path.Combine(pathToSave, fileNameOriginal);

                // saving original file for badup (future changes)
                using (var stream = new FileStream(fullPathOriginal, FileMode.Create))
                    await file.CopyToAsync(stream);

                // saving compressed image
                var imageService = new ImageService();
                await imageService.CompressAndSaveImageAsync(file, fullPath);

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
            // TODO: specify valid file extensions (also when uploading)

            var folderName = Path.Combine("Resources", "Images", "Recipes");
            var workingDir = Directory.GetCurrentDirectory();
            string imagePath = Path.Combine(workingDir, folderName, "main_" + recipeId + ".*");

            string[] matchingFiles = Directory.GetFiles(Path.GetDirectoryName(imagePath)!, Path.GetFileName(imagePath));
            string firstMatchingFile;

            if (matchingFiles.Length > 0)
                firstMatchingFile = matchingFiles[0];
            else
                return NotFound(); // Return 404 Not Found if the image doesn't exist

            if (!System.IO.File.Exists(firstMatchingFile))
                return NotFound(); // Return 404 Not Found if the image doesn't exist

            var imageBytes = System.IO.File.ReadAllBytes(firstMatchingFile);
            return File(imageBytes, "image/jpeg"); // Return the image as a FileResult
        }
    }
}
