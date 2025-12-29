using System.Text;
using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Repository;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Exceptions;
using Message_Backend.Domain.Models.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Application.Services;

public class MessageService : BaseService<Message,long>,IMessageService
{
    public MessageService(IRepository<Message,long> repository) : base(repository) {}

    public async Task<IEnumerable<Message>> 
        GetChatMessages(int chatId, int pageSize,DateTime? lastSentAt = null)
    {
        pageSize = pageSize < 1 ? 1 : pageSize;
        IEnumerable<Message> chatMessages = [];
        
        if (lastSentAt is null)
        {
             chatMessages = await _repository.GetAll(q => q.Include(m => m.Content))
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(c => c.SentAt)
                .Take(pageSize)
                .ToListAsync();
        }
        else
        {
            
            chatMessages = await _repository.GetAll(q => q.Include(m => m.Content))
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(c => c.SentAt)
                .Where(c => c.SentAt < lastSentAt.Value)
                .Take(pageSize)
                .ToListAsync();
        }

        return chatMessages.Reverse();
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
        if (message.Status is MessageStatus.Deleted)
            throw new EntityAlreadyDeleted("Already deleted");
        const string deletedMessageContent="Message Deleted";
        const string deletedMessageType="text/plain";
        
        message.Status = MessageStatus.Deleted;
        message.Content.Data = Encoding.UTF8.GetBytes(deletedMessageContent);
        message.Type = deletedMessageType;
        
        await _repository.Update(message);
    }
}