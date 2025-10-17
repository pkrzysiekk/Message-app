using Message_Backend.Models;

namespace Message_Backend.Service;

public interface IMessageService : IBaseService<Message,long>
{
    public Task<IEnumerable<Message>> GetChatMessages(int chatId,int page, int pageSize);
    public Task Add(Message message, MessageContent content);
}