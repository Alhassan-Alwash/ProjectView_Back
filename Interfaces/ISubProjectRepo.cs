using ProjectView.Models;

namespace ProjectView.Interfaces
{
    public interface ISubProjectRepo
    {
        Task<IEnumerable<SubProject>> GetSubProjectsAsync();
        Task<SubProject> GetSubProjectAsync(Guid Id);
        Task<bool> SubProjectExistsAsync(Guid Id);
        Task<bool> CreateSubProjectAsync(SubProject subProject);
        Task<bool> UpdateSubProjectAsync(SubProject subProject);
        Task<bool> DeleteSubProjectAsync(Guid Id);
        Task<bool> SaveAsync();
    }
}
