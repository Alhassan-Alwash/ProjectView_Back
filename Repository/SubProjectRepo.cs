using Microsoft.EntityFrameworkCore;
using ProjectView.Data;
using ProjectView.Interfaces;
using ProjectView.Models;

namespace ProjectView.Repository
{
    public class SubProjectRepo : ISubProjectRepo
    {
        private readonly ApplicationDbContext _context;

        public SubProjectRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateSubProjectAsync(SubProject subProject)
        {
            try
            {
                _context.SubProjects.Add(subProject);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                // Handle exceptions
                return false;
            }
        }

        public async Task<bool> DeleteSubProjectAsync(Guid Id)
        {
            try
            {
                var subProject = await _context.SubProjects.FindAsync(Id);
                if (subProject == null)
                    return false;

                _context.SubProjects.Remove(subProject);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                // Handle exceptions
                return false;
            }
        }

        public async Task<SubProject> GetSubProjectAsync(Guid Id)
        {
            return await _context.SubProjects.FindAsync(Id);
        }

        public async Task<IEnumerable<SubProject>> GetSubProjectsAsync()
        {
            return await _context.SubProjects.ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SubProjectExistsAsync(Guid Id)
        {
            return await _context.SubProjects.AnyAsync(sp => sp.Id == Id);
        }

        public async Task<bool> UpdateSubProjectAsync(SubProject subProject)
        {
            try
            {
                var existingSubProject = await _context.SubProjects.FindAsync(subProject.Id);
                if (existingSubProject == null)
                {
                    return false;
                }

                // Manually update the properties
                existingSubProject.Notes = subProject.Notes;
                existingSubProject.StartDate = subProject.StartDate;
                existingSubProject.EndDate = subProject.EndDate;
                existingSubProject.ProjectVersion = subProject.ProjectVersion;

                _context.Entry(existingSubProject).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                // Handle exceptions
                return false;
            }
        }
    }
}
