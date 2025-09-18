using Message_Backend.Data;
using Message_Backend.Exceptions;
using Message_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Message_Backend.Repository;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly MessageContext _context;
    public UserRepository(UserManager<User> userManager, MessageContext context)
    {
        _userManager = userManager;
        _context = context;
        
    }
    
    public async Task Create(User item,string password)
    {
       var result= await _userManager.CreateAsync(item,password);
       if (!result.Succeeded)
           throw new UserManagerException(String.Join("\n",result.Errors.Select(e => e.Description)));
    }

    public IQueryable<User> GetAll()
    {
        return _userManager.Users.AsQueryable()
            .Include(u=>u.Avatar);
    }

    public async Task<User?> GetById(int id)
    {
        return await GetAll().FirstOrDefaultAsync(u=>u.Id == id);
    }

    public async Task Update(User user)
    {
        var userToUpdate = await GetById(user.Id);
        if (userToUpdate == null)
            throw new NotFoundException("User not found");
        var result= await _userManager.SetUserNameAsync(userToUpdate, user.UserName);
        if (!result.Succeeded)
            throw new UserManagerException(String.Join("\n", result.Errors.Select(e => e.Description)));
        
    }

    public async Task Delete(int id)
    {
        var userToDelete = await GetById(id);
        if (userToDelete == null)
            throw new NotFoundException("User not found");
        var result= await _userManager.DeleteAsync(userToDelete);
        if (!result.Succeeded)
            throw new UserManagerException(String.Join("\n", result.Errors.Select(e => e.Description)));

    }

    public async Task ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var userToUpdate = await GetById(userId);
        if (userToUpdate == null)
            throw new NotFoundException("User not found");
        var result=await _userManager.ChangePasswordAsync(userToUpdate, oldPassword, newPassword);
        if (!result.Succeeded)
            throw new UserManagerException(String.Join("\n", result.Errors.Select(e => e.Description)));
    }

    public async Task ChangeEmail(int userId, string email)
    {
       var  userToUpdate = await GetById(userId);
       if (userToUpdate == null)
           throw new NotFoundException("User not found");
       var result = await _userManager.SetEmailAsync(userToUpdate, email);
       if (!result.Succeeded)
           throw new UserManagerException(String.Join("\n", result.Errors.Select(e => e.Description)));
    }

    public async Task SetAvatar(int userId, Avatar avatar)
    {
        var  userToUpdate = await GetById(userId);
        if (userToUpdate == null)
            throw new NotFoundException("User not found");
        
        var addedAvatar= _context.UserAvatar.Add(avatar).Entity; 
        if (userToUpdate.Avatar != null)
            _context.UserAvatar.Remove(userToUpdate.Avatar);
        userToUpdate.Avatar = addedAvatar;
        await _context.SaveChangesAsync();
    }

    public async Task<Avatar?> GetUserAvatar(int userId)
    {
        var user = await GetById(userId);
        if (user == null)
            throw new NotFoundException("User not found");
        return user.Avatar;
        
    }
}