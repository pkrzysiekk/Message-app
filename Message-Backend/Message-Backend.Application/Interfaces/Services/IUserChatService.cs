using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Interfaces.Services;

public interface IUserChatService :IBaseService<UserChat,int>
{
   public Task Create(UserChat userChat);
   public Task<UserChat> GetByUserId(int userId, int chatId);
}