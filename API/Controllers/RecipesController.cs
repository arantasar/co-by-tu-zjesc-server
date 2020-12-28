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
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeRepository recipeRepository;

        public PhotoHelper PhotoHelper { get; }
        public IUserRepository UserRepository { get; }

        public RecipesController(IRecipeRepository recipeRepository, PhotoHelper photoHelper, IUserRepository userRepository)
        {
            this.recipeRepository = recipeRepository;
            PhotoHelper = photoHelper;
            UserRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeForDisplayDto>>> Index()
        {
            var recipes = await recipeRepository.List();
            var recipesForDisplay = new List<RecipeForDisplayDto>();

            foreach(var recipe in recipes)
            {
                var userForRecipe = await UserRepository.Get(recipe.UserId);
                var recipeForDisplay = new RecipeForDisplayDto
                {
                    Id = recipe.Id,
                    Name = recipe.Name,
                    Description = recipe.Description,
                    RecipeLines = recipe.RecipeLines,
                    DateAdded = recipe.DateAdded,
                    ViewCounter = recipe.ViewCounter,
                    InFavourite = recipe.InFavourite,
                    Likes = recipe.Likes,
                    Categories = recipe.Categories,
                    Diets = recipe.Diets,
                    PhotoPath = recipe.PhotoPath,
                    User = new UserForRecipeDto { Id = userForRecipe.Id, Name = userForRecipe.Name, PhotoPath = userForRecipe.PhotoPath },
                    UserId = recipe.UserId,
                    PrepareTime = recipe.PrepareTime,
                    Size = recipe.Size
                };
                recipesForDisplay.Add(recipeForDisplay);
            }

            return Ok(recipesForDisplay);
        }

        [HttpGet("{id:guid}"), ActionName("Get")]
        public async Task<ActionResult<RecipeForDisplayDto>> Get(Guid id)
        {
            var recipe = await recipeRepository.Get(id);
            if (recipe == null)
            {
                return NotFound();
            }

            await ViewCounterActualizer(recipe);

            var userForRecipe = await UserRepository.Get(recipe.UserId);
            var recipeForDisplay = new RecipeForDisplayDto
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                RecipeLines = recipe.RecipeLines,
                DateAdded = recipe.DateAdded,
                ViewCounter = recipe.ViewCounter,
                InFavourite = recipe.InFavourite,
                Likes = recipe.Likes,
                Categories = recipe.Categories,
                Diets = recipe.Diets,
                PhotoPath = recipe.PhotoPath,
                User = new UserForRecipeDto {Id = userForRecipe.Id, Name = userForRecipe.Name, PhotoPath = userForRecipe.PhotoPath },
                UserId = recipe.UserId,
                PrepareTime = recipe.PrepareTime,
                Size = recipe.Size
            };

            return Ok(recipeForDisplay);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Recipe>> Add([FromForm]RecipeForCreationDto recipeForCreationDto)
        {
            if (await recipeRepository.Exists(recipeForCreationDto.Name))
            {
                return Conflict(new {message = "Przepis o takiej nazwie istnieje już w bazie!" });
            }

            var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string uniqueFileName = null;
            if (recipeForCreationDto.Photo != null)
            {
                uniqueFileName = PhotoHelper.AddPhoto(recipeForCreationDto.Photo,HttpContext ,"Photos", "RecipePhotos");
            }

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var recipe = new Recipe
            {
                UserId = Guid.Parse(id),
                Name = recipeForCreationDto.Name,
                Categories = JsonSerializer.Deserialize<List<Category>>(recipeForCreationDto.Categories, serializeOptions),
                Description = recipeForCreationDto.Description,
                RecipeLines = JsonSerializer.Deserialize<List<RecipeLine>>(recipeForCreationDto.RecipeLines, serializeOptions),
                Diets = JsonSerializer.Deserialize<List<Diet>>(recipeForCreationDto.Diets, serializeOptions),
                PhotoPath = uniqueFileName,
                PrepareTime = recipeForCreationDto.PrepareTime,
                Size = recipeForCreationDto.Size
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

        

        [HttpPost]
        [Route("update/{recipeId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<User>> Update([FromForm] RecipeForUpdateDto recipeForUpdateDto, [FromRoute]Guid recipeId )
        {      
            if (recipeId == null)
            {
                return NotFound(new { message = "Nie podano przepisu!" });
            }

            if (await recipeRepository.Exists(recipeId) == false)
            {
                return NotFound(new { message = "Brak przepisu w bazie!" });
            }

            recipeForUpdateDto.Id = recipeId;

            //var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var recipeForUpdate = await recipeRepository.Get(Guid.Parse(id));

            var recipeForUpdate = await recipeRepository.Get(recipeId);

            string uniqueFileName = recipeForUpdate.PhotoPath;

            if (recipeForUpdateDto.Photo != null)
            {
                uniqueFileName = PhotoHelper.AddPhoto(recipeForUpdateDto.Photo, HttpContext, "Photos", "RecipePhotos");
            }

            recipeForUpdate.PhotoPath = uniqueFileName;
            recipeForUpdate.Name = recipeForUpdateDto.Name;
            recipeForUpdate.Description = recipeForUpdateDto.Description;
            recipeForUpdate.RecipeLines = recipeForUpdateDto.RecipeLines;
            recipeForUpdate.Categories = recipeForUpdateDto.Categories;
            recipeForUpdate.Diets = recipeForUpdateDto.Diets;
            recipeForUpdate.PrepareTime = recipeForUpdateDto.PrepareTime;
            recipeForUpdate.Size = recipeForUpdateDto.Size;

            await recipeRepository.Add(recipeForUpdate);
            return CreatedAtAction("Get", new { id = recipeForUpdate.Id }, recipeForUpdate);
        }

        [HttpPost]
        [Route("favourites")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<UserWithRecipeDto>> Favourites(RecipeIdWrapper recipeIdWrapper)
        {
            if (recipeIdWrapper.Id == null)
            {
                return NotFound(new { message = "Nie podano przepisu!" });
            }

            if (await recipeRepository.Exists(recipeIdWrapper.Id) == false)
            {
                return NotFound(new { message = "Brak przepisu w bazie!" });
            }

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await UserRepository.Get(Guid.Parse(userId));

            var recipe = await recipeRepository.Get(recipeIdWrapper.Id);
            
            var recipeForFavourites = new RecipeForFavourite
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Categories = recipe.Categories,
                Diets = recipe.Diets,
                PhotoPath = recipe.PhotoPath,
                UserName = user.Name,
                UserId = recipe.UserId,
                PrepareTime = recipe.PrepareTime
            };

            var recipeInFavourites = user.Favourites.Find(r => r.Id == recipeForFavourites.Id);

            if (recipeInFavourites != null)
            {
                await DecrementInFavourite(recipe);
                user.Favourites.Remove(recipeInFavourites);
            }
            else
            {
                await IncrementInFavourite(recipe);
                user.Favourites.Add(recipeForFavourites);
            }
            await UserRepository.Add(user);

            return Ok(new UserWithRecipeDto {
                Recipe = recipe,
                User = user
            });
        }

        [HttpPost]
        [Route("likes")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Recipe>> Likes(RecipeIdWrapper recipeIdWrapper)
        {
            if (recipeIdWrapper.Id == null)
            {
                return NotFound(new { message = "Nie podano przepisu!" });
            }

            if (await recipeRepository.Exists(recipeIdWrapper.Id) == false)
            {
                return NotFound(new { message = "Brak przepisu w bazie!" });
            }

            var recipe = await recipeRepository.Get(recipeIdWrapper.Id);

            await AddLike(recipe);

            return Ok(recipe);
        }

        [HttpGet("newest/{amount?}")]
        public async Task<ActionResult<IEnumerable<RecipeForDisplayDto>>> Newest(int amount = 4)
        {
            var recipes = await recipeRepository.List();
            var recipesForDisplay = new List<RecipeForDisplayDto>();
            Recipe[] recipesSortedByDate = new Recipe[] {};

            recipesSortedByDate = recipes.OrderByDescending(o => DateTime.Parse(o.DateAdded)).ToArray();

            var arrayRange = Math.Min(amount, recipesSortedByDate.Length);
            
            for (int i = 0; i < arrayRange; i++)
            {
                var userForRecipe = await UserRepository.Get(recipesSortedByDate[i].UserId);
                var recipeForDisplay = new RecipeForDisplayDto
                {
                    Id = recipesSortedByDate[i].Id,
                    Name = recipesSortedByDate[i].Name,
                    Description = recipesSortedByDate[i].Description,
                    RecipeLines = recipesSortedByDate[i].RecipeLines,
                    DateAdded = recipesSortedByDate[i].DateAdded,
                    ViewCounter = recipesSortedByDate[i].ViewCounter,
                    InFavourite = recipesSortedByDate[i].InFavourite,
                    Likes = recipesSortedByDate[i].Likes,
                    Categories = recipesSortedByDate[i].Categories,
                    Diets = recipesSortedByDate[i].Diets,
                    PhotoPath = recipesSortedByDate[i].PhotoPath,
                    User = new UserForRecipeDto 
                    { Id = userForRecipe.Id, Name = userForRecipe.Name, PhotoPath = userForRecipe.PhotoPath },
                    UserId = recipesSortedByDate[i].UserId,
                    PrepareTime = recipesSortedByDate[i].PrepareTime,
                    Size = recipesSortedByDate[i].Size
                };
                recipesForDisplay.Add(recipeForDisplay);
            }
            return Ok(recipesForDisplay);
        }

        [HttpGet("mostliked/{amount?}")]
        public async Task<ActionResult<IEnumerable<RecipeForDisplayDto>>> MostLiked(int amount = 4)
        {
            var recipes = await recipeRepository.List();
            var recipesForDisplay = new List<RecipeForDisplayDto>();
            Recipe[] recipesSortedByLikes = new Recipe[] { };

            recipesSortedByLikes = recipes.OrderByDescending(o => o.Likes).ToArray();

            var arrayRange = Math.Min(amount, recipesSortedByLikes.Length);

            for (int i = 0; i < arrayRange; i++)
            {
                var userForRecipe = await UserRepository.Get(recipesSortedByLikes[i].UserId);
                var recipeForDisplay = new RecipeForDisplayDto
                {
                    Id = recipesSortedByLikes[i].Id,
                    Name = recipesSortedByLikes[i].Name,
                    Description = recipesSortedByLikes[i].Description,
                    RecipeLines = recipesSortedByLikes[i].RecipeLines,
                    DateAdded = recipesSortedByLikes[i].DateAdded,
                    ViewCounter = recipesSortedByLikes[i].ViewCounter,
                    InFavourite = recipesSortedByLikes[i].InFavourite,
                    Likes = recipesSortedByLikes[i].Likes,
                    Categories = recipesSortedByLikes[i].Categories,
                    Diets = recipesSortedByLikes[i].Diets,
                    PhotoPath = recipesSortedByLikes[i].PhotoPath,
                    User = new UserForRecipeDto
                    { Id = userForRecipe.Id, Name = userForRecipe.Name, PhotoPath = userForRecipe.PhotoPath },
                    UserId = recipesSortedByLikes[i].UserId,
                    PrepareTime = recipesSortedByLikes[i].PrepareTime,
                    Size = recipesSortedByLikes[i].Size
                };
                recipesForDisplay.Add(recipeForDisplay);
            }
            return Ok(recipesForDisplay);
        }


        private async Task ViewCounterActualizer(Recipe recipe)
        {
            recipe.ViewCounter++;
            await recipeRepository.VievCounterRepositoryActualizer(recipe);
        }

        private async Task IncrementInFavourite(Recipe recipe)
        {
            recipe.InFavourite++;
            await recipeRepository.IncrementInFavouriteRepository(recipe);
        }

        private async Task DecrementInFavourite(Recipe recipe)
        {
            recipe.InFavourite--;
            await recipeRepository.DecrementInFavouriteRepository(recipe);
        }

        private async Task AddLike(Recipe recipe)
        {
            recipe.Likes++;
            await recipeRepository.AddLikeRepository(recipe);
        }
    }
}
