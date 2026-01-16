using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Application.Mappers;
using Message_Backend.Application.Models.DTOs;
using Message_Backend.Domain.Entities;
using Message_Backend.Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUserChatService _userChatService;
        public ChatController(IChatService chatService, IUserChatService userChatService)
        {
            _chatService = chatService;
            _userChatService = userChatService;
        }
        
        [HttpGet("{chatId}")]
        [Authorize(Policy = "UserHasRequiredRoleInGroup")]
        public async Task<ActionResult<ChatDto>> Get([FromRoute] int chatId)
        {
            var chat = await _chatService.GetById(chatId);
            return Ok(chat.ToDto());
        }

        [HttpPost]
        [Authorize(Policy = "CanCreateChatWithProvidedRole")]
        public async Task<ActionResult> Post([FromBody] ChatDto chatDto)
        {
            var chat = chatDto.ToBo();
             await _chatService.AddChatToGroup(chat);
            return CreatedAtAction(nameof(Get), new { chatId = chat.Id }, chat.ToDto());
        }

        [HttpPut]
        [Authorize(Policy = "CanCreateChatWithProvidedRole")]
        public async Task<ActionResult> Put([FromBody] ChatDto chatDto)
        {
            var chat = chatDto.ToBo();
            await _chatService.Update(chat);
            return Ok();
        }

        // DELETE api/<ChatController>/5
        [HttpDelete("{chatId}")]
        [Authorize(Policy = "UserHasRequiredRoleInGroup")]
        public async Task<ActionResult> Delete([FromRoute] int chatId)
        {
            await _chatService.Delete(chatId);
            return Ok();
        }

        [HttpGet("group/{groupId}")]
        [Authorize(Policy = "GroupMember")]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetAllGroupChats([FromRoute] int groupId)
        {
            var chats =await  _chatService.GetAllGroupChats(groupId);
            var chatsDto=chats.Select(x=>x.ToDto()).ToList();
            return Ok(chatsDto);
        }

        [HttpGet("user-chats")]
        [Authorize(Policy = "SameUser")]
        //For Development Only
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetAllUserChats()
        {
            var userId = CookieHelper.GetUserIdFromCookie(User);
            var userChats=await _chatService.GetUserChats(userId);
            return Ok(userChats.Select(x=>x.ToDto()).ToList());
        }

        [HttpGet("{groupId}/chats")]
        [Authorize(Policy = "GroupMember")]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetUserChatsInGroup([FromRoute] int groupId)
        {
           var userId = CookieHelper.GetUserIdFromCookie(User);
           var userChatsInGroup= await _chatService.GetUserChatsInGroup(userId, groupId);
           return Ok(userChatsInGroup.Select(c=>c.ToDto()).ToList());
        }

        [HttpPut("user-chats/{userId}")]
        [Authorize(Policy = "GroupMember")]
        public async Task<ActionResult>
            UpdateUserChats([FromRoute] int userId, [FromBody] UserChatDto userChatDto)
        {
            var userChat = userChatDto.ToBo();
            await _userChatService.Update(userChat);
            return Ok();
        }
    }
}
