using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectView.Dto;
using ProjectView.Dto.member;
using ProjectView.Interfaces;
using ProjectView.Models;
using System.Net;

namespace ProjectView.Controllers
{
    [Route("API/[Controller]")]
    [ApiController]
    public class MemberCtrl : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMemberRepo _dbMember;

        public MemberCtrl(IMemberRepo dbMember)
        {
            _dbMember = dbMember;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetMembers()
        {
            try
            {
                IEnumerable<Member> memberList = await _dbMember.GetMembersAsync();
                _response.Result = memberList; // No mapping required for now
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

        [HttpGet("{id:Guid}", Name = "GetMember")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetMember(Guid id)
        {
            try
            {
                if (id == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var member = await _dbMember.GetMemberAsync(id);
                if (member == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = member; // No mapping required for now
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
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateMember([FromBody] MemberCreateDto memberCreateDto)
        {
            try
            {
                if (memberCreateDto == null)
                {
                    return BadRequest(memberCreateDto);
                }

                Member member = new Member
                {
                    // Map properties from DTO if needed
                    Name = memberCreateDto.Name
                };

                await _dbMember.CreateMemberAsync(member);
                _response.Result = member; // No mapping required for now
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetMember", new { id = member.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpDelete("{id:Guid}", Name = "DeleteMember")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteMember(Guid id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest();
                }

                var member = await _dbMember.GetMemberAsync(id);

                if (member == null)
                {
                    return NotFound();
                }
                await _dbMember.DeleteMemberAsync(id);

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
        [HttpPut("{id:Guid}", Name = "UpdateMember")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateMember([FromRoute] Guid id, [FromBody] MemberUpdateDto memberUpdateDto)
        {
            try
            {
                var existingMember = await _dbMember.GetMemberAsync(id);

                if (memberUpdateDto == null || existingMember == null)
                {
                    return BadRequest();
                }


                // Map properties from DTO if needed
                existingMember.Name = memberUpdateDto.Name;


                await _dbMember.UpdateMemberAsync(existingMember);

                _response.Result = existingMember; // No mapping required for now

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
