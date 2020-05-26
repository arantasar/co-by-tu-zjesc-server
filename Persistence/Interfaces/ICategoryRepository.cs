using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Interfaces
{
    public interface ICategoryRepository
    {
        Task Add(Category category);
        Task Remove(Guid id);
        Task<Category> Get(Guid id);
        Task<IEnumerable<Category>> List();
        Task<bool> Exists(string name);
    }
}
