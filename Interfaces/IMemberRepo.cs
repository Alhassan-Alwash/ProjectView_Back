using ProjectView.Models;

namespace ProjectView.Interfaces
{
    public interface IMemberRepo
    {
        Task<IEnumerable<Member>> GetMembersAsync();
        Task<Member> GetMemberAsync(Guid id);
        Task<bool> MemberExistsAsync(Guid id);
        Task<bool> CreateMemberAsync(Member member);
        Task<bool> UpdateMemberAsync(Member member);
        Task<bool> DeleteMemberAsync(Guid id);
        Task<bool> SaveAsync();
    }
}
