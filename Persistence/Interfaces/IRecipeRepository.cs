﻿using Domain;
using Persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Interfaces
{
    public interface IRecipeRepository
    {
        Task Add(Recipe recipe);
        Task Remove(Guid id);
        Task<Recipe> Get(Guid id);
        Task<IEnumerable<Recipe>> List();
        Task<bool> Exists(string name);
    }
}