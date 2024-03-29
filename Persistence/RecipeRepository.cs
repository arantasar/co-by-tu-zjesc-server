﻿using Domain;
using Persistence.Interfaces;
using Nest;
using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Persistence.Models;

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

        public async Task<bool> Exists(Guid id)
        {
            var query = await Context.SearchAsync<Recipe>(
                 s => s.Index("recipes").Query(
                    q => q.Term(
                        p => p.Id.Suffix("keyword"), id)));

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
                 .Index("recipes")
                 .MatchAll()
                 .Sort(ss => ss.Ascending(p => p.Name.Suffix("keyword")))
            );
            return new List<Recipe>(result.Documents);
        }

        public async Task Remove(Guid id)
        {
            await Context.DeleteAsync<Recipe>(id, selector => selector.Index("recipes").Refresh(Refresh.True));
        }

        public async Task VievCounterRepositoryActualizer(Recipe recipe)
        {
            var result = await Context.GetAsync<Recipe>(recipe.Id, selector => selector.Index("recipes"));
            result.Source.ViewCounter++;

            await Context.UpdateAsync<Recipe>(result.Source.Id, selector => selector.Index("recipes").Doc(result.Source).Refresh(Refresh.True));
        }

        public async Task IncrementInFavouriteRepository(Recipe recipe)
        {
            var result = await Context.GetAsync<Recipe>(recipe.Id, selector => selector.Index("recipes"));
            result.Source.InFavourite++;

            await Context.UpdateAsync<Recipe>(result.Source.Id, selector => selector.Index("recipes").Doc(result.Source).Refresh(Refresh.True));
        }
        public async Task DecrementInFavouriteRepository(Recipe recipe)
        {
            var result = await Context.GetAsync<Recipe>(recipe.Id, selector => selector.Index("recipes"));
            result.Source.InFavourite--;

            await Context.UpdateAsync<Recipe>(result.Source.Id, selector => selector.Index("recipes").Doc(result.Source).Refresh(Refresh.True));
        }

        public async Task AddLikeRepository(Recipe recipe)
        {
            var result = await Context.GetAsync<Recipe>(recipe.Id, selector => selector.Index("recipes"));
            result.Source.Likes++;

            await Context.UpdateAsync<Recipe>(result.Source.Id, selector => selector.Index("recipes").Doc(result.Source).Refresh(Refresh.True));
        }

        public async Task<IEnumerable<Recipe>> Search(SearchDataDTO searchData)
        {
            var shoulds = searchData.Ingredients.ConvertAll<QueryContainer>(i => new MatchQuery
            {
                Field = "recipeLines.ingredient.id",
                Query = i.Id
            });

            var categories = searchData.Categories.ConvertAll<string>(c => c.Id);
            var diets = searchData.Diets.ConvertAll<string>(d => d.Id);

            var result = await Context.SearchAsync<Recipe>(s => s
                .Index("recipes")
                .Query(q => q
                    .Bool(b => b
                        .Should(shoulds.ToArray())
                        .Filter(new TermsQuery { 
                            Field = "categories.id.keyword",
                            Terms = categories
                        }, new TermsQuery
                        {
                            Field = "diets.id.keyword",
                            Terms = diets
                        })
                        .MinimumShouldMatch(1)
                )
            ));

            return new List<Recipe>(result.Documents);
        }
    }
}
