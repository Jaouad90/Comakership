using AutoMapper;
using Models;
using Models.ViewModels;
using System;
using System.Linq;

namespace ComakershipsApi.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {

            // comakership
            CreateMap<Comakership, ComakershipBasic>()
                .ForMember(cc => cc.Skills, c => c.MapFrom(c => c.LinkedSkills.Select(ls => ls.Skill)))
                .ForMember(cc => cc.Programs, c => c.MapFrom(c => c.LinkedPrograms.Select(lp => lp.Program)));
            CreateMap<Comakership, ComakershipComplete>()
                .ForMember(cc => cc.Skills, c => c.MapFrom(c => c.LinkedSkills.Select(ls => ls.Skill)))
                .ForMember(cc => cc.Programs, c => c.MapFrom(c => c.LinkedPrograms.Select(lp => lp.Program)))
                .ForMember(cc => cc.Students, c => c.MapFrom(c => c.StudentUsers.Select(su => su.StudentUser)));
            CreateMap<ComakershipPost, Comakership>();
            CreateMap<ComakershipPut, Comakership>();

            // TeamComakership
            CreateMap<TeamComakership, TeamComakershipTeamGet>();
            CreateMap<TeamComakership, TeamComakershipComakershipGet>();

            // team
            CreateMap<Team, TeamBasic>();
            CreateMap<TeamPost, Team>();
            CreateMap<TeamPut, Team>();
            CreateMap<StudentUser, StudentView>();
            CreateMap<Team, TeamComplete>()
               .ForMember(tc => tc.Members, t => t.MapFrom(t => t.LinkedStudents.Select(ls => ls.StudentUser)));

            // todo
            CreateMap<Deliverable, DeliverableGet>();
            CreateMap<DeliverablePost, Deliverable>();
            CreateMap<DeliverablePut, Deliverable>();

            // status
            CreateMap<ComakershipStatus, ComakershipStatusGet>();
            CreateMap<ComakershipStatusPost, ComakershipStatus>();
            CreateMap<ComakershipStatusPut, ComakershipStatus>();

            // Company
            CreateMap<Company, CompanyPostVM>().ReverseMap();         // map LogoGuid only if it's not a Guid.Empty
            CreateMap<Company, CompanyPutVM>().ReverseMap().ForMember(c => c.LogoGuid, opt => opt.Condition(src => (src.LogoGuid != Guid.Empty))).ForAllOtherMembers(opt => opt.Condition((CompanyPutVM, Company, sourceMember, destMember) => sourceMember != null));
            CreateMap<Company, CompanyVM>();

            // CompanyLogo
            CreateMap<Company, CompanyLogoVM>()
                    .ForMember(c => c.CompanyId, cc => cc.MapFrom(cc => cc.Id))
                    .ForMember(c => c.LogoGuid, cc => cc.MapFrom(cc => cc.LogoGuid));
            CreateMap<Company, CompanyLogoPostVM>();
            CreateMap<CompanyLogoPostVM, CompanyPutVM>();
            CreateMap<CompanyLogoPostVM, CompanyLogoVM>();

            // University
            CreateMap<University, UniversityPutVM>().ReverseMap().ForAllMembers(opt  => opt.Condition((UniversityPutVM, University, sourceMember, destMember) => (sourceMember != null)));
            CreateMap<University, UniversityDomainVM>();
            CreateMap<University, UniversityPostVM>().ReverseMap();

            // skill
            CreateMap<Skill, SkillGet>();
            CreateMap<SkillPost, Skill>();
            CreateMap<SkillPut, Skill>();

            // studentteam
            CreateMap<StudentTeamUpdate, StudentTeam>();

            //user
            CreateMap<StudentPostVM, StudentUser>().ForMember(
                destination => destination.Name,
                options => options.MapFrom(source => source.FirstName + " " + source.LastName)
            );
            CreateMap<StudentUser, StudentPutVM>().ReverseMap().ForAllMembers(opt => opt.Condition((StudentPutVM, StudentUser, sourceMember, destMember) => sourceMember != null));
            CreateMap<CompanyUserPostVM, CompanyUser>();
            CreateMap<CompanyUserVM, CompanyUser>().ReverseMap();
            CreateMap<CompanyUserPutVM, CompanyUser>().ReverseMap().ForAllMembers(opt => opt.Condition((StudentPutVM, StudentUser, sourceMember, destMember) => sourceMember != null));

            // program
            CreateMap<Program, ProgramGet>();
            CreateMap<ProgramPost, Program>();
            CreateMap<ProgramPut, Program>();

            // purchasekey
            CreateMap<PurchaseKeyPost, PurchaseKey>();

            // joinrequest
            CreateMap<JoinRequest, JoinRequestGet>();

            // teaminvite
            CreateMap<TeamInvite, TeamInviteGet>();

            // Review
            CreateMap<ReviewPostVM, Review>();
        }
    }
}