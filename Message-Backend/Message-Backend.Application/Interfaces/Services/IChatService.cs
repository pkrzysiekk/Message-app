using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Interfaces.Services;

public interface IChatService :IBaseService<Chat,int>
{
   public Task<IEnumerable<Chat>> GetAllGroupChats(int groupId);
   public Task<Chat> Create(Chat chat);
   public Task<Chat> Update(Chat chat);
   public Task AddChatToGroup(Chat chat);
   public Task<IEnumerable<Chat>> GetUserChats(int userId);
   public Task<IEnumerable<Chat>> GetUserChatsInGroup(int userId,int groupId);
}