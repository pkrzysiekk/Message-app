using Message_Backend.Models;
using Message_Backend.Models.DTOs;

namespace Message_Backend.Service;

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