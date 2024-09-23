using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectView.Dto;
using ProjectView.Dto.subProject;
using ProjectView.Interfaces;
using ProjectView.Models;
using System.Net;

namespace ProjectView.Controllers
{
    [Route("API/[Controller]")]
    [ApiController]
    public class SubProjectCtrl : ControllerBase
    {
        protected APIResponse _response;
        private readonly ISubProjectRepo _subProjectRepo;
        private readonly IMapper _mapper;


        public SubProjectCtrl(ISubProjectRepo subProjectRepo, IMapper mapper)
        {
            _subProjectRepo = subProjectRepo;
            _response = new();
            _mapper = mapper;


        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetSubProjects()
        {
            try
            {
                IEnumerable<SubProject> subProjects = await _subProjectRepo.GetSubProjectsAsync();
                _response.Result = subProjects; // No mapping required for now
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

        [HttpGet("{id:Guid}", Name = "GetSubProject")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetSubProject(Guid id)
        {
            try
            {
                if (id == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var subProject = await _subProjectRepo.GetSubProjectAsync(id);
                if (subProject == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = subProject; // No mapping required for now
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
        public async Task<ActionResult<APIResponse>> CreateSubProject([FromBody] SubProjectCreateDto subProjectCreateDto)
        {
            try
            {
                if (subProjectCreateDto == null)
                {
                    return BadRequest(subProjectCreateDto);
                }

                var subProjectEntity = _mapper.Map<SubProject>(subProjectCreateDto);


                await _subProjectRepo.CreateSubProjectAsync(subProjectEntity);
                _response.Result = subProjectEntity; // No mapping required for now
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetSubProject", new { id = subProjectEntity.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpDelete("{id:Guid}", Name = "DeleteSubProject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteSubProject(Guid id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest();
                }

                var subProject = await _subProjectRepo.GetSubProjectAsync(id);

                if (subProject == null)
                {
                    return NotFound();
                }
                await _subProjectRepo.DeleteSubProjectAsync(id);

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
        [HttpPut("{id:Guid}", Name = "UpdateSubProject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateSubProject([FromRoute] Guid id, [FromBody] SubProjectUpdateDto subProjectUpdateDto)
        {
            try
            {
                var existingSubProject = await _subProjectRepo.GetSubProjectAsync(id);

                if (subProjectUpdateDto == null || existingSubProject == null)
                {
                    return BadRequest();
                }

                var subProjectEntity = new SubProject
                {
                    Id = existingSubProject.Id, // Ensure the ID remains the same
                    ProjectId = existingSubProject.ProjectId, // Keep the foreign key unchanged
                    ProjectVersion = subProjectUpdateDto.ProjectVersion,
                    Notes = subProjectUpdateDto.Notes,
                    StartDate = subProjectUpdateDto.StartDate,
                    EndDate = subProjectUpdateDto.EndDate,
                };

                await _subProjectRepo.UpdateSubProjectAsync(subProjectEntity);

                _response.Result = subProjectEntity; // No mapping required for now

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
