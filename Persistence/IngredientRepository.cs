using Domain;
using Elasticsearch.Net;
using Nest;
using Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class IngredientRepository : IIngredientRepository
    {
        public IngredientRepository(ElasticClient elasticClient)
        {
            Context = elasticClient;
        }

        public ElasticClient Context { get; }

        public async Task Add(Ingredient ingredient)
        {
            await Context.IndexAsync(ingredient, selector => selector
            .Index("ingredients")
            .Refresh(Refresh.True)
            );
        }

        public async Task<Ingredient> Get(Guid id)
        {
            var ingredient = await Context.GetAsync<Ingredient>(id, selector => selector.Index("ingredients"));
            return ingredient.Source;
        }

        public async Task<bool> Exists(string name)
        {
            var query = await Context.SearchAsync<Ingredient>(
                 s => s.Index("ingredients").Query(
                    q => q.Term(
                        p => p.Name.Suffix("keyword"), name)));

            return query.Hits.Count > 0;
        }

        public async Task<IEnumerable<Ingredient>> List()
        {
            var result = await Context.SearchAsync<Ingredient>(item => item
                .Index("ingredients")
                .MatchAll()
                .Sort(ss => ss.Ascending(p => p.Name.Suffix("keyword")))
            );
            return new List<Ingredient>(result.Documents);
        }

        public async Task Remove(Guid id)
        {
            await Context.DeleteAsync<Ingredient>(id, selector => selector.Index("ingredients").Refresh(Refresh.True));
        }

        public async Task Update(Ingredient ingredient)
        {
            var result = await Context.UpdateAsync<Ingredient>(ingredient.Id, selector =>
                selector.Index("ingredients").Doc(ingredient).Refresh(Refresh.True));
        }

        public async Task AddUnit(Guid id, Unit unit)
        {
            var ingredent = await Get(id);
            ingredent.Units.Add(unit);
            await Update(ingredent);
        }

        public async Task RemoveUnit(Guid id, Guid idToRemove)
        {
            var ingredent = await Get(id);
            var unitToRemove = ingredent.Units.Find(item => item.Id == idToRemove);
            ingredent.Units.Remove(unitToRemove);
            await Update(ingredent);
        }

        public Task AddAlternative(Guid id, Ingredient ingredient)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAlternative(Guid id, Guid idToRemove)
        {
            throw new NotImplementedException();
        }
    }
}
