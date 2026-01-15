using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Application.Services;

public class MessageAuthorizationService :IMessageAuthorizationService
{
    private readonly IMessageService _messageService;
    private readonly IChatService _chatService;
    private readonly IGroupService _groupService;
    
    public MessageAuthorizationService
        (IMessageService messageService, IChatService chatService, IGroupService groupService)
    {
        _messageService = messageService;
        _chatService = chatService;
        _groupService = groupService;
    }

    public async Task<bool> CanUserModifyMessage(long messageId, int userId)
    {
        var message = await _messageService.GetById(messageId);
        if(message.SenderId==userId)
            return true;
        var userChatsInGroup =
            await _chatService.GetUserChatsInGroup(userId,message.Chat.GroupId);
        var userRole = await _groupService.GetUserRoleInGroup(userId, message.Chat.GroupId);
        
        bool userIsInChat = userChatsInGroup.Any(c=>c.Id == message.ChatId);
        bool userIsAdminOrOwner = userRole is GroupRole.Admin or  GroupRole.Owner;
        return userIsInChat && userIsAdminOrOwner;
    }

    public async Task<bool> IsUserInGroup(int groupId, int userId)
    {
        var userRole = await _groupService.GetUserRoleInGroup(userId, groupId);
        return userRole != null;
    }

    public async Task<bool> IsUserInChat(int chatId, int userId)
    {
       var userChats = await _chatService.GetUserChats(userId); 
       return userChats.Any(c=>c.Id == chatId);
    }

    public async Task<bool> IsUserOwner(int groupId,int userId)
    {
        var userRole = await _groupService.GetUserRoleInGroup(userId, groupId);
        if(userRole == null)
            return false;
        return userRole==GroupRole.Owner;
    }

    public async Task<bool> CanDeleteMember(int groupId, int userId, int userIdToRemove)
    {
        if (userId == userIdToRemove)
            return true;
        var callersRole = await _groupService.GetUserRoleInGroup(userId, groupId);
        var userToDeleteRole = await _groupService.GetUserRoleInGroup(userIdToRemove, groupId);
         
        bool userToDeleteWasRemoved = userToDeleteRole == null;
        bool callerHasElevatedRole = callersRole is GroupRole.Admin or  GroupRole.Owner;
        
        return callerHasElevatedRole && userToDeleteWasRemoved;
    }

    public async Task<bool> CanDeleteChat(int groupId, int userId, int chatId)
    {
        var chat = await _chatService.GetById(chatId);
        var userRole = await _groupService.GetUserRoleInGroup(userId, groupId);
        return userRole >= chat.ForRole && userRole != GroupRole.Member;
    }
}