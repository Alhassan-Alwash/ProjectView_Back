namespace ProjectView.Dto.project
{
    public class ProjectCreateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public required string State { get; set; }


    }
}
