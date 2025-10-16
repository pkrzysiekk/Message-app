using Message_Backend.Data;
using Message_Backend.Exceptions;
using Message_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Repository;

public class Repository<T,TKey> :IRepository<T,TKey> where T: class, IEntity<TKey>
{
    private readonly MessageContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(MessageContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>(); 
    }
    public virtual async Task<T> Create(T item)
    {
        await _context.AddAsync(item);
        await SaveChanges();
        return item;
    }        

    public virtual IQueryable<T> GetAll(Func<IQueryable<T>, IQueryable<T>>? include=null)
    {
        IQueryable<T> query = _dbSet;
        if (include != null)
            query = include(query);
        return query;
    }

    public virtual async Task<T?> GetById(int id)
    {
        var item = await _dbSet.FindAsync(id);
        if (item == null)
            throw new NotFoundException("Item not found");
        return item;
    }

    public virtual async Task<T> Update(T item)
    {
        var itemToUpdate = await _dbSet.FindAsync(item.Id);
        if (itemToUpdate == null)
            throw new NotFoundException("Item not found");
        _context.Entry(itemToUpdate).CurrentValues.SetValues(item);
        await SaveChanges();
        return itemToUpdate;
    }

    public virtual async Task Delete(int id)
    {
        var item = await _dbSet.FindAsync(id);
        if (item == null)
            throw new NotFoundException("Item not found");
        _dbSet.Remove(item);
        await SaveChanges();
    }

    public virtual async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}