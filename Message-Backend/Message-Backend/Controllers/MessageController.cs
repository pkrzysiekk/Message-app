using Message_Backend.Mappers;
using Message_Backend.Models;
using Message_Backend.Models.DTOs;
using Message_Backend.Repository;
using Message_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Message_Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        // GET api/<MessageController>/5
        [HttpGet("{messageId}")]
        [Authorize(Policy = "CanReadMessage")]
        public async Task<ActionResult> Get(long messageId)
        {
            var message = await _messageService.GetById(messageId);
            return Ok(message.ToDto());
        }

        // POST api/<MessageController>
        [HttpPost]
        [Authorize(Policy = "UserHasRequiredChatRole")]
        public async Task<ActionResult> Post([FromBody] MessageDto messageDto)
        {
            var message = messageDto.ToBo();
            MessageContent messageContent = new MessageContent()
            {
                Data = messageDto.Content
            };
            
            await _messageService.Add(message, messageContent);
            return CreatedAtAction(nameof(Get), new { id = message.Id }, message);

        }

        // PUT api/<MessageController>/5
        [HttpPut("{messageId}")]
        [Authorize(Policy = "UserIsSender")]
        public async Task<ActionResult> 
            Put([FromRoute] long messageId,[FromBody] MessageDto messageDto)
        {
            MessageContent messageContent = new MessageContent()
            {
                Data = messageDto.Content
            };
            await _messageService.Update(messageId,messageContent);
            return NoContent();
        }

        // DELETE api/<MessageController>/5
        [HttpDelete("{messageId}")]
        [Authorize(Policy = "UserCanDelete")]
        public async Task<ActionResult> Delete([FromRoute] long messageId)
        {
            await _messageService.Delete(messageId);
            return NoContent();
        }
    }
}
