using Message_Backend.Mappers;
using Message_Backend.Models;
using Message_Backend.Models.DTOs;
using Message_Backend.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private  readonly IFriendsService  _friendsService;

        public FriendsController(IFriendsService friendsService)
        {
           _friendsService = friendsService; 
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<FriendsDto>> Get([FromRoute] int id)
        {
            var friends = await _friendsService.FindById(id);
            return friends.ToDto();
        }

        [HttpGet("/users/{userId}/friends")]
        public async Task<ActionResult<IEnumerable<FriendsDto>>> GetFriends([FromRoute] int userId)
        {
            var friends = await _friendsService.GetAllUserFriends(userId);
            var friendsDto=friends.Select(x=>x.ToDto()).ToList();
            return friendsDto;
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _friendsService.Delete(id);
            return Ok("Friends deleted");
        }
    }
}
