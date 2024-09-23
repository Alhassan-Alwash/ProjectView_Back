using Microsoft.EntityFrameworkCore;
using ProjectView.Data;
using ProjectView.Interfaces;
using ProjectView.Models;

namespace ProjectView.Repository
{
    public class MemberRepo : IMemberRepo
    {
        private readonly ApplicationDbContext _context;

        public MemberRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateMemberAsync(Member member)
        {
            _context.Members.Add(member);
            return await SaveAsync();
        }

        public async Task<bool> DeleteMemberAsync(Guid id)
        {
            var memberToDelete = _context.Members.Where(c => c.Id == id).FirstOrDefault();
            if (memberToDelete == null)
                return false;

            _context.Members.Remove(memberToDelete);
            return await SaveAsync();
        }

        public async Task<Member> GetMemberAsync(Guid id)
        {
            return _context.Members.Where(c => c.Id == id).FirstOrDefault();
        }

        public async Task<IEnumerable<Member>> GetMembersAsync()
        {
            return await _context.Members.ToListAsync();
        }

        public async Task<bool> MemberExistsAsync(Guid id)
        {
            return await _context.Members.AnyAsync(m => m.Id == id);
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

        public async Task<bool> UpdateMemberAsync(Member member)
        {
            _context.Members.Update(member);
            return await SaveAsync();
        }
    }
}
