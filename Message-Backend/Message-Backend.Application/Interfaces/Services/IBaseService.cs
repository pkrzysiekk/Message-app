namespace Message_Backend.Application.Interfaces.Services;

public interface IBaseService<T,TKey>
{
   public Task Delete(TKey id);
   public Task<T> GetById(TKey id);
}