using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Helpers;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Persistence.Interfaces;
using Persistence.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UsersController(TokenHelper tokenHelper, IRecipeRepository recipeRepository, IUserRepository userRepository,
            PhotoHelper photoHelper)
        {
            TokenHelper = tokenHelper;
            RecipeRepository = recipeRepository;
            UserRepository = userRepository;
            PhotoHelper = photoHelper;
        }

        public TokenHelper TokenHelper { get; }
        public IRecipeRepository RecipeRepository { get; }
        public IUserRepository UserRepository { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }
        public PhotoHelper PhotoHelper { get; }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await UserRepository.GetByEmail(userForLoginDto.Email);

            if (userFromRepo == null)
            {
                return NotFound(new { message = "Nie ma takiego użytkownika!" });
            }

            if (userForLoginDto.Password != userFromRepo.Password)
            {
                return Unauthorized(new { message = "Błędny login lub hasło!" });
            }

            var user = new User
            {
                Id = userFromRepo.Id,
                Role = userFromRepo.Role
            };

            var recipesAddedCount = NumberOfRecipesAdded(userFromRepo);

            var userForDisplay = new UserForDisplayDto
            {
                Id = userFromRepo.Id,
                Name = userFromRepo.Name,
                Email = userFromRepo.Email,
                Role = userFromRepo.Role,
                Favourites = userFromRepo.Favourites,
                PhotoPath = userFromRepo.PhotoPath,
                LastLogin = userFromRepo.LastLogin,
                Recipes = userFromRepo.Recipes,
                Description = userFromRepo.Description,
                DateCreated = userFromRepo.DateCreated,
                RecipesAddedCount = recipesAddedCount,
                ReceivedLikes = userFromRepo.ReceivedLikes
            };

            var token = TokenHelper.CreateToken(user);

            await UserRepository.UpdateLoginDate(userFromRepo);

            return Ok(new { User = userForDisplay, Token = token });
        }

        [HttpGet("{id:guid}/recipes")]
        public async Task<ActionResult<IEnumerable<RecipeForDisplayDto>>> Recipes(Guid id)
        {
            var recipes = await UserRepository.GetUserRecipes(id);
            var recipesForDisplay = new List<RecipeForDisplayDto>();

            foreach (var recipe in recipes)
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
                    UserId = recipe.UserId
                };
                recipesForDisplay.Add(recipeForDisplay);
            }
            return Ok(recipesForDisplay);
        }

        [HttpGet("{id:guid}"), ActionName("Get")]
        public async Task<ActionResult<UserForDisplayDto>> Get(Guid id)
        {
            var userFromRepo = await UserRepository.Get(id);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            var recipesAddedCount = NumberOfRecipesAdded(userFromRepo);

            var userForDisplay = new UserForDisplayDto
            {
                Id = userFromRepo.Id,
                Name = userFromRepo.Name,
                Email = userFromRepo.Email,
                Role = userFromRepo.Role,
                Favourites = userFromRepo.Favourites,
                PhotoPath = userFromRepo.PhotoPath,
                LastLogin = userFromRepo.LastLogin,
                Recipes = userFromRepo.Recipes,
                Description = userFromRepo.Description,
                RecipesAddedCount = recipesAddedCount,
                ReceivedLikes = userFromRepo.ReceivedLikes,
                DateCreated = userFromRepo.DateCreated
            };

            return Ok(userForDisplay);
        }

        [HttpGet(), ActionName("GetWeek")]
        public async Task<ActionResult<UserForDisplayDto>> GetWeek()
        {
            var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = await UserRepository.Get(Guid.Parse(id));
           
            // listę zakupów, listę przepisów żeby usunąć
            return Ok(userFromRepo.Week);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Add(UserForCreationDto userForCreationDto)
        {
            if (await UserRepository.Exists(userForCreationDto.Name, userForCreationDto.Email))
            {
                return Conflict(new { message = "Użytkownik o takiej nazwie lub adresie email istnieje już w bazie!" });
            }

            var user = new User
            {
                Name = userForCreationDto.Name,
                Email = userForCreationDto.Email,
                Password = userForCreationDto.Password,
            };
            await UserRepository.Add(user);
            return CreatedAtAction("Get", new { id = user.Id }, user);
        }

        [HttpPost]
        [Route("update")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<User>> Update([FromForm] UserForUpdateDto userForUpdateDto)
        {
            var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userForUpdate = await UserRepository.Get(Guid.Parse(id));

            string uniqueFileName = userForUpdate.PhotoPath;

            if (userForUpdateDto.Photo != null)
            {
                uniqueFileName = PhotoHelper.AddPhoto(userForUpdateDto.Photo, HttpContext, "Photos", "UserPhotos");
            }

            userForUpdate.PhotoPath = uniqueFileName;
            userForUpdate.Email = userForUpdateDto.Email;
            userForUpdate.Description = userForUpdateDto.Description;

            await UserRepository.Add(userForUpdate);
            return CreatedAtAction("Get", new { id = userForUpdate.Id }, userForUpdate);
        }

        [HttpPost]
        [Route("week")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<User>> Week(AddToWeekWrapper addToWeekWrapper)
        {
            var recipeId = addToWeekWrapper.RecipeId;
            var sizeFromClient = addToWeekWrapper.SizeFromClient;
            var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await UserRepository.Get(Guid.Parse(id));

            var recipe = await RecipeRepository.Get(recipeId);
            int size;

            if (sizeFromClient != null)
            {
                size = (int)sizeFromClient;
                if(size != recipe.Size)
                {
                    foreach(var recipeLine in recipe.RecipeLines)
                    {
                        recipeLine.Amount *= size / recipe.Size;
                    }
                }
            }
            else
            {
                size = recipe.Size;
            }

            RecipeForWeek recipeForWeek = new RecipeForWeek
            {
                Id = recipe.Id,
                Name = recipe.Name,
                RecipeLines = recipe.RecipeLines,
                Size = size
            };

            user.Week.Add(recipeForWeek);

            await UserRepository.Add(user);
            return Ok();
        }

        [HttpDelete]
        [Route("deleteweek")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<User>> DeleteWeek(Guid itemId)
        {
            var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await UserRepository.Get(Guid.Parse(id));

            var recipeToRemove = user.Week.Find(r => r.ItemId == itemId);
            user.Week.Remove(recipeToRemove);

            await UserRepository.Add(user);
            return Ok();
        }

        private int NumberOfRecipesAdded(User user)
        {
            int numberOfRecipes = user.Recipes.Count;
            return numberOfRecipes;
        }
    }
}
