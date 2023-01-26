using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class StatusRepository : BaseRepository<ComakershipStatus>, IStatusRepository
    {
        public StatusRepository(ComakershipsContext _context) :base(_context)
        {           
        }              

        // Read all
        public async Task<IEnumerable<ComakershipStatus>> GetStatuses()
        {
            return await _context.ComakershipStatuses
                .ToListAsync();
        }
    }
}
