using Message_Backend.Models;

namespace Message_Backend.Service;

public interface IChatService
{
   public Task<Chat> Get(int id); 
   public Task<IEnumerable<Chat>> GetAllGroupChats(int groupId);
   public Task Create(Chat chat);
   public Task Update(Chat chat);
   public Task Delete(int id);
   
}