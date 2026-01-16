using Message_Backend.Application.Models.DTOs;
using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Mappers;

public static class UserChatMapper
{
   public static UserChat ToBo(this UserChatDto dto)
   {
      return new UserChat()
      {
         UserId = dto.UserId,
         ChatId = dto.ChatId,
         LastMessageId = dto.LastMessageId,
         LastReadAt = dto.LastReadAt,
      };
   }

   public static UserChatDto ToDto(this UserChat userChat)
   {
      return new UserChatDto()
      {
         UserId = userChat.UserId,
         ChatId = userChat.ChatId,
         LastMessageId = userChat.LastMessageId,
         LastReadAt = userChat.LastReadAt
      };
   }
}