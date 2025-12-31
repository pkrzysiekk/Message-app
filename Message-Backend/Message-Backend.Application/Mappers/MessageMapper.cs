using Message_Backend.Application.Models.DTOs;
using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Mappers;

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
            SenderName = message?.Sender.UserName,
            ChatId = message.ChatId,
            Status = message.Status,
            SentAt = message.SentAt,
            Type = message.Type,
            Content = message.Content.Data,
        };
    }
}