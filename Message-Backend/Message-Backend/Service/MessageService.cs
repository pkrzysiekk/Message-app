using Message_Backend.Exceptions;
using Message_Backend.Models;
using Message_Backend.Models.Enums;
using Message_Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Service;

public class MessageService : BaseService<Message,long>,IMessageService
{
    public MessageService(IRepository<Message,long> repository) : base(repository) {}

    public async Task<IEnumerable<Message>> GetChatMessages(int chatId, int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 1 : pageSize;
        
        var chatMessages = await _repository.GetAll(q => q.Include(m => m.Content))
            .Where(m => m.ChatId == chatId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return chatMessages;
    }

    public override async Task<Message> GetById(long id)
    {
        var messageToGet = await _repository.GetAll()
            .Include(m => m.Content)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (messageToGet is null)
            throw new NotFoundException("Message not found");
        return messageToGet;
    }
    
    public async Task Add(Message message)
    {
        message.SentAt = DateTime.UtcNow;
        await _repository.Create(message);
    }

    public async Task Update(long messageId, MessageContent content)
    {
        var message = await _repository
            .GetAll(q=>q.Include(m=>m.Content))
            .FirstOrDefaultAsync(m=>m.Id == messageId);
        if (message is null)
            throw new NotFoundException("Message not found");
        message.Content.Data=content.Data;
        await _repository.Update(message);
    }

    public override async Task Delete(long id)
    {
        var message = await GetById(id);
        message.Status = MessageStatus.Deleted;
        await _repository.Update(message);
    }
}