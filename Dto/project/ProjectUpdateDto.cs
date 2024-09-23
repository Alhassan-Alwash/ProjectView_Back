namespace ProjectView.Dto.project
{
    public class ProjectUpdateWImgDto
    {

        public ProjectUpdateDto Project { get; set; }
        public List<IFormFile>? Files { get; set; }


    }

    public class ProjectUpdateDto
    {
        public string Name { get; set; }
        public string Notes { get; set; }
        public required string State { get; set; }
    }
}
