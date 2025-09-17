using Message_Backend.Models;
using Message_Backend.Models.DTOs;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Message_Backend.Mappers;

public static class AvatarMapper
{
   public static AvatarDto ToDto(this Avatar avatar)
   {
      return new AvatarDto()
      {
         UserId = avatar.UserId,
         Content = avatar.Content,
      };
   }

   public static Avatar FromDto(this AvatarDto avatarDto)
   {
      return new Avatar()
      {
         UserId = avatarDto.UserId,
         Content = avatarDto.Content,
      };
   }
}