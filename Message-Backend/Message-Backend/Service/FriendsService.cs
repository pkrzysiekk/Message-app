using Message_Backend.Exceptions;
using Message_Backend.Models;
using Message_Backend.Models.Enums;
using Message_Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Service;

public class FriendsService :IFriendsService
{
    private readonly IRepository<Friends> _friendsRepository;
    private readonly IUserService _userService;

    public FriendsService(IRepository<Friends> friendsRepository, IUserService userService)
    {
        _friendsRepository = friendsRepository;
        _userService = userService;
    } 
    
    public async Task SendInvite(int userId,int friendId)
    {
        bool alreadyFriends= _friendsRepository
            .GetAll()
            .Any(f => (f.UserId == userId && f.FriendId == friendId)
                      || (f.UserId == friendId && f.FriendId == userId));
        if (alreadyFriends)
            throw new EntityAlreadyExistsException("Already Friends");
        
        var user = await _userService.Get(userId);
        var friend = await _userService.Get(friendId);
        var invite = new Friends()
        {
            UserId = userId,
            FriendId = friendId,
            Status = FriendInvitationStatus.Pending
        };
        await _friendsRepository.Create(invite);
    }

    public async Task Delete(int friendId)
    {
        await _friendsRepository.Delete(friendId);
    }

    public async Task<Friends> FindById(int id)
    {
        var friend = await _friendsRepository.GetById(id);
        return friend ?? throw new NotFoundException("Entity not found");
    }

    public async Task Update(Friends friends)
    {
        await _friendsRepository.Update(friends);
    }

    public async Task<List<Friends>> GetAllUserFriends(int userId)
    {
        var friends = await _friendsRepository
            .GetAll()
            .Where(f => f.UserId == userId || f.FriendId == userId)
            .ToListAsync();
        return friends;
    }
}