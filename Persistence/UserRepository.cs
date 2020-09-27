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
    public class UserRepository : IUserRepository
    {

        public UserRepository(ElasticClient elasticClient)
        {
            Context = elasticClient;
        }

        public ElasticClient Context { get; }

        public async Task Add(User user)
        {
            await Context.IndexAsync(user, selector => selector.Index("users").Refresh(Refresh.True));
        }

        public async Task<bool> Exists(string name)
        {
            var query = await Context.CountAsync<User>(s => s
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

        public async Task<User> Get(Guid id)
        {
            var result = await Context.GetAsync<User>(id, selector => selector.Index("users"));
            return result.Source;
        }

        public async Task<IEnumerable<User>> List()
        {
            var result = await Context.SearchAsync<User>(item => item
                       .Index("units")
                       .MatchAll()
                       .Sort(ss => ss.Ascending(p => p.Name.Suffix("keyword"))));
            return new List<User>(result.Documents);
        }

        public async Task Remove(Guid id)
        {
            await Context.DeleteAsync<Recipe>(id, selector => selector.Index("units").Refresh(Refresh.True));
        }
    }
}
