using Message_Backend.Application.Models.DTOs;
using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user) =>
        new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            Username = user.UserName!,
            LastSeen = user.LastSeen,
            IsOnline = user.IsOnline,
            Avatar= user.Avatar != null ? new AvatarDto()
            {
                Content = user.Avatar.Content,
                ContentType = user.Avatar.ContentType,
            } :  null
        };

    public static User ToBo(this UserDto dto) =>
        new User
        {
            Id = dto.Id,
            Email = dto.Email!,
            UserName = dto.Username!,
            LastSeen = dto.LastSeen,
            IsOnline = dto.IsOnline,
        };

}