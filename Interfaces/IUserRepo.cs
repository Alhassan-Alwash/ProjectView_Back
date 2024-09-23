using ProjectView.Dto.user;
using ProjectView.Models;

namespace ProjectView.Interfaces
{
    public interface IUserRepo
    {
        Task<UserDto> Register(RegisterationRequestDto registerationRequestDto);
        Task<LoginResponseDto>? Login(LoginRequestDto loginRequestDto);
        Task<bool> IsPasswordCorrect(RegisterationRequestDto registerationRequestDto);
        Task<bool> IsUniqueUser(string UserName);

        Task<bool> UserExistsAsync(Guid Id);

        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(Guid Id);
        Task<bool> DeleteUserAsync(Guid Id);
        Task<bool> UpdateUserAsync(Guid id, UserUpdateDto userUpdateDto);
        Task<bool> SaveAsync();

    }
}
