using AutoMapper;
using ProjectView.Dto.member;
using ProjectView.Dto.project;
using ProjectView.Dto.projectMember;
using ProjectView.Dto.role;
using ProjectView.Dto.subProject;
using ProjectView.Models;


namespace ProjectView
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Member, MemberDto>().ReverseMap();
            CreateMap<Member, MemberCreateDto>().ReverseMap();
            CreateMap<Member, MemberUpdateDto>().ReverseMap();

            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<Role, RoleCreateDto>().ReverseMap();
            CreateMap<Role, RoleUpdateDto>().ReverseMap();

            CreateMap<Project, ProjectDto>().ReverseMap();
            CreateMap<Project, ProjectCreateDto>().ReverseMap();
            CreateMap<Project, ProjectUpdateDto>().ReverseMap();
            CreateMap<Project, ProjectWImgDto>().ReverseMap();
            CreateMap<Project, ProjectUpdateWImgDto>().ReverseMap();

            CreateMap<SubProject, SubProjectDto>().ReverseMap();
            CreateMap<SubProject, SubProjectCreateDto>().ReverseMap();
            CreateMap<SubProject, SubProjectUpdateDto>().ReverseMap();
            CreateMap<SubProject, PWsubProjectDto>().ReverseMap();

            CreateMap<ProjectMember, ProjectMemberDto>().ReverseMap();
            CreateMap<ProjectMember, ProjectMemberCreateDto>().ReverseMap();
            CreateMap<ProjectMember, ProjectMemberUpdateDto>().ReverseMap();
            CreateMap<ProjectMember, PWprojectMemberDto>().ReverseMap();

            CreateMap<ProjectMember, ProjectWImgDto>().ReverseMap();






        }
    }
}
