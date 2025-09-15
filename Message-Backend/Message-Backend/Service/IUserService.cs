using Message_Backend.Models;

namespace Message_Backend.Service;

public interface IUserService
{
   public Task<User> Get(string id);
   public Task Add(User user,string password);
   public Task Update(User user);
   public Task Delete(string id);
   public Task<IEnumerable<User>> SearchForUsers(string term);
   
   
}