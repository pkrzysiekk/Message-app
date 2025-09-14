using Message_Backend.Data;
using Message_Backend.Models;

namespace Message_Backend.Repository;

public class UserRepository : IRepository<User>
{
    private readonly MessageContext _context;

    public UserRepository(MessageContext context)
    {
       _context = context; 
    }
    
    public async Task Create(User item)
    {
        _context.Users.Add(item);
        await SaveChanges();
    }

    public IQueryable<User> GetAll()
    {
        return _context.Users.AsQueryable();
    }

    public async Task<User?> GetById(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task Update(User user)
    {
        var userToUpdate = await GetById(user.Id);
        if (userToUpdate == null)
            throw new Exception("User not found");
        userToUpdate.Username = user.Username;
        await SaveChanges();
    }

    public async Task Delete(int id)
    {
        var userToDelete = await GetById(id);
        if (userToDelete == null)
            throw new Exception("User not found");
        _context.Users.Remove(userToDelete);
        await SaveChanges();

    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}