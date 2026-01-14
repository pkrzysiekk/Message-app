using Message_Backend.Application.Interfaces.Repository;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Application.Services;

public class UserChatService :BaseService<UserChat,int>,IUserChatService
{
    private readonly IMessageService _messageService;
    private readonly IChatService _chatService;
    public UserChatService
        (IRepository<UserChat, int> repository,IMessageService messageService,IChatService chatService) : base(repository)
    {
        _messageService = messageService;
        _chatService = chatService;
    }

    public async Task Create(UserChat userChat)
    {
        await _repository.Create(userChat);
    }
    
    public async Task Update(UserChat userChat)
    {
        var  userChatToUpdate = await _repository.GetById(userChat.Id);
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

    public async Task<int> GetNewMessagesCount(int userId, int chatId)
    {
        var userChatInfo = await GetByUserId(userId, chatId);
        if (userChatInfo.LastMessageId is null || userChatInfo.LastReadAt is null)
            return 0;
        var newMessagesCount = await _messageService
            .GetUserNewChatMessageCount(userId, chatId, userChatInfo.LastReadAt.Value);
        return newMessagesCount;
    }

    public async Task EnsureUserChatsExists(int userId, int groupId)
    {
        var allUserChatsInGroup = await _chatService.GetUserChatsInGroup(userId, groupId);
        var allUserChats = await _repository.GetAll()
            .Where(uc => uc.UserId == userId)
            .Select(uc => uc.ChatId)
            .ToListAsync();
        var missingUserChats = allUserChatsInGroup
            .Where(c=>!allUserChats.Contains(c.Id)).ToList();
        
        foreach (var missingUserChat in missingUserChats)
        {
            var userChatInfo = new UserChat()
            {
                UserId = userId,
                ChatId = missingUserChat.Id,
                LastMessageId = null,
                LastReadAt = null
            };
            try
            {
                await _repository.Create(userChatInfo);
            }
            catch (DbUpdateException)
            {
            }
            
        }
    }

}