using Message_Backend.Application.Models.DTOs;
using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Mappers;

public static class FriendsMapper
{
   public static FriendsDto ToDto(this Friends friends)
   {
      return new FriendsDto()
      {
         UserId = friends.UserId,
         FriendId = friends.FriendId,
         FriendsSince = friends.FriendsSince,
         Status = friends.Status,
      };
   }

   public static Friends ToBo(this FriendsDto friendsDto)
   {
      return new Friends()
      {
         UserId = friendsDto.UserId,
         FriendId = friendsDto.FriendId,
         FriendsSince = friendsDto.FriendsSince,
         Status = friendsDto.Status,
      };
   }
}