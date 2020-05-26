using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Interfaces
{
    public interface IIngredientRepository
    {
        Task Add(Ingredient ingredient);
        Task Update(Ingredient ingredient);
        Task<Ingredient> Get(Guid id);
        Task Remove(Guid id);
        Task<IEnumerable<Ingredient>> List();
        Task<bool> Exists(string name);
        Task AddUnit(Guid id, Unit unit);
        Task RemoveUnit(Guid id, Guid idToRemove);
        Task AddAlternative(Guid id, Ingredient ingredient);
        Task RemoveAlternative(Guid id, Guid idToRemove);

    }
}
