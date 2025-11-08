using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Application.Mappers;
using Message_Backend.Application.Models.DTOs;
using Message_Backend.Presentation.ApiRequests;
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
        public async Task<ActionResult<GroupDto>> Get(int groupId)
        {
            var group = await _groupService.GetById(groupId);
            return Ok(group.ToDto());
        }

        [HttpPost]
        [Authorize(Policy = "UserCreatesGroupForThemselves")]
        public async Task<ActionResult> Post([FromBody] UserGroupRequest request)
        {
            var groupBo=request.GroupDto.ToBo();
            await _groupService.CreateGroup(groupBo,request.userId);
            return  CreatedAtAction(nameof(Get), new { groupId = groupBo.Id }, groupBo.ToDto());
        }

        [HttpPut]
        [Authorize(Policy = "RequireAdminRoleInGroup")]
        public async Task<IActionResult> Put([FromBody] GroupDto group)
        {
           var groupBo=group.ToBo();
           await _groupService.UpdateGroup(groupBo);
           return Ok("Group updated");
        }

        [HttpDelete("{groupId}")]
        [Authorize(Policy = "RequireAdminRoleInGroup")]
        public async Task<IActionResult> Delete(int groupId)
        {
            await _groupService.Delete(groupId);
            return Ok("Group deleted");
        }

        [HttpGet("{userId}/user-groups")]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetUserGroups
            ([FromRoute] int userId,[FromQuery] int page,[FromQuery] int pageSize)
        {
            var userGroups= await _groupService.GetPaginatedUserGroups(userId,page,pageSize);
            var userGroupsDto = userGroups.Select(ug => ug.ToDto());
            return Ok(userGroupsDto);
        }

        [HttpPut("{userId}/add-user-to-group")]
        [Authorize(Policy = "RequireAdminRoleInGroup")]
        public async Task<ActionResult> AddUserToGroup
            ([FromRoute] int userId, [FromBody] UserGroupRoleRequest roleRequest)
        {
           await _groupService.AddUserToGroup(userId, roleRequest.GroupId, roleRequest.Role); 
           return Ok("User added to group");
        }

        [HttpDelete("{userId}/remove-user-from-group")]
        [Authorize(Policy = "RequireAdminRoleInGroup")]
        public async Task<ActionResult> RemoveUserFromGroup([FromRoute] int userId, [FromQuery] int groupId)
        {
            await  _groupService.RemoveUserFromGroup(userId, groupId);
            return Ok("User removed from group");   
        }

        [HttpPut("{userId}/update-group-role")]
        [Authorize(Policy = "RequireAdminRoleInGroup")]
        public async Task<ActionResult> UpdateUserRole
            ([FromRoute] int userId,[FromBody] UserGroupRoleRequest request)
        {
            await _groupService.UpdateUserRoleInGroup(userId,request.GroupId, request.Role);
            return Ok("User role updated");
        }
    }
}
