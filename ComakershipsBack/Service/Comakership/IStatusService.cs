using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ServiceLayer
{
    public interface IStatusService : IBaseService<ComakershipStatus>
    {
        Task<IList<ComakershipStatusGet>> GetStatuses();

        Task<ComakershipStatusGet> GetStatus(int id);

        Task<int> CreateStatus(ComakershipStatusPost newStatus);

        Task<bool> UpdateStatus(ComakershipStatusPut updatedStatus);

        Task<bool> DeleteStatus(int id);
    }
}
