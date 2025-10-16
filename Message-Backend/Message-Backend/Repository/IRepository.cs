using Message_Backend.Models;

namespace Message_Backend.Repository;

public interface IRepository<T,TKey> where T : IEntity<TKey>
{
   public Task<T> Create(T item);
    public IQueryable<T> GetAll(Func<IQueryable<T>, IQueryable<T>>? include = null);
   public Task<T?> GetById(TKey id);
   public Task<T> Update(T item);
   public Task Delete(TKey id);
   public Task SaveChanges();
}