using Microsoft.EntityFrameworkCore;
using ProjectView.Data;
using ProjectView.Interfaces;
using ProjectView.Models;

namespace ProjectView.Repository
{
    public class ProjectMemberRepo : IProjectMemberRepo
    {
        private readonly ApplicationDbContext _context;

        public ProjectMemberRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateProjectMemberAsync(ProjectMember projectMember)
        {
            try
            {
                _context.ProjectMembers.Add(projectMember);
                await SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProjectMemberAsync(Guid id)
        {
            try
            {

                var projectMember = await _context.ProjectMembers.FindAsync(id);
                if (projectMember == null)
                    return false;

                _context.ProjectMembers.Remove(projectMember);
                await SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ProjectMember> GetProjectMemberAsync(Guid id)
        {
            return await _context.ProjectMembers.FindAsync(id);
        }

        public async Task<IEnumerable<ProjectMember>> GetProjectMembersAsync()
        {
            return await _context.ProjectMembers.ToListAsync();
        }

        public async Task<bool> ProjectMemberExistsAsync(Guid id)
        {
            return await _context.ProjectMembers.AnyAsync(pm => pm.Id == id);
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

        public async Task<bool> UpdateProjectMemberAsync(ProjectMember projectMember)
        {
            try
            {
                var existingProjectMember = await _context.ProjectMembers.FindAsync(projectMember.Id);
                if (existingProjectMember == null)
                {
                    return false;
                }

                existingProjectMember.MemberId = projectMember.MemberId;
                existingProjectMember.RoleId = projectMember.RoleId;

                _context.Entry(projectMember).State = EntityState.Modified;
                await SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
