using ProjectView.Dto.member;
using ProjectView.Dto.role;

namespace ProjectView.Dto.projectMember
{
    public class ProjectMemberDto
    {
        public Guid Id { get; set; }


        public Guid ProjectId { get; set; }
        public Guid RoleId { get; set; }
        public Guid MemberId { get; set; }
        /*public Member Member { get; set; }

        public Project Project { get; set; }
        public Role Role { get; set; }*/
        public MemberDto Member { get; set; } // Add this property
        public RoleDto Role { get; set; } // Add this property

    }
}
