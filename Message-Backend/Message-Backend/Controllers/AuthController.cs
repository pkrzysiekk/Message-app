using Message_Backend.Helpers;
using Message_Backend.Mappers;
using Message_Backend.Models;
using Message_Backend.Models.DTOs;
using Message_Backend.Models.RSA;
using Message_Backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly JwtOptions _jwtOptions;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _jwtOptions=RsaHelper.LoadJwtOptions();
        }
        
        [HttpPost("/auth")]
        public async Task<ActionResult<string>> Post([FromBody] UserAuthorizationRequest request)
        { 
            bool authenticationResult = await _authService.ValidateUserCredentials(request.Username, request.Password);
            if (!authenticationResult)
                return Unauthorized();
            var token = await _authService.GenerateToken(_jwtOptions,request.Username);
            return Ok(token);
        }
    }
}
