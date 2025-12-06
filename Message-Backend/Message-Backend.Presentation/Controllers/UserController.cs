using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Application.Mappers;
using Message_Backend.Application.Models.DTOs;
using Message_Backend.Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Presentation.Controllers
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
            var user = await _userService.GetById(id);
            return Ok(user.ToDto());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Search([FromQuery] string term)
        {
            var fetchedUsers=await _userService.SearchForUsers(term);
            var usersDto=fetchedUsers.Select(u=>u.ToDto());
            return Ok(usersDto);
        }

        // PUT api/<UserController>/5
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UserDto userDto)
        {
            var userId = CookieHelper.GetUserIdFromCookie(User);
            if (userDto.Id != userId)
                return BadRequest("User id mismatch");
            var user = userDto.ToBo();
            await _userService.Update(user);
            return Ok();
        }

        // DELETE api/<UserController>/5
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var userId = CookieHelper.GetUserIdFromCookie(User);
            await _userService.Delete(userId);
            return Ok("Deleted");
        }

        [HttpPut("change-password")]
        public async Task<ActionResult> ChangePassword(string oldPassword, string newPassword)
        {         
                var userId = CookieHelper.GetUserIdFromCookie(User);
                await _userService.ChangePassword(userId, oldPassword, newPassword);
                return Ok("Password changed");
        }

        [HttpPut("change-email")]
        public async Task<ActionResult> ChangeEmail( string email)
        {
            var userId = CookieHelper.GetUserIdFromCookie(User);
            await _userService.ChangeEmail(userId, email);
            return Ok("Email changed");
        }

        [HttpPut("change-avatar")]
        public async Task<ActionResult> ChangeAvatar(IFormFile avatar)
        {
            var userId = CookieHelper.GetUserIdFromCookie(User);
            await _userService.SetAvatar(userId,avatar);
            return Ok("Avatar changed");
        }

        [HttpGet("{userId}/avatar")]
        public async Task<ActionResult> GetAvatar([FromRoute] int userId)
        {
            var avatar = await _userService.GetAvatar(userId);
            return File(avatar.Content,avatar.ContentType);
        }
    }
}
