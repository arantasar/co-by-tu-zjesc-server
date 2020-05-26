using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Interfaces
{
    public interface IDietRepository
    {
        Task Add(Diet diet);
        Task Remove(Guid id);
        Task<Diet> Get(Guid id);
        Task<IEnumerable<Diet>> List();
        Task<bool> Exists(string name);
    }
}
