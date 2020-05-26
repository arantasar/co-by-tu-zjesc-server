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
            ElasticClient = elasticClient;
        }

        public ElasticClient ElasticClient { get; }

        public async Task Add(Ingredient ingredient)
        {
            await ElasticClient.IndexAsync(ingredient, selector => selector.Index("ingredients"));
        }

        public async Task<Ingredient> Get(Guid id)
        {
            var ingredient = await ElasticClient.GetAsync<Ingredient>(id, selector => selector.Index("ingredients"));
            return ingredient.Source;
        }

        public async Task<bool> Exists(string name)
        {
            var query = await ElasticClient.CountAsync<Ingredient>(s => s
                .Index("ingredients")
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Query(name)
                        )
                    )
                );

            if (query.Count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Ingredient>> List()
        {
            var result = await ElasticClient.SearchAsync<Ingredient>(item => item
                .Index("ingredients")
                .MatchAll()
                .Sort(ss => ss.Ascending(p => p.Name.Suffix("keyword")))
            );
            return new List<Ingredient>(result.Documents);
        }

        public async Task Remove(Guid id)
        {
            await ElasticClient.DeleteAsync<Ingredient>(id, selector => selector.Index("ingredients").Refresh(Refresh.True));
        }

        public async Task Update(Ingredient ingredient)
        {
            var result = await ElasticClient.UpdateAsync<Ingredient>(ingredient.Id, selector =>
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
