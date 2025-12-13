using Message_Backend.Application.Helpers;
using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Application.Mappers;
using Message_Backend.Application.Models.DTOs;
using Message_Backend.Domain.Models.RSA;
using Message_Backend.Presentation.ApiRequests;
using Message_Backend.Presentation.Atributes;
using Message_Backend.Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Presentation.Controllers
{
    [Route("api/[controller]")]

    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly JwtOptions _jwtOptions;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _jwtOptions=RsaHelper.JwtOptions;
        }
        
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> LogIn([FromBody] UserAuthorizationRequest request)
        { 
            string unauthorizedMessageError="Wrong username or password";
            bool authenticationResult = await _authService.ValidateUserCredentials(request.Username, request.Password);
            if (!authenticationResult)
                return Unauthorized(unauthorizedMessageError);
            var token = await _authService.GenerateToken(_jwtOptions,request.Username);
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,         
                SameSite = SameSiteMode.None
            });

            return Ok(await _authService.GetUserId(request.Username));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            await _authService.RegisterUser(request.Username, request.Password,request.Email);
            return Ok();
        }

    }
}
