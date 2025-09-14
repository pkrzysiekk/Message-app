using Message_Backend.Data;
using Message_Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Message_Backend.Repository;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task Create(User item,string password)
    {
        await _userManager.CreateAsync(item,password);
    }

    public IQueryable<User> GetAll()
    {
        return _userManager.Users.AsQueryable();
    }

    public async Task<User?> GetById(int id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }

    public async Task Update(User user)
    {
        var userToUpdate = await GetById(user.Id);
        if (userToUpdate == null)
            throw new Exception("User not found");
        userToUpdate.UserName = user.UserName;
        userToUpdate.Email = user.Email;
        await _userManager.UpdateAsync(userToUpdate);
        
    }

    public async Task Delete(int id)
    {
        var userToDelete = await GetById(id);
        if (userToDelete == null)
            throw new Exception("User not found");
        await _userManager.DeleteAsync(userToDelete);

    }

    public Task SaveChanges()
    {
        throw new System.NotImplementedException("You need to implement this function, UserManager from Identity takes" +
                                                 "care of it :)");
    }
}