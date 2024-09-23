using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectView.Dto;
using ProjectView.Dto.project;
using ProjectView.Interfaces;
using ProjectView.Models;
using System.Net;


namespace ProjectView.Controllers
{
    [Route("API/[Controller]")]
    [ApiController]
    public class ProjectCtrl : ControllerBase
    {
        private readonly IProjectRepo _dbProject;
        private readonly APIResponse _response;
        private readonly IMapper _mapper;


        public ProjectCtrl(IProjectRepo dbProject, IMapper mapper)
        {
            _dbProject = dbProject;
            _response = new APIResponse();
            _mapper = mapper;
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> SearchProjects([FromQuery] ProjectSearchDto searchCriteria)
        {
            try
            {
                var projectList = await _dbProject.GetProjectsAsync(searchCriteria);
                if (projectList == null || !projectList.Any())
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "No projects found matching the search criteria." };
                    return NotFound(_response);
                }

                _response.Result = projectList;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
        [HttpGet("{id:Guid}", Name = "GetProject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetProject(Guid id)
        {
            try
            {
                var project = await _dbProject.GetProjectDetails(id);
                if (project == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = project;
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

        [HttpGet("status-counts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetProjectStatusCounts()
        {
            try
            {
                var statusCounts = await _dbProject.GetProjectStatusCounts();
                _response.Result = statusCounts;
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
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("multipart/form-data")]
        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateProject([FromForm] ProjectWImgDto projectWImgDto, [FromForm] PWsubProjectDto subProjectCreateDto, [FromForm] List<string> projectMemberCreateList)
        {
            try
            {
                var projectMemberCreate = new List<PWprojectMemberDto>();
                foreach (var x in projectMemberCreateList)
                {
                    var projectMemberCreateJson = JsonConvert.DeserializeObject<PWprojectMemberDto>(x);
                    if (projectMemberCreateJson != null)
                    {
                        projectMemberCreate.Add(projectMemberCreateJson);
                    }
                }

                if (projectWImgDto == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var projectEntity = _mapper.Map<Project>(projectWImgDto.Project);
                var subProjectEntity = _mapper.Map<SubProject>(subProjectCreateDto);
                var projectMemberEntity = _mapper.Map<List<ProjectMember>>(projectMemberCreate);



                // Create the project with image files
                bool result = await _dbProject.CreateProjectAsync(projectEntity, projectWImgDto.Files, subProjectEntity, projectMemberEntity);
                if (result)
                {
                    _response.Result = projectEntity;
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetProject", new { id = projectEntity.Id }, _response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { "Failed to create project." };
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }




        [Authorize]
        [HttpDelete("{id:Guid}", Name = "DeleteProject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteProject(Guid id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest();
                }

                var project = await _dbProject.GetProjectAsync(id);

                if (project == null)
                {
                    return NotFound();
                }
                await _dbProject.DeleteProjectAsync(id);

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

        [Authorize]
        [HttpPut("{id:Guid}", Name = "UpdateProject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateProject([FromRoute] Guid id, [FromForm] ProjectUpdateWImgDto projectUpdateDto)
        {
            try
            {
                var existingProject = await _dbProject.GetProjectAsync(id);

                if (projectUpdateDto == null || existingProject == null)
                {
                    return BadRequest();
                }

                // Map the updated DTO to the existing project entity
                _mapper.Map(projectUpdateDto.Project, existingProject);

                // Update the project
                bool result = await _dbProject.UpdateProjectAsync(existingProject, projectUpdateDto.Files);

                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "Failed to update project." };
                    return BadRequest(_response);
                }

                _response.Result = existingProject;
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}
