using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Interfaces
{
    public interface IUnitRepository
    {
        Task Add(Unit unit);
        Task Remove(Guid id);
        Task<Unit> Get(Guid id);
        Task<IEnumerable<Unit>> List();
        Task<bool> Exists(string name);
    }
}
