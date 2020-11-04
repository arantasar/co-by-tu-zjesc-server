using API.Helpers;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Interfaces;
using Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeRepository recipeRepository;

        public PhotoHelper PhotoHelper { get; }

        public RecipesController(IRecipeRepository recipeRepository, PhotoHelper photoHelper)
        {
            this.recipeRepository = recipeRepository;
            PhotoHelper = photoHelper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> Index()
        {
            var recipes = await recipeRepository.List();
            return Ok(recipes);
        }

        [HttpGet("{id:guid}"), ActionName("Get")]
        public async Task<ActionResult<Recipe>> Get(Guid id)
        {
            var recipe = await recipeRepository.Get(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return Ok(recipe);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Recipe>> Add(RecipeForCreationDto recipeForCreationDto)
        {
            if (await recipeRepository.Exists(recipeForCreationDto.Name))
            {
                return Conflict(new {message = "Przepis o takiej nazwie istnieje już w bazie!" });
            }

            var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string uniqueFileName = null;
            if (recipeForCreationDto.Photo != null)
            {
                uniqueFileName = PhotoHelper.AddPhoto(recipeForCreationDto, "Photos", "RecipePhotos");
            }

            var recipe = new Recipe
            {
                UserId = Guid.Parse(id),
                Name = recipeForCreationDto.Name,
                Categories = recipeForCreationDto.Categories,
                Description = recipeForCreationDto.Description,
                RecipeLines = recipeForCreationDto.RecipeLines,
                Diets = recipeForCreationDto.Diets,
                PhotoPath = uniqueFileName
            };
            await recipeRepository.Add(recipe);
            return CreatedAtAction("Get", new { id = recipe.Id }, recipe);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await recipeRepository.Remove(id);
            return NoContent();
        }
    }
}
