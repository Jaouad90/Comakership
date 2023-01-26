using AutoMapper;
using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class ProgramService : BaseService<Program>, IProgramService
    {
        private readonly IProgramRepository _programRepository;
        private readonly IMapper _mapper;

        public ProgramService(IProgramRepository programRepository, IMapper mapper) : base(programRepository)
        {
            _programRepository = programRepository;
            _mapper = mapper;
        }

        // Create
        public async Task<int> CreateProgram(ProgramPost postedProgram)
        {
            var newProgram = _mapper.Map<Program>(postedProgram);
            if (await _programRepository.Add(newProgram))
            {
                return newProgram.Id;
            }
            else throw new Exception("Unable to add program due to internal error");            
        }

        // Read one
        public async Task<ProgramGet> GetProgram(int id)
        {            
            var program = await GetSingle(id);
            return _mapper.Map<ProgramGet>(program);
        }

        // Read all
        public async Task<IList<ProgramGet>> GetPrograms()
        {
            var programs = await _programRepository.GetPrograms();
            return _mapper.Map<List<ProgramGet>>(programs);             
        }

        // Update
        public async Task<bool> UpdateProgram(ProgramPut updatedProgram)
        {
            var dbProgram = await _programRepository.GetSingle(updatedProgram.Id);
            if (dbProgram == null)
            {
                return false;
            }
            _mapper.Map(updatedProgram, dbProgram);            
            return await _programRepository.Update(dbProgram);           
        }
    }
}
