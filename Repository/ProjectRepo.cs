using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectView.Data;
using ProjectView.Dto.member;
using ProjectView.Dto.project;
using ProjectView.Dto.projectMember;
using ProjectView.Dto.role;
using ProjectView.Dto.subProject;
using ProjectView.Interfaces;
using ProjectView.Models;
using System.Text.RegularExpressions;


namespace ProjectView.Repository
{
    public class ProjectRepo : IProjectRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;


        public ProjectRepo(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        public async Task<bool> CreateProjectAsync(Project project, List<IFormFile> files, SubProject subProject, List<ProjectMember> projectMembers)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (files != null && files.Count > 0)
                    {
                        var allowedExtensionsPattern = @"\.jpeg$|\.jpg$|\.png$|\.gif$|\.pdf$|\.doc$|\.docx$";
                        string projectId = project.Id.ToString();

                        // List to store unique file names
                        List<string> uniqueFileNames = new List<string>();

                        foreach (var file in files)
                        {
                            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}"; // Generate unique file name with GUID and original file extension
                            var projectPath = Path.Combine("wwwroot", "Files", "ProjectImages", projectId);
                            if (!Directory.Exists(projectPath))
                            {
                                Directory.CreateDirectory(projectPath);
                            }

                            // Check if the file extension is allowed
                            if (Regex.IsMatch(uniqueFileName, allowedExtensionsPattern, RegexOptions.IgnoreCase))
                            {
                                var savePath = Path.Combine(projectPath, uniqueFileName);
                                using (var fileStream = new FileStream(savePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                }
                                uniqueFileNames.Add(uniqueFileName); // Add unique file name to the list
                            }
                            else
                            {
                                // Handle invalid file extension
                                Console.WriteLine("Invalid file extension for file: " + file.FileName);
                            }
                        }

                        // Concatenate unique file names into a single string separated by commas
                        string filesString = string.Join(",", uniqueFileNames);

                        // Set the Files property of the project entity with the concatenated file names
                        project.Files = filesString;
                    }

                    // Set the foreign key relationship
                    _context.Projects.Add(project);

                    subProject.ProjectId = project.Id;
                    _context.SubProjects.Add(subProject);

                    foreach (var projectMember in projectMembers)
                    {
                        projectMember.ProjectId = project.Id;
                        _context.ProjectMembers.Add(projectMember);
                    }

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex);
                    return false;
                }
            }
        }
        public async Task<bool> UpdateProjectAsync(Project project, List<IFormFile> files)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (files != null && files.Count > 0)
                    {
                        var allowedExtensionsPattern = @"\.jpeg$|\.jpg$|\.png$|\.gif$|\.pdf$|\.doc$|\.docx$";
                        string projectId = project.Id.ToString();

                        // Path to the project directory
                        var projectPath = Path.Combine("wwwroot", "Files", "ProjectImages", projectId);

                        // Delete existing files in the project directory
                        if (Directory.Exists(projectPath))
                        {
                            var existingFiles = Directory.GetFiles(projectPath);
                            foreach (var file in existingFiles)
                            {
                                File.Delete(file);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(projectPath);
                        }

                        // List to store unique file names
                        List<string> uniqueFileNames = new List<string>();

                        foreach (var file in files)
                        {
                            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}"; // Generate unique file name with GUID and original file extension

                            // Check if the file extension is allowed
                            if (Regex.IsMatch(uniqueFileName, allowedExtensionsPattern, RegexOptions.IgnoreCase))
                            {
                                var savePath = Path.Combine(projectPath, uniqueFileName);
                                using (var fileStream = new FileStream(savePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                }
                                uniqueFileNames.Add(uniqueFileName); // Add unique file name to the list
                            }
                            else
                            {
                                // Handle invalid file extension
                                Console.WriteLine("Invalid file extension for file: " + file.FileName);
                            }
                        }

                        // Concatenate unique file names into a single string separated by commas
                        string filesString = string.Join(",", uniqueFileNames);

                        // Update the Files property of the project entity with the concatenated file names
                        project.Files = filesString;
                    }

                    // Update the project entity
                    _context.Entry(project).State = EntityState.Modified;

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex);
                    return false;
                }
            }
        }
        public async Task<bool> DeleteProjectAsync(Guid id)
        {
            var projectToDelete = await _context.Projects.FindAsync(id);
            if (projectToDelete == null)
                return false;

            var projectId = projectToDelete.Id.ToString(); // Convert project ID to string

            _context.Projects.Remove(projectToDelete);
            var deleted = await SaveAsync();

            if (deleted)
            {
                var projectFolderPath = Path.Combine("wwwroot", "Files", "ProjectImages", projectId);

                if (Directory.Exists(projectFolderPath))
                {
                    Directory.Delete(projectFolderPath, true); // Delete the project folder recursively
                }
            }

            return deleted;
        }

        public async Task<Project> GetProjectAsync(Guid id)
        {
            return await _context.Projects.FindAsync(id);
        }
        public async Task<ProjectDto> GetProjectDetails(Guid id)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.Member)
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.Role)
                .Include(p => p.SubProjects)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return null;
            }

            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Notes = project.Notes,
                Files = project.Files,
                State = project.State,
                ProjectMembers = project.ProjectMembers.Select(pm => new ProjectMemberDto
                {
                    Id = pm.Id,
                    MemberId = pm.MemberId,
                    ProjectId = pm.ProjectId,
                    RoleId = pm.RoleId,
                    Member = new MemberDto
                    {
                        Id = pm.Member.Id,
                        Name = pm.Member.Name
                        // Map other properties as needed
                    },
                    Role = new RoleDto
                    {
                        Id = pm.Role.Id,
                        Name = pm.Role.Name
                        // Map other properties as needed
                    }
                }).ToList(),
                SubProjects = project.SubProjects.Select(sp => new SubProjectDto
                {
                    Id = sp.Id,
                    Notes = sp.Notes,
                    StartDate = sp.StartDate,
                    EndDate = sp.EndDate,
                    ProjectVersion = sp.ProjectVersion,
                    ProjectId = sp.ProjectId,
                }).ToList()
            };

            return projectDto;
        }


        public async Task<ICollection<ProjectDto>> GetProjectsAsync(ProjectSearchDto searchCriteria)
        {
            var query = _context.Projects.AsQueryable();

            if (!string.IsNullOrEmpty(searchCriteria.Query))
            {
                var searchQuery = searchCriteria.Query.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchQuery) ||
                                         p.State.ToLower().Contains(searchQuery));
            }

            var projects = await query
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.Member)
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.Role)
                .Include(p => p.SubProjects)
                .ToListAsync();

            var projectDtos = projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Notes = p.Notes,
                Files = p.Files,
                State = p.State,
                ProjectMembers = p.ProjectMembers.Select(pm => new ProjectMemberDto
                {
                    Id = pm.Id,
                    MemberId = pm.MemberId,
                    ProjectId = pm.ProjectId,
                    RoleId = pm.RoleId,
                    Member = new MemberDto
                    {
                        Id = pm.Member.Id,
                        Name = pm.Member.Name
                        // Map other properties as needed
                    },
                    Role = new RoleDto
                    {
                        Id = pm.Role.Id,
                        Name = pm.Role.Name
                        // Map other properties as needed
                    }
                }).ToList(),
                SubProjects = p.SubProjects.Select(sp => new SubProjectDto
                {
                    Id = sp.Id,
                    Notes = sp.Notes,
                    StartDate = sp.StartDate,
                    EndDate = sp.EndDate,
                    ProjectVersion = sp.ProjectVersion,
                    ProjectId = sp.ProjectId,
                }).ToList()
            }).ToList();

            return projectDtos;
        }

        public async Task<bool> ProjectExistsAsync(Guid id)
        {
            return await _context.Projects.AnyAsync(p => p.Id == id);
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

        public async Task<List<ProjectCountDto>> GetProjectStatusCounts()
        {
            try
            {
                var statusCounts = await _context.Projects
                    .GroupBy(p => p.State)
                    .Select(g => new ProjectCountDto
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                return statusCounts;
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // LogError("GetProjectStatusCounts", ex);

                return new List<ProjectCountDto>(); // Return an empty list in case of an error
            }
        }
    }
}
