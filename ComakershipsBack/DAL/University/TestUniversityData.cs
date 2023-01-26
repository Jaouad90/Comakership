//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AutoMapper;
//using Models;
//using Models.ViewModels;

//namespace DAL
//{
//    public class TestUniversityData : IUniversityData
//    {
//        private List<University> universityList;
//        private List<UniversityDomainVM> UniversityDomainVMList;

//        private readonly IMapper _mapper;

//        public TestUniversityData(IMapper mapper)
//        {
//            _mapper = mapper;

//            // neat way of initializing the lists, otherwise this ctor was rather full.
//            initializeBogusUniversityList();
//            initializeBogusUniversityDomainVMList();
//        }
//        private void initializeBogusUniversityList()
//        {
//            universityList = new List<University>();

//            University u1 = new University
//            {
//                Id = 66,
//                Name = "Inholland",
//                Street = "Bijdorplaan 15",
//                City = "Haarlem",
//                Zipcode = "2015CE",
//                RegistrationDate = new DateTime(637328304000000000),
//                Domain = "student.inholland.nl"
//            };

//            University u2 = new University
//            {
//                Id = 124,
//                Name = "HvA Kohnstammhuis",
//                Street = "Wibautstraat 2-4",
//                City = "Amsterdam",
//                Zipcode = "1091GM",
//                RegistrationDate = new DateTime(637328232000000000),
//                Domain = "student.hva.nl"
//            };

//            universityList.Add(u1);
//            universityList.Add(u2);
//        }

//        private void initializeBogusUniversityDomainVMList()
//        {
//            UniversityDomainVMList = new List<UniversityDomainVM>();

//            UniversityDomainVM u1 = new UniversityDomainVM()
//            {
//                Id = 66,
//                Name = "Hogeschool Inholland",
//                Domain = "student.inholland.nl"
//            };

//            UniversityDomainVM u2 = new UniversityDomainVM()
//            {
//                Id = 123,
//                Name = "Hogeschool van Amsterdam",
//                Domain = "student.hva.nl"
//            };

//            UniversityDomainVM u3 = new UniversityDomainVM()
//            {
//                Id = 99,
//                Name = "Zweinstein",
//                Domain = "hocuspocus.zweinstein.co.uk"
//            };

//            UniversityDomainVMList.Add(u1);
//            UniversityDomainVMList.Add(u2);
//            UniversityDomainVMList.Add(u3);
//        }

//        // Models
//        public IEnumerable<University> GetAllUniversities()
//        {
//            return universityList.AsEnumerable();
//        }

//        public University GetUniversityById(int id)
//        {
//            return universityList.Find(x => x.Id == id);
//        }

//        public async Task<University> EditUniversity(UniversityPutVM universityPutVM)
//        {
//            // TODO write DB implementation, currently returning the object.
//            return universityList.Find(universityPutVM => universityPutVM.Id == universityPutVM.Id);
//        }

//        public async Task<University> DeleteUniversityById(int id)
//        {
//            // TODO write DB implementation, currently returning the requested to be deleted object.
//            return universityList.Find(x => x.Id == id);
//        }


//        // ViewModels
//        public IEnumerable<UniversityDomainVM> GetAllUniversityDomainVM()
//        {
//            return _mapper.Map<List<UniversityDomainVM>>(universityList);
//        }

//        public UniversityDomainVM GetUniversityDomainById(int id)
//        {
//            return UniversityDomainVMList.Find(x => x.Id == id);
//        }

//        public List<UniversityDomainVM> GetUniversityDomainVMList()
//        {
//            return _mapper.Map<List<UniversityDomainVM>>(universityList);
//        }

//        public Task<UniversityPostVM> SaveUniversity(UniversityPostVM universityPost)
//        {
//            // TODO write DB implementation
//            throw new NotImplementedException();
//        }
//    }
//}
