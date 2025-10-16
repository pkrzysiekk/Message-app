using Message_Backend.Models;

namespace Message_Backend.Repository;

public interface IMessageRepository : IRepository<Message, long>
{
    public Task<MessageContent> AddMessageContent(MessageContent messageContent);
    public Task<MessageContent> UpdateMessageContent(MessageContent messageContent);
}