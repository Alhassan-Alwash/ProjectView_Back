namespace ProjectView.Dto.projectMember
{
    public class ProjectMemberCreateDto
    {
        public Guid MemberId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid RoleId { get; set; }
    }
}
