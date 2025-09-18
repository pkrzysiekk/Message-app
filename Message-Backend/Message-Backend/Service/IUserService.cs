using Message_Backend.Models;

namespace Message_Backend.Service;

public interface IUserService
{
   public Task<User> Get(int id);
   public Task Add(User user,string password);
   public Task Update(User user);
   public Task Delete(int id);
   public Task<IEnumerable<User>> SearchForUsers(string term);
   public Task ChangePassword(int id, string oldPassword, string newPassword);
   public Task ChangeEmail(int id, string email);
   public Task SetAvatar(int id,IFormFile avatarContent);
   
   
}