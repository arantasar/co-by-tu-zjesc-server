using Domain;
using Microsoft.AspNetCore.Mvc;
using Persistence;
using Persistence.Interfaces;
using Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        public IRecipeRepository RecipeRepository { get; private set; }

        public SearchController(IRecipeRepository recipeRepository)
        {
            RecipeRepository = recipeRepository;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Recipe>>> Search(SearchDataDTO searchData) 
        {
            var recipes = await RecipeRepository.Search(searchData);
            if (searchData.Size[0] != null)
            {
                recipes.ToList().ForEach(r => {
                    var requiredSize = Int32.Parse(searchData.Size[0].Id);
                    if (requiredSize != r.Size)
                    {
                        float factor = requiredSize / (float)r.Size;
                        r.RecipeLines.ForEach(line => line.Amount *= factor);
                    }
                });
            }
          

            return Ok(recipes);
        }
           

    }
}
