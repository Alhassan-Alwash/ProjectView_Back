using ProjectView.Models;

namespace ProjectView.Interfaces
{
    public interface IProjectMemberRepo
    {
        Task<IEnumerable<ProjectMember>> GetProjectMembersAsync();
        Task<ProjectMember> GetProjectMemberAsync(Guid Id);
        Task<bool> ProjectMemberExistsAsync(Guid Id);
        Task<bool> CreateProjectMemberAsync(ProjectMember projectMember);
        Task<bool> UpdateProjectMemberAsync(ProjectMember projectMember);
        Task<bool> DeleteProjectMemberAsync(Guid Id);
        Task<bool> SaveAsync();
    }
}
