using Message_Backend.Models;

namespace Message_Backend.Service;

public interface IChatService
{
   public Task<Chat> Get(int id); 
   public Task<IEnumerable<Chat>> GetAllGroupChats(int groupId);
   public Task<Chat> Create(Chat chat);
   public Task<Chat> Update(Chat chat);
   public Task Delete(int id);
   public Task AddChatToGroup(Chat chat, int groupId);
   public Task RemoveChatFromGroup(int chatId);
   public Task UpdateChat(Chat chat);
}