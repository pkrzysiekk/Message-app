using Message_Backend.Application.Models.DTOs;
using Message_Backend.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Message_Backend.Application.Interfaces.Services;

public interface IUserService : IBaseService<User,int>
{
   public Task Add(User user,string password);
   public Task Update(User user);
   public Task<IEnumerable<User>> SearchForUsers(string term);
   public Task ChangePassword(int id, string oldPassword, string newPassword);
   public Task ChangeEmail(int id, string email);
   public Task ChangeOnlineStatus(int id,bool isOnline);
   public Task SetAvatar(int id,IFormFile avatarContent);
   public Task<AvatarDto> GetAvatar(int userId);
   
}