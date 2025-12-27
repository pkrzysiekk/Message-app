using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Repository;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Application.Services;

public class ChatService : BaseService<Chat,int>, IChatService
{
    private readonly IGroupService _groupService;
    private readonly IUserService _userService;
    public ChatService
        (IRepository<Chat,int> repository, IGroupService groupService,IUserService userService):base(repository)
    {
        _groupService = groupService;
        _userService = userService;
    }
    
    public async Task<IEnumerable<Chat>> GetAllGroupChats(int groupId)
    {
        var groups= await _repository
            .GetAll().Where(x => x.GroupId == groupId).ToListAsync();
        return groups;
    }


    public async Task<Chat> Create(Chat chat)
    {
        var addedChat=await _repository.Create(chat);
        return addedChat;
    }

    public async Task<Chat> Update(Chat chat)
    {
        var updatedChat=await _repository.Update(chat);
        return updatedChat;
    }

    
    public async Task AddChatToGroup(Chat chat)
    {
        var group = await _groupService.GetById(chat.GroupId);
        if (group == null)
            throw new NotFoundException("Group not found");
        await Create(chat);
    }

    public async Task<IEnumerable<Chat>> GetUserChats(int userId)
    {
        var user = await _userService.GetById(userId);

        var userChats = await _repository
            .GetAll(q => q.Include(c => c.Group)
                .ThenInclude(g => g.UserGroups))
            .Where(c => c.Group.UserGroups.Any(ug => ug.UserId == userId && ug.Role >= c.ForRole))
            .ToListAsync();
        return userChats;
    }

    public async Task<IEnumerable<Chat>> GetUserChatsInGroup(int userId, int groupId)
    {
        var allUserChats=await GetUserChats(userId);
        return  allUserChats.Where(x=>x.GroupId==groupId).ToList();
    }
}