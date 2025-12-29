namespace Message_Backend.Application.Interfaces.Services;

public interface IMessageAuthorizationService
{
    public Task<bool> CanUserModifyMessage(long messageId, int userId);
    Task<bool> IsUserInGroup(int groupId, int userId);
    Task<bool> IsUserInChat(int chatId, int userId);
}