using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Application.Interfaces.Services;

public interface IFriendsService :IBaseService<Friends,int>
{
    public Task SendInvite(int userId,int friendId);
    public Task Update(Friends friends);
    public Task<Friends> GetFriendsInvitesByUserIds(int userId, int friendId);
    public Task AcceptInvite(int recipientId, int senderId);
    public Task DeclineInvite(int recipientId, int senderId);
    public Task RemoveFriend(int userId, int friendId);
    public Task<FriendInvitationStatus?> GetFriendsStatus(int userId, int friendId);
 
    public Task<List<Friends>> GetAllUserFriends(int userId);
    public Task<List<Friends>> GetAllUserPendingInvites(int userId);
    public Task<IEnumerable<User>> GetUsersFromFriends(int userId);

}