using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UsersController(TokenHelper tokenHelper)
        {
            TokenHelper = tokenHelper;
        }

        public TokenHelper TokenHelper { get; }

        [HttpPost("login")]
        public IActionResult Login(UserForLoginDto userForLoginDto)
        {
            if (userForLoginDto.Email != "janusz.guzowski@gmail.com" || userForLoginDto.Password != "1234")
            {
                return Unauthorized();
            }

            var user = new User 
            {
                Name = "Test User",
                Email = userForLoginDto.Email,
                Password = userForLoginDto.Password,
            };

            var userForDisplay = new UserForDisplayDto
            {
                Name = user.Name,
                Email = user.Email,
                Role = Role.ADMIN
            };

            var token = TokenHelper.CreateToken(user);

            return Ok(new { User = userForDisplay, Token = token });
        }
    }
}
