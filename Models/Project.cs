using System.ComponentModel.DataAnnotations;

namespace ProjectView.Models
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string? Files { get; set; }

        public string State { get; set; }
        public ICollection<ProjectMember> ProjectMembers { get; set; }
        public ICollection<SubProject> SubProjects { get; set; }
    }
}
