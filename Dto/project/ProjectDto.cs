using ProjectView.Dto.projectMember;
using ProjectView.Dto.subProject;

namespace ProjectView.Dto.project
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }

        public string Files { get; set; }

        public string State { get; set; }

        public ICollection<ProjectMemberDto> ProjectMembers { get; set; }

        public ICollection<SubProjectDto> SubProjects { get; set; }
    }
}
