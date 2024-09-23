using ProjectView.Dto.project;
using ProjectView.Models;

namespace ProjectView.Interfaces
{
    public interface IProjectRepo
    {
        Task<ICollection<ProjectDto>> GetProjectsAsync(ProjectSearchDto searchCriteria);
        Task<Project> GetProjectAsync(Guid Id);
        Task<ProjectDto> GetProjectDetails(Guid Id);
        Task<bool> ProjectExistsAsync(Guid Id);
        Task<bool> UpdateProjectAsync(Project project, List<IFormFile> files);
        Task<bool> DeleteProjectAsync(Guid Id);
        Task<bool> SaveAsync();
        Task<List<ProjectCountDto>> GetProjectStatusCounts();
        Task<bool> CreateProjectAsync(Project project, List<IFormFile> Files, SubProject subProject, List<ProjectMember> projectMember);
    }
}
