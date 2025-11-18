using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Application.Mappers;
using Message_Backend.Application.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private  readonly IFriendsService  _friendsService;

        public FriendsController(IFriendsService friendsService)
        {
           _friendsService = friendsService; 
        }
        
        [HttpGet]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult<FriendsDto>> Get([FromQuery] int userId,[FromQuery]int friendId)
        {
            var friends = await _friendsService.GetFriendsByUserIds(userId, friendId);
            return friends.ToDto();
        }

        [HttpGet("/users/{userId}/friends")]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult<IEnumerable<FriendsDto>>> GetFriends([FromRoute] int userId)
        {
            var friends = await _friendsService.GetAllUserFriends(userId);
            var friendsDto=friends.Select(x=>x.ToDto()).ToList();
            return friendsDto;
        }

        [HttpGet("/users/{userId}/invites")]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult<IEnumerable<FriendsDto>>>
            GetUserPendingInvites([FromRoute] int userId)
        {
            var invites = await _friendsService.GetAllUserPendingInvites(userId);
            var invitesDto = invites.Select(x => x.ToDto()).ToList();
            return invitesDto;
        }
        
        [HttpPost]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult> 
            Invite([FromQuery] int userId,[FromBody] FriendsDto friendsDto)
        {
            if(userId != friendsDto.UserId)
                return BadRequest("UserId does not match");
            await _friendsService.SendInvite(friendsDto.UserId,friendsDto.FriendId);
            return CreatedAtAction(nameof(Get), new { id = friendsDto.FriendId }, friendsDto);
        }

        [HttpPut("/acceptInvite")]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult> AcceptInvite([FromQuery] int userId,[FromQuery] int friendId)
        {
            await _friendsService.AcceptInvite(userId, friendId);
            return Ok("Friends updated");
        }
        [HttpPut("/declineInvite")]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult> DeclineInvite([FromQuery] int userId,[FromQuery] int friendId)
        {
            await _friendsService.DeclineInvite(userId, friendId);
            return Ok("Friends updated");
        } 

        [HttpDelete("/deleteFriend")]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult> RemoveFriend([FromQuery] int userId,[FromQuery] int friendId)
        {
            await _friendsService.RemoveFriend(userId, friendId);
            return Ok("Friends deleted");
        }
    }
}
