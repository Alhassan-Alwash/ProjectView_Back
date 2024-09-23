using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectView.Data;
using ProjectView.Dto.user;
using ProjectView.Interfaces;
using ProjectView.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectView.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly string _secretKey;

        public UserRepo(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<bool> DeleteUserAsync(Guid Id)
        {
            var user = await _context.Users.FindAsync(Id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> GetUserByIdAsync(Guid Id)
        {
            return await _context.Users.FindAsync(Id);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> IsPasswordCorrect(RegisterationRequestDto registerationRequestDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == registerationRequestDto.UserName);
            return user != null && user.Password == registerationRequestDto.Password;
        }

        public async Task<bool> IsUniqueUser(string userName)
        {
            bool isUnique = await _context.Users.AllAsync(u => u.UserName != userName);


            return isUnique;
        }



        public async Task<UserDto> Register(RegisterationRequestDto registerationRequestDto)
        {


            var user = new User
            {
                UserName = registerationRequestDto.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(registerationRequestDto.Password), // You might want to hash the password here
                FullName = registerationRequestDto.FullName,
                Role = registerationRequestDto.Role // Set the role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto { Id = user.Id, UserName = user.UserName, Role = user.Role, Password = user.Password }; // Return UserDto with role
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UserExistsAsync(Guid Id)
        {
            return await _context.Users.AnyAsync(u => u.Id == Id);
        }

        public async Task<bool> UpdateUserAsync(Guid id, UserUpdateDto userUpdateDto)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return false;
            }

            existingUser.UserName = userUpdateDto.UserName;
            if (!string.IsNullOrEmpty(userUpdateDto.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(userUpdateDto.Password);
            }
            existingUser.FullName = userUpdateDto.FullName;
            existingUser.Role = userUpdateDto.Role;

            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            // Find the user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            // If user is not found or password does not match, return null
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.Password))
            {
                return null;
            }

            // Create the JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.UserName),
                    // Add other claims as needed
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new LoginResponseDto
            {
                User = user.UserName, // Map User to UserDto
                Roles = user.Role,
                Token = tokenString
            };
        }
    }
}
