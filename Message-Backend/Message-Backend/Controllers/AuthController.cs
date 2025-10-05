using Message_Backend.Mappers;
using Message_Backend.Models;
using Message_Backend.Models.DTOs;
using Message_Backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("/auth")]
        public ActionResult<string> Post([FromBody] UserDto userDto)
        {
            var user = userDto.ToBo();
            var token = _authService.GenerateToken(user);
            return Ok(token);
        }
    }
}
