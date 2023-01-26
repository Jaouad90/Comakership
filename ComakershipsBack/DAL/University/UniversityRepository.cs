using AutoMapper;
using Models;
using Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class UniversityRepository : IUniversityRepository
    {

        private readonly ComakershipsContext _context;
        private readonly IMapper _mapper;

        public UniversityRepository(IMapper mapper)
        {
            _mapper = mapper;
            _context = new ComakershipsContext();
        }

        public async Task<bool> CheckIfUniversityExistsAsync(int id)
        {
            return await _context.University.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> CheckIfUniversitynameExistsAsync(string name)
        {
            return await _context.University.AnyAsync(u => u.Name == name);
        }

        public async Task<bool> DeleteUniversityByIdAsync(int id)
        {
            University university = new University
            {
                Id = id
            };

            _context.University.Remove(university);

            return await _context.SaveChangesAsync() != 0;
        }

        public async Task<bool> EditUniversityAsync(University university)
        {
            University ExistingUniversity = await _context.University.FirstOrDefaultAsync(u => u.Id == university.Id);

            _context.Add(ExistingUniversity).State = EntityState.Modified;
            return await _context.SaveChangesAsync() != 0;
        }

        public async Task<IEnumerable<University>> GetAllUniversitiesAsync()
        {
            return await _context.University.ToListAsync();   
        }

        public async Task<IEnumerable<University>> GetAllUniversityDomainVMAsync()
        {
            List<University> universityList = await _context.University.ToListAsync();
            return universityList;
        }

        public async Task<University> GetByDomain(string domain) {
            return await _context.University.FirstOrDefaultAsync(u => u.Domain == domain);
        }

        public async Task<University> GetUniversityByIdAsync(int? id)
        {
            return await _context.University.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<UniversityDomainVM> GetUniversityDomainByIdAsync(int? id)
        {
            UniversityDomainVM universityDomainVM = _mapper.Map<UniversityDomainVM>(await _context.University.FirstOrDefaultAsync(s => s.Id == id));
            return universityDomainVM;
        }

        public async Task<bool> SaveUniversityAsync(University university)
        {
            _context.University.Add(university);
            return await _context.SaveChangesAsync() != 0;
        }
    }
}
