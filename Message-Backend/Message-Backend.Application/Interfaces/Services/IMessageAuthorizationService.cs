namespace Message_Backend.Application.Interfaces.Services;

public interface IMessageAuthorizationService
{
    public Task<bool> CanUserModifyMessage(long messageId, int userId);
    public Task<bool> IsUserInGroup(int groupId, int userId);
    public Task<bool> IsUserInChat(int chatId, int userId);
    public Task<bool> IsUserOwner(int groupId,int userId);
    public Task<bool> CanDeleteMember(int groupId, int userId, int userIdToRemove);
    public Task<bool> CanSendDeleteChat(int groupId, int userId, int chatId);
    public Task<bool> CanSendRemoveGroup(int groupId, int userId);

}