using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Repository;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Exceptions;
using Message_Backend.Domain.Interfaces;

namespace Message_Backend.Application.Services;

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