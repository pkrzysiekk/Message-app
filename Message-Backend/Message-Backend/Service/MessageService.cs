using Message_Backend.Exceptions;
using Message_Backend.Models;
using Message_Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Service;

public class MessageService : BaseService<Message,long>,IMessageService
{
    private readonly IMessageRepository _messageRepository;
    
    public MessageService(IMessageRepository repository) : base(repository)
    {
        _messageRepository = repository;
    }

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
        var messageToGet = await _messageRepository.GetAll()
            .Include(m => m.Content)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (messageToGet is null)
            throw new NotFoundException("Message not found");
        return messageToGet;
    }
    
    public async Task Add(Message message,MessageContent content)
    {
        await _messageRepository.AddMessage(message,content);
    }
    
    public async Task AddMessageContent(MessageContent messageContent)
    {
        await _messageRepository.AddMessageContent(messageContent);
    }
    
    public async Task UpdateMessageContent(MessageContent messageContent)
    {
       var updated=await _messageRepository.UpdateMessageContent(messageContent);
    }
    
}