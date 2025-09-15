using Message_Backend.Models;
using Message_Backend.Models.DTOs;

namespace Message_Backend.Mappers;

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