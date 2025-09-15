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
       var result= await _userManager.CreateAsync(item,password);
       if (!result.Succeeded)
           throw new Exception(String.Join("\n",result.Errors.Select(e => e.Description)));
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
        var result= await _userManager.SetUserNameAsync(userToUpdate, user.UserName);
        if (!result.Succeeded)
            throw new Exception(String.Join("\n", result.Errors.Select(e => e.Description)));
        
    }

    public async Task Delete(int id)
    {
        var userToDelete = await GetById(id);
        if (userToDelete == null)
            throw new Exception("User not found");
        var result= await _userManager.DeleteAsync(userToDelete);
        if (!result.Succeeded)
            throw new Exception(String.Join("\n", result.Errors.Select(e => e.Description)));

    }

    public async Task ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var userToUpdate = await GetById(userId);
        if (userToUpdate == null)
            throw new Exception("User not found");
        var result=await _userManager.ChangePasswordAsync(userToUpdate, oldPassword, newPassword);
        if (!result.Succeeded)
        {
            throw new Exception(String.Join("\n", result.Errors.Select(e => e.Description)));
        }
    }


}