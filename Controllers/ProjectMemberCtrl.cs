using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectView.Dto;
using ProjectView.Dto.projectMember;
using ProjectView.Interfaces;
using ProjectView.Models;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectView.Controllers
{
    [Route("API/[Controller]")]
    [Authorize]
    [ApiController]
    public class ProjectMemberCtrl : ControllerBase
    {
        private readonly IProjectMemberRepo _dbProjectMember;
        private readonly IMemberRepo _dbMember;
        private readonly IRoleRepo _dbRole;
        private readonly IProjectRepo _dbProject;
        private readonly APIResponse _response;
        private readonly IMapper _mapper;
        private readonly JsonSerializerOptions _jsonOptions;



        public ProjectMemberCtrl(IMapper mapper, IProjectMemberRepo dbProjectMember, IMemberRepo dbMemberRepo, IRoleRepo dbRoleRepo, IProjectRepo dbProjectRepo)
        {
            _dbProjectMember = dbProjectMember;
            _dbMember = dbMemberRepo;
            _dbRole = dbRoleRepo;
            _dbProject = dbProjectRepo;
            _mapper = mapper;
            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };



            _response = new APIResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetProjectMembers()
        {
            try
            {


                var projectMemberList = await _dbProjectMember.GetProjectMembersAsync();
                _response.Result = _mapper.Map<List<ProjectMemberDto>>(projectMemberList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = _response.Result,
                    ErrorMessages = new List<string>()
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpGet("{id:Guid}", Name = "GetProjectMember")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetProjectMember(Guid id)
        {
            try
            {
                if (id == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var projectMember = await _dbProjectMember.GetProjectMemberAsync(id);
                if (projectMember == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = projectMember;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateProjectMember([FromBody] ProjectMemberCreateDto projectMemberCreateDto)
        {
            try
            {
                if (projectMemberCreateDto == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                // Validate if the provided IDs for Member, Project, and Role exist
                bool memberExists = await _dbMember.MemberExistsAsync(projectMemberCreateDto.MemberId);
                bool projectExists = await _dbProject.ProjectExistsAsync(projectMemberCreateDto.ProjectId);
                bool roleExists = await _dbRole.RoleExistsAsync(projectMemberCreateDto.RoleId);

                if (!memberExists || !projectExists || !roleExists)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Invalid member, project, or role ID." };
                    return BadRequest(_response);
                }

                var projectMember = new ProjectMember
                {
                    // Map properties from DTO
                    MemberId = projectMemberCreateDto.MemberId,
                    ProjectId = projectMemberCreateDto.ProjectId,
                    RoleId = projectMemberCreateDto.RoleId
                };

                await _dbProjectMember.CreateProjectMemberAsync(projectMember);
                _response.Result = projectMember;
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetProjectMember", new { id = projectMember.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }


        [HttpDelete("{id:Guid}", Name = "DeleteProjectMember")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteProjectMember(Guid id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest();
                }

                var projectMember = await _dbProjectMember.GetProjectMemberAsync(id);

                if (projectMember == null)
                {
                    return NotFound();
                }
                await _dbProjectMember.DeleteProjectMemberAsync(id);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPut("{id:Guid}", Name = "UpdateProjectMember")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateProjectMember([FromRoute] Guid id, [FromBody] ProjectMemberUpdateDto projectMemberUpdateDto)
        {
            try
            {
                var existingProjectMember = await _dbProjectMember.GetProjectMemberAsync(id);

                if (projectMemberUpdateDto == null || existingProjectMember == null)
                {
                    return BadRequest();
                }

                // Map properties from DTO if needed
                existingProjectMember.MemberId = projectMemberUpdateDto.MemberId;
                existingProjectMember.RoleId = projectMemberUpdateDto.RoleId;

                await _dbProjectMember.UpdateProjectMemberAsync(existingProjectMember);

                _response.Result = existingProjectMember;
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}
