using Microsoft.EntityFrameworkCore;
using ProjectView.Data;
using ProjectView.Interfaces;
using ProjectView.Models;

namespace ProjectView.Repository
{
    public class RoleRepo : IRoleRepo
    {
        private readonly ApplicationDbContext _context;

        public RoleRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateRoleAsync(Role role)
        {
            _context.Roles.Add(role);
            return await SaveAsync();
        }

        public async Task<bool> DeleteRoleAsync(Guid id)
        {
            var roleToDelete = _context.Roles.Where(r => r.Id == id).FirstOrDefault();
            if (roleToDelete == null)
                return false;

            _context.Roles.Remove(roleToDelete);
            return await SaveAsync();
        }

        public async Task<Role> GetRoleAsync(Guid id)
        {
            return await _context.Roles.Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<bool> RoleExistsAsync(Guid id)
        {
            return await _context.Roles.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateRoleAsync(Role role)
        {
            _context.Roles.Update(role);
            return await SaveAsync();
        }
    }
}
