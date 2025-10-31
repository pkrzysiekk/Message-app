using Message_Backend.Models;
using Message_Backend.Models.DTOs;

namespace Message_Backend.Mappers;

public static class MessageMapper
{
    public static Message ToBo(this MessageDto messageDto)
    {
        return new Message()
        {
            Id = messageDto.MessageId,
            SenderId = messageDto.SenderId,
            ChatId = messageDto.ChatId,
            Status = messageDto.Status,
            Type = messageDto.Type,
            Content = new MessageContent()
            {
                Data = messageDto.Content
            }
        };
    }
    
    public static MessageDto ToDto(this Message message)
    {
        return new MessageDto()
        {
            MessageId = message.Id,
            SenderId = message.SenderId,
            ChatId = message.ChatId,
            Status = message.Status,
            Type = message.Type,
            Content = message.Content.Data,
        };
    }
}