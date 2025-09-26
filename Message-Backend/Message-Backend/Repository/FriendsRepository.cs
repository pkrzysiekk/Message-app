using Message_Backend.Data;
using Message_Backend.Exceptions;
using Message_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Repository;

public class FriendsRepository :IFriendsRepository
{
    private MessageContext _context;
    
    public FriendsRepository(MessageContext context)
    {
        _context = context;
    }
    public async Task<Friends> Create(Friends item)
    {
       var added= await _context.Friends.AddAsync(item);
       await SaveChanges();
       return added.Entity;
    }

    public IQueryable<Friends> GetAll()
    {
        return _context.Friends;
    }

    public async Task<Friends> Update(Friends item)
    {
        var itemToUpdate= await GetById(item.UserId, item.FriendId);
        if (itemToUpdate == null)
            throw new NotFoundException("Entity not found");
        itemToUpdate.Status = item.Status;
        await SaveChanges(); 
        return itemToUpdate;
    }

    public async Task<Friends?> GetById(int userId,int friendId)
    {
        return await GetAll()
            .Where(x => (x.UserId == userId && x.FriendId == friendId)
            ||  (x.UserId == friendId && x.FriendId == userId))
            .FirstOrDefaultAsync();
    }

    public async Task Delete(int userId,int friendId)
    {
        var friend = await GetById(userId,friendId);
        if (friend == null)
            throw new Exception("Entity not found");
        _context.Friends.Remove(friend);
        await SaveChanges();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}