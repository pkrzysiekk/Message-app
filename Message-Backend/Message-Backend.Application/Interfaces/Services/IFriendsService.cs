using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Interfaces.Services;

public interface IFriendsService :IBaseService<Friends,int>
{
    public Task SendInvite(int userId,int friendId);
    public Task Update(Friends friends);
    public Task<Friends> GetFriendsByUserIds(int userId, int friendId);
    public Task AcceptInvite(int recipientId, int senderId);
    public Task DeclineInvite(int recipientId, int senderId);
    public Task RemoveFriend(int userId, int friendId);
 
    public Task<List<Friends>> GetAllUserFriends(int userId);
    public Task<List<Friends>> GetAllUserPendingInvites(int userId);
}