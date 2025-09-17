using Message_Backend.Mappers;
using Message_Backend.Models;
using Message_Backend.Models.DTOs;
using Message_Backend.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult>Post([FromBody] UserDto userDto,string password)
        {
            var user = userDto.ToBo();
            await _userService.Add(user, password);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToDto()); 
        }

        // PUT api/<UserController>/5
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UserDto userDto)
        {
            var user = userDto.ToBo();
            await _userService.Update(user);
            return Ok();
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.Delete(id);
            return Ok("Deleted");
        }

        [HttpPost("{id}/change-password")]
        public async Task<ActionResult> ChangePassword(int id, string oldPassword, string newPassword)
        {
                await _userService.ChangePassword(id, oldPassword, newPassword);
                return Ok("Password changed");
        }
    }
}
