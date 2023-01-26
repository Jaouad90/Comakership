using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ProgramRepository : BaseRepository<Program>, IProgramRepository
    {
        public ProgramRepository(ComakershipsContext _context) :base(_context)
        {           
        }

        // Read all
        public async Task<IEnumerable<Program>> GetPrograms()
        {
            return await _context.Programs
                .ToListAsync();
        }
    }
}
