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
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(long id)
        {
            var message = await _messageService.GetById(id);
            return Ok(message.ToDto());
        }

        // POST api/<MessageController>
        [HttpPost]
        [Authorize(Policy = "UserIsSender")]
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
        [HttpPut("{id}")]
        public async Task<ActionResult> 
            Put([FromRoute] long id,[FromBody] MessageDto messageDto)
        {
            MessageContent messageContent = new MessageContent()
            {
                Data = messageDto.Content
            };
            await _messageService.Update(id,messageContent);
            return NoContent();
        }

        // DELETE api/<MessageController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] long id)
        {
            await _messageService.Delete(id);
            return NoContent();
        }
    }
}
