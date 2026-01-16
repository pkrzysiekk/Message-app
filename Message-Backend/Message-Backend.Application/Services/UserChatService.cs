using Message_Backend.Application.Interfaces.Repository;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Application.Services;

public class UserChatService :BaseService<UserChat,int>,IUserChatService
{
    private readonly IMessageService _messageService;
    public UserChatService
        (IRepository<UserChat, int> repository,IMessageService messageService) : base(repository)
    {
        _messageService = messageService;
    }

    public async Task Create(UserChat userChat)
    {
        await _repository.Create(userChat);
    }

    public async Task<UserChat> FindByUserId(int userId, int chatId)
    {
        var userChatInfo =
            _repository.GetAll()
                .FirstOrDefault(uc => uc.UserId == userId && uc.ChatId == chatId);
        if (userChatInfo is null)
            throw new NotFoundException("UserChat info has not been found");
        return userChatInfo;
    }
    
    public async Task Update(UserChat userChat)
    {
        var userChatToUpdate = await FindByUserId(userChat.UserId, userChat.ChatId);
        if (userChatToUpdate == null)
            throw new NotFoundException("Entity not found");
        if (!userChat.LastMessageId.HasValue)
            throw new NotFoundException("Message has to have LastMessageId when updating");
        var message = await _messageService.GetById(userChat.LastMessageId.Value);
        userChatToUpdate.LastMessageId = userChat.LastMessageId;
        userChatToUpdate.LastReadAt=message.SentAt;
        await _repository.Update(userChatToUpdate);
    }

    public async Task<UserChat?> GetByUserId(int userId, int chatId)
    {
        var userChatInfo = await _repository.GetAll(q =>
            q.Include(uc => uc.Chat)
                .Include(uc => uc.Message)
        ).FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ChatId == chatId);
        
        return userChatInfo;
    }

    public async Task<IEnumerable<UserChat>> GetUserChatsInGroup(int userId)
    {
       var userChats = await _repository.GetAll()
           .Where(uc => uc.UserId == userId).ToListAsync(); 
       return userChats;
    }

    public async Task<int> GetNewMessagesCount(int userId, int chatId)
    {
        var userChatInfo = await GetByUserId(userId, chatId);
        if (userChatInfo.LastMessageId is null || userChatInfo.LastReadAt is null)
            return 0; 
        var newMessagesCount = await GetUserNewChatMessageCount(userId, chatId, userChatInfo.LastReadAt.Value);
        return newMessagesCount;
    }

    private async Task<int> GetUserNewChatMessageCount(int userId, int chatId, DateTime lastReadAt)
    {
        var userCount = await _repository.GetAll()
            .Include(uc=>uc.Message)
            .Select(uc => uc.Message)
            .Where(m => m.ChatId == chatId && lastReadAt < m.SentAt)
            .CountAsync();
        return userCount;
    }

  
    }

