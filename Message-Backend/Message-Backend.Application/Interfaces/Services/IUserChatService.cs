using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Interfaces.Services;

public interface IUserChatService : IBaseService<UserChat, int>
{
    public Task Create(UserChat userChat);
    public Task<UserChat?> GetByUserId(int userId, int chatId);
    public Task Update(UserChat userChat);
    public Task<IEnumerable<UserChat>> GetUserChatsInGroup(int userId);
    public Task<int> GetNewMessagesCountByChat(int userId, int chatId);


}
