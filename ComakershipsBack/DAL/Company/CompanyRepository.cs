using AutoMapper;
using Models;
using Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;

namespace DAL
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        private readonly IMapper _mapper;

        public CompanyRepository(ComakershipsContext _context, IMapper mapper) : base(_context)
        {
            _mapper = mapper;
        }

        public async Task<bool> CheckifCompanyExistsAsync(int id)
        {
            return await _context.Company.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> CheckIfCompanynameExistsAsync(string name)
        {
            return await _context.Company.AnyAsync(c => c.Name == name);
        }

        public async Task<bool> DeleteCompanyByIdAsync(int id)
        {
            Company company = new Company();
            company.Id = id;

            _context.Company.Remove(company);

            return await _context.SaveChangesAsync() != 0;
        }

        public async Task<bool> EditCompanyAsync(CompanyPutVM companyPutVM)
        {
            Company ExisitingCompany = await _context.Company.FirstOrDefaultAsync(c => c.Id == companyPutVM.Id);
            Company UpdatedCompany = _mapper.Map(companyPutVM, ExisitingCompany);

            _context.Add(ExisitingCompany).State = EntityState.Modified;

            return await _context.SaveChangesAsync() != 0;
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _context.Company.ToListAsync();
        }

        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            return await _context.Company
                .Include(x => x.Reviews)
                .SingleAsync(x => x.Id == id);
        }

        public async Task<bool> SaveCompanyAsync(Company company)
        {
            _context.Company.Add(company);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Company> GetCompanyByName(string name) {
            return await _context.Company.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<CompanyLogoVM> GetCompanyIdAndGuidByCompanyNameAsync(string companyName)
        {
            var companyLogoVM = await _context.Company.Where(c => c.Name == companyName)
                                                  .Select(x => new CompanyLogoVM { 
                                                      CompanyId = x.Id, 
                                                      LogoGuid = x.LogoGuid
                                                  })
                                                  .SingleAsync();
            return companyLogoVM;
        }
    }
}
