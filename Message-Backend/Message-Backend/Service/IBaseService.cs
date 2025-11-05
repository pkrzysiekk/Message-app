namespace Message_Backend.Service;

public interface IBaseService<T,TKey>
{
   public Task Delete(TKey id);
   public Task<T> GetById(TKey id);
}