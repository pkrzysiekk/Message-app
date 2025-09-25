using Message_Backend.Models;
using Message_Backend.Models.DTOs;

namespace Message_Backend.Mappers;

public static class ChatMapper
{
   public static Chat ToBo(this ChatDto dto)
   {
      return new Chat()
      {
         Name = dto.ChatName,
         Id = dto.Id,
      };
   }

   public static ChatDto ToDto(this Chat chat)
   {
      return new ChatDto()
      {
         Id = chat.Id,
         ChatName = chat.Name
      };
   }
}