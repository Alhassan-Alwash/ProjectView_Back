namespace ProjectView.Dto.subProject
{
    public class SubProjectCreateDto
    {
        public string Notes { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProjectVersion { get; set; }
        public Guid ProjectId { get; set; }
    }
}
