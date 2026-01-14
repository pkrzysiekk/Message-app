using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Interfaces.Services;

public interface IMessageService : IBaseService<Message,long>
{
    public Task<IEnumerable<Message>> GetChatMessages(int chatId, int pageSize, DateTime? lastSentAt = null);
    public Task <int> GetUserNewChatMessageCount(int userId, int chatId,DateTime lastReadAt);

     public Task Add(Message message);
    public Task Update(long messageId, MessageContent content);
}
