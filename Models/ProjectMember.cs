using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectView.Models
{
    public class ProjectMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }




        [ForeignKey("Member")]
        public Guid MemberId { get; set; }
        public Member Member { get; set; }

        [ForeignKey("Project")]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

        [ForeignKey("Role")]
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
