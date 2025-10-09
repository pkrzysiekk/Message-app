using Message_Backend.Mappers;
using Message_Backend.Models;
using Message_Backend.Models.DTOs;
using Message_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
           _userService = userService; 
        }
        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(int id)
        {
            var user = await _userService.Get(id);
            return Ok(user.ToDto());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Search([FromQuery] string term)
        {
            var fetchedUsers=await _userService.SearchForUsers(term);
            var usersDto=fetchedUsers.Select(u=>u.ToDto());
            return Ok(usersDto);
        }

        // POST api/<UserController>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult>Post([FromBody] UserDto userDto,string password)
        {
            var user = userDto.ToBo();
            await _userService.Add(user, password);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToDto()); 
        }

        // PUT api/<UserController>/5
        [HttpPut("{userId}")]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult> Put([FromRoute] int userId,[FromBody] UserDto userDto)
        {
            if (userDto.Id != userId)
                return BadRequest("User id mismatch");
            var user = userDto.ToBo();
            await _userService.Update(user);
            return Ok();
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{userId}")]
        [Authorize(Policy = "SameUser")]
        public async Task<IActionResult> Delete([FromRoute] int userId)
        {
            await _userService.Delete(userId);
            return Ok("Deleted");
        }

        [HttpPut("{userId}/change-password")]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult> ChangePassword([FromRoute] int userId, string oldPassword, string newPassword)
        {
                await _userService.ChangePassword(userId, oldPassword, newPassword);
                return Ok("Password changed");
        }

        [HttpPut("{userId}/change-email")]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult> ChangeEmail([FromRoute]  int userId, string email)
        {
            await _userService.ChangeEmail(userId, email);
            return Ok("Email changed");
        }

        [HttpPut("{userId}/change-avatar")]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult> ChangeAvatar([FromRoute] int userId,  IFormFile avatar)
        {
            await _userService.SetAvatar(userId,avatar);
            return Ok("Avatar changed");
        }

        [HttpGet("{userId}/avatar")]
        [Authorize(Policy = "SameUser")]
        public async Task<ActionResult> GetAvatar([FromRoute] int userId)
        {
            var avatar = await _userService.GetAvatar(userId);
            return File(avatar.Content,avatar.ContentType);
        }
    }
}
