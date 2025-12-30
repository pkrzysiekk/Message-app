using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Application.Mappers;
using Message_Backend.Application.Models.DTOs;
using Message_Backend.Domain.Models.Enums;
using Message_Backend.Presentation.ApiRequests;
using Message_Backend.Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
           _groupService = groupService; 
        } 

        [HttpGet("{groupId}")]
        [Authorize(Policy = "GroupMember")]
        public async Task<ActionResult<GroupDto>> Get([FromRoute] int groupId)
        {
            var group = await _groupService.GetById(groupId);
            return Ok(group.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GroupDto group)
        {
            var userId = CookieHelper.GetUserIdFromCookie(User);
            var groupBo=group.ToBo();
            await _groupService.CreateGroup(groupBo,userId);
            return  CreatedAtAction(nameof(Get), new { groupId = groupBo.Id }, groupBo.ToDto());
        }

        [HttpPut]
        [Authorize(Policy = "RequireAdminRoleInGroup")]
        public async Task<IActionResult> Put([FromBody] GroupDto group)
        {
           var groupBo=group.ToBo();
           await _groupService.UpdateGroup(groupBo);
           return Ok();
        }

        [HttpDelete("{groupId}")]
        [Authorize(Policy = "RequireAdminRoleInGroup")]
        public async Task<IActionResult> Delete([FromRoute] int groupId)
        {
            await _groupService.Delete(groupId);
            return Ok();
        }

        [HttpGet("user-groups")]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetUserGroups()
        {
            var userId = CookieHelper.GetUserIdFromCookie(User);
            var userGroups= await _groupService.GetUserGroups(userId);
            var userGroupsDto = userGroups.Select(ug => ug.ToDto());
            return Ok(userGroupsDto);
        }

        [HttpPut("{userId}/add")]
        [Authorize(Policy = "RequireAdminRoleInGroup")]
        public async Task<ActionResult> AddUserToGroup
            ([FromRoute] int userId, [FromBody] UserGroupRoleRequest roleRequest)
        {
           await _groupService.AddUserToGroup(userId, roleRequest.GroupId, roleRequest.Role); 
           return Ok();
        }

        [HttpDelete("{userId}/remove")]
        [Authorize(Policy = "RequireAdminRoleInGroup")]
        public async Task<ActionResult> RemoveUserFromGroup([FromRoute] int userId, [FromQuery] int groupId)
        {
            await  _groupService.RemoveUserFromGroup(userId, groupId);
            return Ok();   
        }

        [HttpPut("{userId}/update-group-role")]
        [Authorize(Policy = "RequireAdminRoleInGroup")]
        public async Task<ActionResult> UpdateUserRole
            ([FromRoute] int userId,[FromBody] UserGroupRoleRequest request)
        {
            await _groupService.UpdateUserRoleInGroup(userId,request.GroupId, request.Role);
            return Ok();
        }

        [HttpGet("{groupId}/user-role")]
        [Authorize(Policy = "GroupMember")]
        public async Task<ActionResult<GroupRole?>> GetUserRole([FromRoute] int groupId)
        {
            var userId = CookieHelper.GetUserIdFromCookie(User);
            var userRole = await _groupService.GetUserRoleInGroup(userId, groupId);
            if (userRole == null)
                return NotFound();
            return Ok(userRole);
        }

        [HttpGet("{groupId}/members")]
        [Authorize(Policy = "GroupMember")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetGroupMembers([FromRoute] int groupId)
        {
            var users = await _groupService.GetUsersInGroup(groupId); 
            return Ok(users.Select(user => user.ToDto()));
        }
 
    }
}
