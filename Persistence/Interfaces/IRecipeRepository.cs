using Domain;
using Persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Interfaces
{
    public interface IRecipeRepository
    {
        Task Add(Recipe recipe);
        Task Remove(Guid id);
        Task<Recipe> Get(Guid id);
        Task<IEnumerable<Recipe>> List();
        Task<IEnumerable<Recipe>> Search(SearchDataDTO searchData);
        Task<bool> Exists(string name);
        Task<bool> Exists(Guid id);
        Task VievCounterRepositoryActualizer(Recipe recipe);
        Task IncrementInFavouriteRepository(Recipe recipe);
        Task DecrementInFavouriteRepository(Recipe recipe);
        Task AddLikeRepository(Recipe recipe);
    }
}
