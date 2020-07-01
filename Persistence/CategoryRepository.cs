using Domain;
using Elasticsearch.Net;
using Nest;
using Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class CategoryRepository : ICategoryRepository
    {
        public CategoryRepository(ElasticClient context)
        {
            Context = context;
        }

        public ElasticClient Context { get; }

        public async Task Add(Category category)
        {
            var result = await Context.IndexAsync(category, s => s.Index("categories").Refresh(Elasticsearch.Net.Refresh.True));

            if (!result.IsValid)
            {
                throw new Exception(result.DebugInformation);
            }
        }

        public async Task<bool> Exists(string name)
        {
            var query = await Context.CountAsync<Category>(s => s
                .Index("categories")
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Query(name)
                        )
                    )
                );

            if (!query.IsValid)
            {
                throw new Exception(query.DebugInformation);
            }

            if (query.Count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<Category> Get(Guid id)
        {
            var result = await Context.GetAsync<Category>(id, selector => selector.Index("categories"));

            if (!result.IsValid)
            {
                throw new Exception(result.DebugInformation);
            }

            return result.Source;
        }

        public async Task<IEnumerable<Category>> List()
        {
            var result = await Context.SearchAsync<Category>(item => item
               .Index("categories")
               .MatchAll()
               .Sort(ss => ss.Ascending(p => p.Name.Suffix("keyword")))
           );

            if (!result.IsValid)
            {
                throw new Exception(result.DebugInformation);
            }

            return new List<Category>(result.Documents);
        }

        public async Task<string> Remove(Guid id)
        {
            var res = await Context.DeleteAsync<Category>(id, selector => selector.Index("categories").Refresh(Refresh.True));
            return res.DebugInformation;
        }
    }
}
