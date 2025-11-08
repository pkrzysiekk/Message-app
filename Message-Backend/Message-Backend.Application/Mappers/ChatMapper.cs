using Message_Backend.Application.Models.DTOs;
using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Mappers;

public static class ChatMapper
{
   public static Chat ToBo(this ChatDto dto)
   {
      return new Chat()
      {
         Id = dto.Id,
         Name = dto.ChatName,
         GroupId = dto.GroupId,
         ForRole = dto.ForRole,
      };
   }

   public static ChatDto ToDto(this Chat chat)
   {
      return new ChatDto()
      {
         Id = chat.Id,
         ChatName = chat.Name,
         GroupId = chat.GroupId,
         ForRole = chat.ForRole,
      };
   }
}