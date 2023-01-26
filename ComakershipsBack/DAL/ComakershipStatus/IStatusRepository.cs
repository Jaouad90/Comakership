using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    public interface IStatusRepository : IBaseRepository<ComakershipStatus>
    {
        Task<IEnumerable<ComakershipStatus>> GetStatuses();
    }
}
