using Message_Backend.Models;

namespace Message_Backend.Repository;

public interface IFriendsRepository :IRepository<Friends>
{
   public Task<Friends?> GetById(int userId,int friendId);
   public Task Delete(int userId,int friendId);
   Task<Friends?> IRepository<Friends>.GetById(int id)
   {
      throw new NotImplementedException("Use GetById(userId, friendId) instead.");
   }

   Task IRepository<Friends>.Delete(int id)
   {
      throw new NotImplementedException("Use Delete(userId,friendId) instead.");
   }
}