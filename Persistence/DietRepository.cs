using Domain;
using Nest;
using Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class DietRepository : IDietRepository
    {
        public DietRepository(ElasticClient context)
        {
            Context = context;
        }

        public ElasticClient Context { get; }

        public async Task Add(Diet diet)
        {
            var result = await Context.IndexAsync(diet, s => s.Index("diets").Refresh(Elasticsearch.Net.Refresh.True));

            if (!result.IsValid)
            {
                throw new Exception(result.DebugInformation);
            }
        }

        public async Task<bool> Exists(string name)
        {
            var query = await Context.SearchAsync<Diet>(
                 s => s.Index("diets").Query(
                    q => q.Term(
                        p => p.Name.Suffix("keyword"), name)));

            return query.Hits.Count > 0;
        }

        public async Task<Diet> Get(Guid id)
        {
            var result = await Context.GetAsync<Diet>(id, selector => selector.Index("diets"));

            if (!result.IsValid)
            {
                throw new Exception(result.DebugInformation);
            }

            return result.Source;
        }

        public async Task<IEnumerable<Diet>> List()
        {
            var result = await Context.SearchAsync<Diet>(item => item
               .Index("diets")
               .MatchAll()
               .Sort(ss => ss.Ascending(p => p.Name.Suffix("keyword")))
           );

            if (!result.IsValid)
            {
                throw new Exception(result.DebugInformation);
            }

            return new List<Diet>(result.Documents);
        }

        public async Task Remove(Guid id)
        {
            await Context.DeleteAsync<Diet>(id, selector => selector.Index("diets").Refresh(Elasticsearch.Net.Refresh.True));
        }
    }
}
