using Message_Backend.Models;

namespace Message_Backend.Service;

public interface IFriendsService
{
    public Task SendInvite(int userId,int friendId);
    public Task Delete(int userId,int friendId);
    public Task<Friends> FindById(int userId,int friendId);
    public Task Update(Friends friends);
    public Task<List<Friends>> GetAllUserFriends(int userId);
}