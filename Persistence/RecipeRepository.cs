using Domain;
using Persistence.Interfaces;
using Nest;
using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class RecipeRepository : IRecipeRepository
    {

        public RecipeRepository(ElasticClient elasticClient)
        {
            Context = elasticClient;
        }

        public ElasticClient Context { get; }

        public async Task Add(Recipe recipe)
        {
            await Context.IndexAsync(recipe, selector => selector.Index("recipes").Refresh(Refresh.True));
        }

        public async Task<bool> Exists(string name)
        {
            var query = await Context.SearchAsync<Recipe>(
                 s => s.Index("recipes").Query(
                    q => q.Term(
                        p => p.Name.Suffix("keyword"), name)));

            return query.Hits.Count > 0;
        }

        public async Task<Recipe> Get(Guid id)
        {
            var result = await Context.GetAsync<Recipe>(id, selector => selector.Index("recipes"));
            return result.Source;
        }

        public async Task<IEnumerable<Recipe>> List()
        {
            var result = await Context.SearchAsync<Recipe>(item => item
                 .Index("units")
                 .MatchAll()
                 .Sort(ss => ss.Ascending(p => p.Name.Suffix("keyword")))
            );
            return new List<Recipe>(result.Documents);
        }

        public async Task Remove(Guid id)
        {
            await Context.DeleteAsync<Recipe>(id, selector => selector.Index("units").Refresh(Refresh.True));
        }
    }
}
