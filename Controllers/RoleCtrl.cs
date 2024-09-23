using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectView.Dto;
using ProjectView.Dto.role;
using ProjectView.Interfaces;
using ProjectView.Models;
using System.Net;

namespace ProjectView.Controllers
{
    [Route("API/[Controller]")]
    [ApiController]
    public class RoleCtrl : ControllerBase
    {
        protected APIResponse _response;
        private readonly IRoleRepo _dbRole;

        public RoleCtrl(IRoleRepo dbRole)
        {
            _dbRole = dbRole;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetRoles()
        {
            try
            {
                IEnumerable<Role> roleList = await _dbRole.GetRolesAsync();
                _response.Result = roleList; // No mapping required for now
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

        [HttpGet("{id:Guid}", Name = "GetRole")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetRole(Guid id)
        {
            try
            {
                if (id == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var role = await _dbRole.GetRoleAsync(id);
                if (role == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = role; // No mapping required for now
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
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateRole([FromBody] RoleCreateDto roleCreateDto)
        {
            try
            {
                if (roleCreateDto == null)
                {
                    return BadRequest(roleCreateDto);
                }

                Role role = new Role
                {
                    // Map properties from DTO if needed
                    Name = roleCreateDto.Name
                };

                await _dbRole.CreateRoleAsync(role);
                _response.Result = role; // No mapping required for now
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetRole", new { id = role.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpDelete("{id:Guid}", Name = "DeleteRole")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteRole(Guid id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest();
                }

                var role = await _dbRole.GetRoleAsync(id);

                if (role == null)
                {
                    return NotFound();
                }
                await _dbRole.DeleteRoleAsync(id);

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
        [HttpPut("{id:Guid}", Name = "UpdateRole")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateRole([FromRoute] Guid id, [FromBody] RoleUpdateDto roleUpdateDto)
        {
            try
            {
                var existingRole = await _dbRole.GetRoleAsync(id);

                if (roleUpdateDto == null || existingRole == null)
                {
                    return BadRequest();
                }

                // Map properties from DTO if needed
                existingRole.Name = roleUpdateDto.Name;

                await _dbRole.UpdateRoleAsync(existingRole);

                _response.Result = existingRole; // No mapping required for now

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
    }
}
