using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Helpers;
using API.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Persistence;
using Persistence.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        public IngredientsController(IIngredientRepository ingredientRepository, PhotoHelper photoHelper)
        {
            IngredientRepository = ingredientRepository;
            PhotoHelper = photoHelper;
        }

        public IIngredientRepository IngredientRepository { get; }
        public PhotoHelper PhotoHelper { get; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> Index()
        {
            var units = await IngredientRepository.List();
            return Ok(units);
        }

        [HttpGet("{id:guid}"), ActionName("Get")]
        public async Task<ActionResult<Ingredient>> Get(Guid id)
        {
            var unit = await IngredientRepository.Get(id);
            if (unit == null)
            {
                return NotFound();
            }
            return Ok(unit);
        }

        [HttpPost]
        public async Task<ActionResult<Ingredient>> Add(IngredientForCreationDto ingredientForCreationDto)
        {
            if (await IngredientRepository.Exists(ingredientForCreationDto.Name))
            {
                return Conflict(new {message = "Składnik o takiej nazwie istnieje już w bazie!" });
            }

            string uniqueFileName = null;
            if (ingredientForCreationDto.Photo != null)
            {
                uniqueFileName = PhotoHelper.AddPhoto(ingredientForCreationDto.Photo, "Photos", "IngredientPhotos");
            }

            var ingredient = new Ingredient
            {
                Name = ingredientForCreationDto.Name,
                Alternatives = ingredientForCreationDto.Alternatives,
                Units = ingredientForCreationDto.Units,
                PhotoPath = uniqueFileName
            };

            await IngredientRepository.Add(ingredient);
            return CreatedAtAction("Get", new { id = ingredient.Id }, ingredient);
        }

        [HttpDelete("{ingredientId}/{unitId}")]
        public async Task<IActionResult> RemoveUnit(Guid ingredientId, Guid unitId)
        {
            await IngredientRepository.RemoveUnit(ingredientId, unitId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveIngredient(Guid id)
        {
            await IngredientRepository.Remove(id);
            return NoContent();
        }
    }
}