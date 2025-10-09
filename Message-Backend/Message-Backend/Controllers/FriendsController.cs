using Message_Backend.Mappers;
using Message_Backend.Models;
using Message_Backend.Models.DTOs;
using Message_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Controllers
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
            var friends = await _friendsService.FindById(userId, friendId);
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
        public async Task<ActionResult<IEnumerable<FriendsDto>>>
            GetUserPendingInvites([FromRoute] int userId)
        {
            var invites = await _friendsService.GetAllUserPendingInvites(userId);
            var invitesDto = invites.Select(x => x.ToDto()).ToList();
            return invitesDto;
        }
        
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] FriendsDto friendsDto)
        {
            await _friendsService.SendInvite(friendsDto.UserId,friendsDto.FriendId);
            return CreatedAtAction(nameof(Get), new { id = friendsDto.FriendId }, friendsDto);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] FriendsDto friendsDto)
        {
            var friends=friendsDto.ToBo();
            await _friendsService.Update(friends);
            return Ok("Friends updated");
        }

        [HttpDelete("/{userId}/{friendId}")]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult> Delete([FromRoute] int userId,[FromRoute] int friendId)
        {
            await _friendsService.Delete(userId, friendId);
            return Ok("Friends deleted");
        }
    }
}
