using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Application.Mappers;
using Message_Backend.Application.Models.DTOs;
using Message_Backend.Presentation.Helpers;
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
        public async Task<ActionResult<FriendsDto>> Get([FromQuery]int friendId)
        {
            int userId = CookieHelper.GetUserIdFromCookie(User);
            var friends = await _friendsService.GetFriendsByUserIds(userId, friendId);
            return friends.ToDto();
        }

        [HttpGet("friends")]
        public async Task<ActionResult<IEnumerable<FriendsDto>>> GetFriends()
        {
            int  userId = CookieHelper.GetUserIdFromCookie(User);
            var friends = await _friendsService.GetAllUserFriends(userId);
            var friendsDto=friends.Select(x=>x.ToDto()).ToList();
            return friendsDto;
        }

        [HttpGet("invites")]
        public async Task<ActionResult<IEnumerable<FriendsDto>>>
            GetUserPendingInvites()
        {
            int userId= CookieHelper.GetUserIdFromCookie(User);
            var invites = await _friendsService.GetAllUserPendingInvites(userId);
            var invitesDto = invites.Select(x => x.ToDto()).ToList();
            return invitesDto;
        }
        
        [HttpPost]
        public async Task<ActionResult> 
            Invite([FromBody] FriendsDto friendsDto)
        {
            int  userId = CookieHelper.GetUserIdFromCookie(User);
            await _friendsService.SendInvite(friendsDto.UserId,friendsDto.FriendId);
            return CreatedAtAction(nameof(Get), new { id = friendsDto.FriendId }, friendsDto);
        }

        [HttpPut("acceptInvite")]
        public async Task<ActionResult> AcceptInvite([FromQuery] int friendId)
        {
            int userId = CookieHelper.GetUserIdFromCookie(User);
            await _friendsService.AcceptInvite(userId, friendId);
            return Ok("Friends updated");
        }
        [HttpPut("declineInvite")]
        public async Task<ActionResult> DeclineInvite([FromQuery] int friendId)
        {
            int userId = CookieHelper.GetUserIdFromCookie(User);
            await _friendsService.DeclineInvite(userId, friendId);
            return Ok("Friends updated");
        } 

        [HttpDelete("deleteFriend")]
        public async Task<ActionResult> RemoveFriend([FromQuery] int friendId)
        {
            int userId = CookieHelper.GetUserIdFromCookie(User);
            await _friendsService.RemoveFriend(userId, friendId);
            return Ok("Friends deleted");
        }
    }
}
