using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.Interfaces;
using Persistence.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            CategoryRepository = categoryRepository;
        }

        public ICategoryRepository CategoryRepository { get; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Index()
        {
            var units = await CategoryRepository.List();
            return Ok(units);
        }

        [HttpGet("{id:guid}"), ActionName("Get")]
        public async Task<ActionResult<Category>> Get(Guid id)
        {
            var unit = await CategoryRepository.Get(id);
            if (unit == null)
            {
                return NotFound();
            }
            return Ok(unit);
        }

        [HttpPost]
        public async Task<ActionResult<Diet>> Add(CategoryForCreationDto categoryForCreationDto)
        {
            if (await CategoryRepository.Exists(categoryForCreationDto.Name))
            {
                return Conflict("Kategoria o takiej nazwie istnieje już w bazie!");
            }
            var category = new Category
            {
                Name = categoryForCreationDto.Name,
            };

            await CategoryRepository.Add(category);
            return CreatedAtAction("Get", new { id = category.Id }, category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCategory(Guid id)
        {
            string res = await CategoryRepository.Remove(id);
            return Ok(res);

            // return NoContent();
        }
    }
}
