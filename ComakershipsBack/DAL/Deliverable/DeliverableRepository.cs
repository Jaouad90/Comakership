using Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using System.Threading.Tasks;

namespace DAL
{
    public class DeliverableRepository : BaseRepository<Deliverable>, IDeliverableRepository
    {       
        public DeliverableRepository(ComakershipsContext _context) :base(_context)
        {           
        }

        // Create
        public async Task<int> CreateDeliverable(Comakership comakership, Deliverable newDeliverable)
        {
            comakership.Deliverables.Add(newDeliverable);                 
            await _context.SaveChangesAsync();
            return newDeliverable.Id;
        }

        // Read all
        public async Task<IEnumerable<Deliverable>> GetDeliverables(int comakershipId)
        {
            return await _context.Deliverables.Where(t => t.ComakershipId == comakershipId).ToListAsync();
        }       
    }
}
