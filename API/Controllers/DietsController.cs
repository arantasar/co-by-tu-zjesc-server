using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Persistence.Interfaces;
using Persistence.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DietsController : ControllerBase
    {
        public DietsController(IDietRepository dietRepository)
        {
            DietRepository = dietRepository;
        }

        public IDietRepository DietRepository { get; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Diet>>> Index()
        {
            var units = await DietRepository.List();
            return Ok(units);
        }

        [HttpGet("{id:guid}"), ActionName("Get")]
        public async Task<ActionResult<Diet>> Get(Guid id)
        {
            var unit = await DietRepository.Get(id);
            if (unit == null)
            {
                return NotFound();
            }
            return Ok(unit);
        }

        [HttpPost]
        public async Task<ActionResult<Diet>> Add(DietForCreationDto dietForCreationDto)
        {
            if (await DietRepository.Exists(dietForCreationDto.Name))
            {
                return Conflict();
            }
            var diet = new Diet
            {
                Name = dietForCreationDto.Name,
            };

            await DietRepository.Add(diet);
            return CreatedAtAction("Get", new { id = diet.Id }, diet);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUnit(Guid id)
        {
            await DietRepository.Remove(id);
            return NoContent();
        }
    }
}
