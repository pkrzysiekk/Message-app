using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Interfaces.Services;

public interface IFriendsService
{
    public Task SendInvite(int userId,int friendId);
    public Task Delete(int userId,int friendId);
    public Task<Friends> FindById(int userId,int friendId);
    public Task Update(Friends friends);
    public Task<List<Friends>> GetAllUserFriends(int userId);
    public Task<List<Friends>> GetAllUserPendingInvites(int userId);
}