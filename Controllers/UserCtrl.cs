using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectView.Dto;
using ProjectView.Dto.user;
using ProjectView.Interfaces;
using ProjectView.Models;
using System.Net;

namespace ProjectView.Controllers
{
    [Route("API/[Controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        protected APIResponse _response;

        public UserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
            _response = new APIResponse();
        }

        [Authorize]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Register([FromBody] RegisterationRequestDto registerationRequestDto)
        {
            try
            {
                if (registerationRequestDto == null)
                {
                    return BadRequest(registerationRequestDto);
                }

                UserDto registeredUser = await _userRepo.Register(registerationRequestDto);
                _response.Result = registeredUser;
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetUserById", new { id = registeredUser.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                if (loginRequestDto == null)
                {
                    return BadRequest(loginRequestDto);
                }

                LoginResponseDto loginResponseDto = await _userRepo.Login(loginRequestDto);
                if (loginResponseDto == null)
                {
                    return Unauthorized();
                }

                _response.Result = loginResponseDto;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetUserById(Guid id)
        {
            try
            {
                User user = await _userRepo.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                _response.Result = user;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetUsers()
        {
            try
            {
                IEnumerable<User> users = await _userRepo.GetUsersAsync();
                if (users == null)
                {
                    return NotFound();
                }

                _response.Result = users;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteUser(Guid id)
        {
            try
            {
                bool result = await _userRepo.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<ActionResult<APIResponse>> UpdateUser([FromRoute] Guid id, [FromBody] UserUpdateDto user)
        {
            try
            {
                var existingUser = await _userRepo.GetUserByIdAsync(id);

                if (existingUser == null)
                {
                    return NotFound();
                }

                if (id != existingUser.Id)
                {
                    return BadRequest();
                }

                bool result = await _userRepo.UpdateUserAsync(id, user);
                if (!result)
                {
                    return NotFound();
                }

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}
