using Domain;
using Elasticsearch.Net;
using Nest;
using Persistence.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class UnitRepository : IUnitRepository
    {
        public UnitRepository(ElasticClient elasticClient)
        {
            Context = elasticClient;
        }

        public ElasticClient Context { get; }

        public async Task Add(Unit unit)
        {
            await Context.IndexAsync(unit, selector => selector.Index("units").Refresh(Refresh.True));
        }

        public async Task<IEnumerable<Unit>> List()
        {
            var result = await Context.SearchAsync<Unit>(item => item
                .Index("units")
                .MatchAll()
                .Sort(ss => ss.Ascending(p => p.Name.Suffix("keyword")))
            );
            return new List<Unit>(result.Documents);
        }

        public async Task Remove(Guid id)
        {
            await Context.DeleteAsync<Unit>(id, selector => selector.Index("units").Refresh(Refresh.True));
        }

        public async Task<bool> Exists(string name)
        {
            var query = await Context.CountAsync<Unit>(s => s
                .Index("units")
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

        public async Task<Unit> Get(Guid id)
        {
            var result = await Context.GetAsync<Unit>(id, selector => selector.Index("units"));
            return result.Source;
        }
    }
}
