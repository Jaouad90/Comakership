using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ServiceLayer
{
    public interface IProgramService : IBaseService<Program>
    {
        Task<IList<ProgramGet>> GetPrograms();

        Task<ProgramGet> GetProgram(int id);

        Task<int> CreateProgram(ProgramPost newProgram);

        Task<bool> UpdateProgram(ProgramPut updatedProgram);
    }
}
