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
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ChatDto>> Get([FromRoute] int id)
        {
            var chat = await _chatService.Get(id);
            return Ok(chat.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ChatDto chatDto)
        {
            var chat = chatDto.ToBo();
            await _chatService.AddChatToGroup(chat);
            return CreatedAtAction(nameof(Get), new { id = chat.Id }, chat.ToDto());
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] ChatDto chatDto)
        {
            var chat = chatDto.ToBo();
            await _chatService.Update(chat);
            return Ok();
        }

        // DELETE api/<ChatController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _chatService.Delete(id);
            return Ok();
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetAllGroupChats([FromRoute] int groupId)
        {
            var chats =await  _chatService.GetAllGroupChats(groupId);
            var chatsDto=chats.Select(x=>x.ToDto()).ToList();
            return Ok(chatsDto);
        }
        
    }
}
