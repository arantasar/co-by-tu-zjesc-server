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

        public async Task<bool> Exists(string name, string email)
        {
            var query = await Context.SearchAsync<User>(
                            s => s.Index("users").Query(
                                q => q.Term(
                                    p => p.Name.Suffix("keyword"), name) || q.Term(p => p.Email.Suffix("keyword"), email)));

            return query.Hits.Count > 0;
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

        public async Task<User> GetByEmail(string email)
        {
            var user = await Context.SearchAsync<User>(
                             s => s.Index("users").Query(
                                 q => q.Term(
                                     p => p.Email.Suffix("keyword"), email)));

                return user.Documents.FirstOrDefault();
        }
    }
}
