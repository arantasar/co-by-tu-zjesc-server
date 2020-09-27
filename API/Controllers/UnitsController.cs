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
    public class UnitsController : ControllerBase
    {
        public UnitsController(IUnitRepository unitRepository)
        {
            UnitRepository = unitRepository;
        }

        public IUnitRepository UnitRepository { get; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Unit>>> Index()
        {
            var units = await UnitRepository.List();
            return Ok(units);
        }

        [HttpGet("{id:guid}"), ActionName("Get")]
        public async Task<ActionResult<Unit>> Get(Guid id)
        {
            var unit = await UnitRepository.Get(id);
            if (unit == null)
            {
                return NotFound();
            }
            return Ok(unit);
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> Add(UnitForCreationDto unitForCreationDto)
        {
            if (await UnitRepository.Exists(unitForCreationDto.Name))
            {
                return Conflict();
            }
            var unit = new Unit { Name = unitForCreationDto.Name };
            await UnitRepository.Add(unit);
            return CreatedAtAction("Get", new { id = unit.Id }, unit);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await UnitRepository.Remove(id);
            return NoContent();
        }
    }
}