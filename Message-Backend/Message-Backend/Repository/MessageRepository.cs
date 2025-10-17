using Message_Backend.Data;
using Message_Backend.Exceptions;
using Message_Backend.Models;
using Message_Backend.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Repository;

public class MessageRepository :Repository<Message,long>, IMessageRepository
{
    public MessageRepository(MessageContext context) : base(context) {}

    public async Task<Message> AddMessage(Message message, MessageContent content)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await base.Create(message);
            content.MessageId = message.Id;
            await AddMessageContent(content);
            await SaveChanges();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        return message;
        
    }
    public async Task<MessageContent> AddMessageContent(MessageContent messageContent)
    {
        var content=await _context.MessageContents.AddAsync(messageContent);
        return content.Entity;
    }

    public async Task<MessageContent> UpdateMessageContent(MessageContent messageContent)
    {
        var item = await
            GetAll(q => q.Include(mc => mc.Content))
            .FirstOrDefaultAsync(m => m.MessageContentId == messageContent.Id);
        if (item is null)
            throw new NotFoundException("item not found");
        item.Content.Data = messageContent.Data;
        item.Status = MessageStatus.Edited;
        await SaveChanges();
        return messageContent;
    }

}