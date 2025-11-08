using Message_Backend.Application.Interfaces.Repository;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Exceptions;
using Message_Backend.Domain.Interfaces;
using Message_Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Infrastructure.Repository;

public class Repository<T,TKey> :IRepository<T,TKey> where T: class, IEntity<TKey>
{
    protected readonly MessageContext _context;
    protected readonly DbSet<T> _dbSet;

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

    public virtual async Task<T?> GetById(TKey id)
    {
        return  await _dbSet.FindAsync(id);
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

    public virtual async Task Delete(TKey id)
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