using Message_Backend.Exceptions;
using Message_Backend.Models;
using Message_Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Service;

public class ChatService : IChatService
{
    private readonly IRepository<Chat> _repository;
    private readonly IGroupService _groupService;
    public ChatService(IRepository<Chat> repository, IGroupService groupService)
    {
        _repository = repository;
        _groupService = groupService;
    }
    
    public async Task<Chat> Get(int id)
    {
        var chat = await _repository.GetById(id);
        return chat ?? throw new NotFoundException("Chat not found");
    }

    public async Task<IEnumerable<Chat>> GetAllGroupChats(int groupId)
    {
        var groups= await _repository
            .GetAll().Where(x => x.GroupId == groupId).ToArrayAsync();
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

    public async Task Delete(int id)
    {
        await _repository.Delete(id);
    }
    
    public async Task AddChatToGroup(Chat chat, int groupId)
    {
        var group = await _groupService.GetGroup(groupId);
        if (group == null)
            throw new NotFoundException("Group not found");
        chat.GroupId = groupId;
        await Create(chat);
    }

    public async Task RemoveChatFromGroup(int chatId)
    {
        await Delete(chatId); 
    }

    public async Task UpdateChat(Chat chat)
    {
        await Update(chat);
    }
}