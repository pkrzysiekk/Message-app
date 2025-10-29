using Message_Backend.Exceptions;
using Message_Backend.Models;
using Message_Backend.Models.Enums;
using Message_Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Service;

public class FriendsService :IFriendsService
{
    private readonly IFriendsRepository _friendsRepository;
    private readonly IUserService _userService;

    public FriendsService(IFriendsRepository friendsRepository, IUserService userService)
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
        
        var user = await _userService.GetById(userId);
        var friend = await _userService.GetById(friendId);
        var invite = new Friends()
        {
            UserId = userId,
            FriendId = friendId,
            Status = FriendInvitationStatus.Pending
        };
        await _friendsRepository.Create(invite);
    }

    public async Task Delete(int userId,int friendId)
    {
        await _friendsRepository.Delete(userId, friendId);
    }

    public async Task<Friends> FindById(int userId,int friendId)
    {
        var friend = await _friendsRepository.GetById(userId, friendId);
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
            .Where(f => 
                (f.UserId == userId || f.FriendId == userId)
                && f.Status==FriendInvitationStatus.Accepted)
            .ToListAsync();
        return friends;
    }

    public async Task<List<Friends>> GetAllUserPendingInvites(int userId)
    {
        var invites = await _friendsRepository
            .GetAll()
            .Where(f => f.FriendId == userId && f.Status == FriendInvitationStatus.Pending)
            .ToListAsync();
        return invites;
    }
}