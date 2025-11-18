using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Repository;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Exceptions;
using Message_Backend.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Application.Services;


public class FriendsService :
    BaseService<Friends,int>, IFriendsService
{
    private readonly IUserService _userService;
    
    public FriendsService
        (IRepository<Friends,int> repository, IUserService userService) :base(repository)
    {
        _userService = userService;
    } 
    
    public async Task SendInvite(int userId,int friendId)
    {
        if (await IsFriend(userId, friendId))
            throw new EntityAlreadyExistsException("Already Friends");
        
        var user = await _userService.GetById(userId);
        var friend = await _userService.GetById(friendId);
        var invite = new Friends()
        {
            UserId = userId,
            FriendId = friendId,
            Status = FriendInvitationStatus.Pending
        };
        await _repository.Create(invite);
    }


    public async Task Update(Friends friends)
    {
        await _repository.Update(friends);
    }

    public async Task<List<Friends>> GetAllUserFriends(int userId)
    {
        var friends = await _repository
            .GetAll()
            .Where(f => 
                (f.UserId == userId || f.FriendId == userId)
                && f.Status==FriendInvitationStatus.Accepted)
            .ToListAsync();
        return friends;
    }

    public async Task<List<Friends>> GetAllUserPendingInvites(int userId)
    {
        var invites = await _repository
            .GetAll()
            .Where(f => f.FriendId == userId && f.Status == FriendInvitationStatus.Pending)
            .ToListAsync();
        return invites;
    }

    public async Task<Friends> GetFriendsByUserIds(int userId, int friendId)
    {
        var friends= await _repository
            .GetAll()
            .FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendId == friendId)
                                  ||  (f.FriendId == userId && f.UserId == friendId));
        
       return friends ?? throw new NotFoundException("Friends not found");
    }

    public async Task AcceptInvite(int recipientId, int senderId)
    {
        var invite = await GetValidPendingInvite(recipientId, senderId); 
        invite.SetUserStatus(FriendInvitationStatus.Accepted);
        await _repository.Update(invite);
    }

    public async Task DeclineInvite(int recipientId, int senderId)
    {
        var invite = await GetValidPendingInvite(recipientId, senderId); 
        invite.SetUserStatus(FriendInvitationStatus.Rejected);
        await _repository.Update(invite);
    }

    public async Task RemoveFriend(int userId, int friendId)
    {
        var friend = await GetFriendsByUserIds(userId, friendId);
        await _repository.Delete(friend.Id);
    }

    public async Task<bool> IsFriend(int userId, int friendId)
    {
        bool isFriend= await _repository
            .GetAll()
            .AnyAsync(f => (f.UserId == userId && f.FriendId == friendId)
                      || (f.UserId == friendId && f.FriendId == userId)); 
        return isFriend;
    }
    
    private async Task<Friends> GetValidPendingInvite(int recipientId, int senderId)
    {
        var invite = await GetFriendsByUserIds(recipientId, senderId);

        bool isCorrectReceiver = invite.FriendId == recipientId;
        bool isPending = invite.Status == FriendInvitationStatus.Pending;

        if (!isCorrectReceiver || !isPending)
            throw new Exception("Invite not valid");

        return invite;
    }
}