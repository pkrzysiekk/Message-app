namespace Message_Backend.Service;

public interface IBaseService<T,TKey>
{
   public Task Add(T entity);
   public Task Update(T entity);
   public Task Delete(TKey id);
   public Task<T> GetById(TKey id);
}