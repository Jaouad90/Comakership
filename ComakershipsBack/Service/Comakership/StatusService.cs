using AutoMapper;
using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class StatusService : BaseService<ComakershipStatus>, IStatusService
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IMapper _mapper;

        public StatusService(IStatusRepository statusRepository, IMapper mapper) : base(statusRepository)
        {
            _statusRepository = statusRepository;
            _mapper = mapper;
        }
        // Create
        public async Task<int> CreateStatus(ComakershipStatusPost postedStatus)
        {
            var newStatus = _mapper.Map<ComakershipStatus>(postedStatus);
            //return await _statusRepository.CreateStatus(newStatus);
            if (await _statusRepository.Add(newStatus))
            {
                return newStatus.Id;
            }
            else throw new Exception("Unable to add status due to internal error");
        }

        // Read one
        public async Task<ComakershipStatusGet> GetStatus(int id)
        {
           var status = await _statusRepository.GetSingle(id);
            return _mapper.Map<ComakershipStatusGet>(status);
        }

        // Read all
        public async Task<IList<ComakershipStatusGet>> GetStatuses()
        {
            var statusses = await _statusRepository.GetStatuses();
            return _mapper.Map<List<ComakershipStatusGet>>(statusses);             
        }

        // Update
        public async Task<bool> UpdateStatus(ComakershipStatusPut updatedStatus)
        {
            var dbStatus = await _statusRepository.GetSingle(updatedStatus.Id);
            if (dbStatus == null)
            {
                return false;
            }
            _mapper.Map(updatedStatus, dbStatus);
            return await _statusRepository.Update(dbStatus);           
        }

        // Delete
        public async Task<bool> DeleteStatus(int id)
        {
            return await _statusRepository.DeleteWhere(s => s.Id == id);
        }
    }
}
