﻿using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Interfaces
{
    public interface IUserRepository
    {
        Task Add(User user);
        Task Remove(Guid id);
        Task<User> Get(Guid id);
        Task<IEnumerable<User>> List();
        Task<bool> Exists(string name, string email);
        Task<bool> Exists(Guid id);
        Task<User> GetByEmail(string email);
        Task UpdateLoginDate(User updatedUser);
        Task<IEnumerable<Recipe>> GetUserRecipes(Guid id);
        Task IncrementReceivedLikes(User user);
    }
}
