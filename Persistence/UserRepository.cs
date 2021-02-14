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

        public async Task UpdateLoginDate(User updatedUser)
        {
            updatedUser.LastLogin = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            await Context.UpdateAsync<User>(updatedUser.Id, 
                selector => selector.Index("users").Doc(updatedUser));
        }

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

        public async Task<bool> Exists(Guid id)
        {
            var query = await Context.SearchAsync<User>(
                            s => s.Index("users").Query(
                                q => q.Term(
                                    p => p.Id.Suffix("keyword"), id)));

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
                       .Index("users")
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

        public async Task<IEnumerable<Recipe>> GetUserRecipes(Guid id)
        {
            var recipes = await Context.SearchAsync<Recipe>(
                s => s.Index("recipes").Query(
                                q => q.Term(
                                    p => p.UserId.Suffix("keyword"), id)).Size(1000));

            return recipes.Documents;
        }

        public async Task IncrementReceivedLikes(User user)
        {
            var result = await Context.GetAsync<User>(user.Id, selector => selector.Index("users"));
            result.Source.ReceivedLikes++;

            await Context.UpdateAsync<User>(result.Source.Id, selector => selector.Index("users").Doc(result.Source).Refresh(Refresh.True));
        }
    }
}
