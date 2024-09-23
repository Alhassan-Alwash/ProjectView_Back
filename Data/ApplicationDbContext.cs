using Microsoft.EntityFrameworkCore;
using ProjectView.Models;

namespace ProjectView.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Member> Members { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SubProject> SubProjects { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //-- SubProject --//

            modelBuilder.Entity<SubProject>()
                 .HasOne(s => s.Project)
                 .WithMany(p => p.SubProjects)
                 .HasForeignKey(s => s.ProjectId)
                 .IsRequired();


            //-- Project Member --//

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Member)
                .WithMany(m => m.ProjectMembers)
                .HasForeignKey(pm => pm.MemberId);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.ProjectMembers)
                .HasForeignKey(pm => pm.ProjectId);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Role)
                .WithMany(r => r.ProjectMembers)
                .HasForeignKey(pm => pm.RoleId);

            //-- Project (for cascade deletion) --//


            modelBuilder.Entity<Project>()
                .HasMany(p => p.SubProjects)
                .WithOne(sp => sp.Project)
                .HasForeignKey(sp => sp.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectMembers)
                .WithOne(pm => pm.Project)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}