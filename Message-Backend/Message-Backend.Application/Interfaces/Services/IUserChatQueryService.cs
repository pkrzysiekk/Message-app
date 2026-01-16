namespace Message_Backend.Application.Interfaces.Services;

public interface IUserChatQueryService
{
    public Task EnsureUserChatsExists(int userId, int groupId);
}