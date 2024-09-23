namespace ProjectView.Dto.project
{
    public class ProjectWImgDto
    {
        public ProjectCreateDto Project { get; set; }

        public List<IFormFile>? Files { get; set; }


    }


    public class PWsubProjectDto
    {
        public string Notes { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProjectVersion { get; set; }

    }

    public class PWprojectMemberDto
    {
        public Guid MemberId { get; set; }
        public Guid RoleId { get; set; }
    }

}


