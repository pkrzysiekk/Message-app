using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Interfaces.Repository;

public interface IFriendsRepository
{
   public Task<Friends> Create(Friends item);
   public Task<Friends?> GetById(int userId,int friendId);
   public Task Delete(int userId,int friendId);
   public Task<Friends> Update(Friends item);
   public IQueryable<Friends> GetAll();
}