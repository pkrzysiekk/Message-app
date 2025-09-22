using Message_Backend.Data;
using Message_Backend.Exceptions;
using Message_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Repository;

public class ChatRepository :IRepository<Chat>
{
    private readonly MessageContext _context;

    public ChatRepository(MessageContext context)
    {
        _context = context;
    } 
    public async Task<Chat> Create(Chat item)
    {
         var added= _context.Chats.Add(item);
         await SaveChanges();
         return added.Entity;
    }

    public IQueryable<Chat> GetAll()
    {
        return _context.Chats.Include(c=>c.Group);
    }

    public async Task<Chat?> GetById(int id)
    {
        return await GetAll().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Chat> Update(Chat item)
    {
        var chatToUpdate = await GetById(item.Id);
        if (chatToUpdate == null)
            throw new NotFoundException("No Chat found with the specified id"); 
        chatToUpdate.Name = item.Name;
        var added=_context.Chats.Update(chatToUpdate);
        await SaveChanges();
        return added.Entity;
    }

    public async Task Delete(int id)
    {
        var chatToDelete = await GetById(id);
        if (chatToDelete == null)
            throw new NotFoundException("No Chat found with the specified id");
        await SaveChanges();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}