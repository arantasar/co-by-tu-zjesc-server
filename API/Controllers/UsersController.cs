using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            IWebHostEnvironment webHostEnvironment)
        {
            TokenHelper = tokenHelper;
            RecipeRepository = recipeRepository;
            UserRepository = userRepository;
            WebHostEnvironment = webHostEnvironment;
        }

        public TokenHelper TokenHelper { get; }
        public IRecipeRepository RecipeRepository { get; }
        public IUserRepository UserRepository { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await UserRepository.GetByEmail(userForLoginDto.Email);

            if(userFromRepo == null)
            {
                return NotFound(new { message = "Nie ma takiego użytkownika!" });
            }

            if (userForLoginDto.Password != userFromRepo.Password)
            {
                return Unauthorized(new {message = "Błędny login lub hasło!" });
            }

            var user = new User 
            {
                Id = userFromRepo.Id,
                Role = userFromRepo.Role
            };

            var userForDisplay = new UserForDisplayDto
            {
                Name = userFromRepo.Name,
                Email = userFromRepo.Email,
                Role = userFromRepo.Role,
                Favourites = userFromRepo.Favourites,
                PhotoPath = userFromRepo.PhotoPath,
                LastLogin = userFromRepo.LastLogin,
                Recipes = userFromRepo.Recipes
            };

            var token = TokenHelper.CreateToken(user);

            await UserRepository.UpdateLoginDate(userFromRepo);

            return Ok(new { User = userForDisplay, Token = token });
        }

        [HttpGet("{id:guid}/recipes")]
        public async Task<ActionResult<IEnumerable<Recipe>>> Recipes()
        {
            var recipes = await RecipeRepository.List();
            return Ok(recipes);
        }

        [HttpGet("{id:guid}"), ActionName("Get")]
        public async Task<ActionResult<Recipe>> Get(Guid id)
        {
            var user = await UserRepository.Get(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Add(UserForCreationDto userForCreationDto)
        {
            if (await UserRepository.Exists(userForCreationDto.Name, userForCreationDto.Email))
            {
                return Conflict(new {message = "Użytkownik o takiej nazwie lub adresie email istnieje już w bazie!" });
            }

            string uniqueFileName = null;
            if(userForCreationDto.Photo != null)
            {
                string uploadsFolder = Path.Combine(WebHostEnvironment.WebRootPath, "Photos", "UserPhotos");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + userForCreationDto.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                userForCreationDto.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            var user = new User
            {
                Name = userForCreationDto.Name,
                Email = userForCreationDto.Email,
                Password = userForCreationDto.Password,
                PhotoPath = uniqueFileName
            };
            await UserRepository.Add(user);
            return CreatedAtAction("Get", new { id = user.Id }, user);
        }
    }
}
