using Message_Backend.Models;

namespace Message_Backend.Repository;

public interface IUserRepository
{
    public Task Create(User item,string password);
    public IQueryable<User> GetAll();
    public Task<User?> GetById(int id);
    public Task Update(User item);
    public Task Delete(int id);
}