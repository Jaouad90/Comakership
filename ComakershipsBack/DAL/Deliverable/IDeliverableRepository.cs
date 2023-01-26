using Models;
using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Threading.Tasks;

namespace DAL
{
    public interface IDeliverableRepository : IBaseRepository<Deliverable>
    {
        Task<IEnumerable<Deliverable>> GetDeliverables(int comakershipId);

        Task<int> CreateDeliverable(Comakership comakership, Deliverable newDeliverable);
    }
}
