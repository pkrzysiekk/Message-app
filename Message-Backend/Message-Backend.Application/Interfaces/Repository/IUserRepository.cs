using Message_Backend.Domain.Entities;

namespace Message_Backend.Application.Interfaces.Repository;

public interface IUserRepository
{
    public Task Create(User item,string password);
    public IQueryable<User> GetAll();
    public Task<User?> GetById(int id);
    public Task Update(User item);
    public Task Delete(int id);
    public Task ChangePassword(int id, string oldPassword, string newPassword);
    public Task ChangeEmail(int userId, string email);
    public Task SetAvatar(int userId,Avatar avatar);
    public Task<Avatar?> GetUserAvatar(int userId);
}