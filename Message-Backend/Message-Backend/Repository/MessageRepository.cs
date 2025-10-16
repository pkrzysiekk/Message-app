using Message_Backend.Data;
using Message_Backend.Exceptions;
using Message_Backend.Models;

namespace Message_Backend.Repository;

public class MessageRepository :Repository<Message,long>
{
    public MessageRepository(MessageContext context) : base(context) {}

    public async Task<MessageContent> AddMessageContent(MessageContent messageContent)
    {
        var item=await _context.MessageContents.AddAsync(messageContent);
        await SaveChanges();
        return item.Entity;
    }

    public async Task<MessageContent> UpdateMessageContent(MessageContent messageContent)
    {
        var item = await GetById(messageContent.MessageId);
        if (item is null)
            throw new NotFoundException("item not found");
        item.Content = messageContent;
        await SaveChanges();
        return messageContent;
    }
}