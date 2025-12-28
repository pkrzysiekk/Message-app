namespace Message_Backend.Application.Interfaces.Services;

public interface IMessageAuthorizationService
{
    public Task<bool> CanUserModifyMessage(long messageId, int userId);
}