using ProjectView.Models;

namespace ProjectView.Interfaces
{
    public interface IRoleRepo
    {
        Task<IEnumerable<Role>> GetRolesAsync();
        Task<Role> GetRoleAsync(Guid Id);
        Task<bool> RoleExistsAsync(Guid Id);
        Task<bool> CreateRoleAsync(Role role);
        Task<bool> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(Guid Id);
        Task<bool> SaveAsync();
    }
}
