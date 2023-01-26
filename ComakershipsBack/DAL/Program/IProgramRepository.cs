using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    public interface IProgramRepository : IBaseRepository<Program>
    {
        Task<IEnumerable<Program>> GetPrograms();
    }
}
