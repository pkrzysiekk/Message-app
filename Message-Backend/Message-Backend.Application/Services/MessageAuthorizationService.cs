using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Domain.Entities;
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

    public async Task<bool> UserHasElevatedRole(int groupId, int userId)
    {
        var userRole = await _groupService.GetUserRoleInGroup(userId, groupId);
        return userRole is  GroupRole.Admin or  GroupRole.Owner;
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

    public async Task<bool> CanSendDeleteChat(int groupId, int userId, int chatId)
    {
        Chat? deletedChat = null;
        try
        {
            deletedChat = await _chatService.GetById(chatId);
        }
        catch (Exception e)
        {
            // ignored
        }

        bool chatDeleted = deletedChat == null;
        bool userHasElevatedRole = await UserHasElevatedRole(groupId, userId);
        return chatDeleted && userHasElevatedRole;
    }

    public async Task<bool> CanSendRemoveGroup(int groupId, int userId)
    {
        var isUserOwner = await IsUserOwner(groupId, userId);
        Group? deletedGroup = null;

        try
        {
            deletedGroup = await _groupService.GetById(groupId);
        }
        catch (Exception e)
        {
            //ignored
        }
        
        bool isGroupDeleted = deletedGroup ==  null;
        return isUserOwner && isGroupDeleted;
    }
}