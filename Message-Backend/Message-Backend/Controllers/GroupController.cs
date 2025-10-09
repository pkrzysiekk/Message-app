using Message_Backend.Mappers;
using Message_Backend.Models;
using Message_Backend.Models.DTOs;
using Message_Backend.Models.Enums;
using Message_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Controllers
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

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> Get(int id)
        {
            var group = await _groupService.GetGroup(id);
            return Ok(group.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserGroupRequest request)
        {
            var groupBo=request.GroupDto.ToBo();
            await _groupService.CreateGroup(groupBo,request.userId);
            return  CreatedAtAction(nameof(Get), new { id = groupBo.Id }, groupBo.ToDto());
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] GroupDto group)
        {
           var groupBo=group.ToBo();
           await _groupService.UpdateGroup(groupBo);
           return Ok("Group updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _groupService.DeleteGroup(id);
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
        public async Task<ActionResult> AddUserToGroup
            ([FromRoute] int userId, [FromBody] UserGroupRoleRequest roleRequest)
        {
           await _groupService.AddUserToGroup(userId, roleRequest.GroupId, roleRequest.Role); 
           return Ok("User added to group");
        }

        [HttpDelete("{userId}/remove-user-from-group")]
        public async Task<ActionResult> RemoveUserFromGroup([FromRoute] int userId, [FromQuery] int groupId)
        {
            await  _groupService.RemoveUserFromGroup(userId, groupId);
            return Ok("User removed from group");   
        }

        [HttpPut("{userId}/update-group-role")]
        public async Task<ActionResult> UpdateUserRole
            ([FromRoute] int userId,[FromBody] UserGroupRoleRequest request)
        {
            await _groupService.UpdateUserRoleInGroup(userId,request.GroupId, request.Role);
            return Ok("User role updated");
        }
    }
}
