using Message_Backend.Exceptions;
using Message_Backend.Models;
using Message_Backend.Repository;

namespace Message_Backend.Service;

public class BaseService<T,TKey> :IBaseService<T,TKey> where T : IEntity<TKey>
{
    protected readonly IRepository<T,TKey> _repository;

    public BaseService(IRepository<T,TKey> repository)
    {
       _repository = repository; 
    }

    public virtual async Task Delete(TKey id)
    {
        await _repository.Delete(id);
    }

    public virtual async Task<T> GetById(TKey id)
    {
        var item=await _repository.GetById(id);
        if(item is null)
            throw new NotFoundException("Item not found");
        return item;
    }
}