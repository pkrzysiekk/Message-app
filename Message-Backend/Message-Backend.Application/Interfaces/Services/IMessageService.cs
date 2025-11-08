using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Interfaces.Services;

public interface IMessageService : IBaseService<Message,long>
{
    public Task<IEnumerable<Message>> GetChatMessages(int chatId,int page, int pageSize);
     public Task Add(Message message);
    public Task Update(long messageId, MessageContent content);
}
