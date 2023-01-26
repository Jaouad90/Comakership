using AutoMapper;
using DAL;
using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UniversityService : IUniversityService
    {
        private IUniversityRepository _universityRepository;
        private readonly IMapper _mapper;

        public UniversityService(IUniversityRepository universityRepository, IMapper mapper)
        {
            _universityRepository = universityRepository;
            _mapper = mapper;
        }
        public async Task<bool> CheckIfUniversityExistsAsync(int id)
        {
            return await _universityRepository.CheckIfUniversityExistsAsync(id);
        }

        public async Task<bool> CheckIfUniversitynameExistsAsync(string name)
        {   
            return await _universityRepository.CheckIfUniversitynameExistsAsync(name);
        }

        public async Task<bool> DeleteUniversityByIdAsync(int? id)
        {
            return await _universityRepository.DeleteUniversityByIdAsync((int)id);
        }

        public async Task<bool> EditUniversityAsync(UniversityPutVM universityPutVM)
        {
            University university = _mapper.Map<University>(universityPutVM);
            return await _universityRepository.EditUniversityAsync(university);
        }

        public async Task<IEnumerable<University>> GetAllUniversitiesAsync()
        {
            return await _universityRepository.GetAllUniversitiesAsync();
        }

        public async Task<IEnumerable<UniversityDomainVM>> GetAllUniversityDomainVMAsync()
        {
            IEnumerable<University> universityList =  await _universityRepository.GetAllUniversityDomainVMAsync();
            IEnumerable<UniversityDomainVM> universityDomainVMList = _mapper.Map<IEnumerable<UniversityDomainVM>>(universityList);
            return universityDomainVMList;
        }

        public async Task<University> GetUniversityByIdAsync(int? id)
        {
            return await _universityRepository.GetUniversityByIdAsync(id);
        }

        public async Task<UniversityDomainVM> GetUniversityDomainByIdAsync(int? id)
        {
            return await _universityRepository.GetUniversityDomainByIdAsync(id);
        }

        public async Task<bool> SaveUniversityAsync(UniversityPostVM universityPostVM)
        {
            University university = _mapper.Map<University>(universityPostVM);
            university.RegistrationDate = DateTime.Now;

            return await _universityRepository.SaveUniversityAsync(university);
        }
    }
}
