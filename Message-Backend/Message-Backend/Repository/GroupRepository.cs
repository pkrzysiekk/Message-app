using Message_Backend.Data;
using Message_Backend.Exceptions;
using Message_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Repository;

public class GroupRepository : IRepository<Group>
{
    private readonly MessageContext _context;
    public GroupRepository(MessageContext context)
    {
        _context = context;
    }
    public async Task Create(Group item)
    {
       _context.Groups.Add(item); 
       await SaveChanges();
    }

    public IQueryable<Group> GetAll()
    {
        return _context.Groups.Include(g=>g.UserGroups);
    }

    public async Task<Group?> GetById(int id)
    {
        return await GetAll().FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task Update(Group item)
    {
        var groupToUpdate= await _context.Groups.FindAsync(item.Id);
        if (groupToUpdate == null)
            throw new NotFoundException("Group not found");
        groupToUpdate.Name = item.Name;
        groupToUpdate.Type = item.Type;
        await SaveChanges();
    }

    public async Task Delete(int id)
    {
        var groupToDelete = await _context.Groups.FindAsync(id);
        if (groupToDelete == null)
            throw new NotFoundException("Group not found");
        _context.Groups.Remove(groupToDelete);
        await SaveChanges();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}