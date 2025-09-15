using Message_Backend.Models;
using Message_Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
       _userRepository = userRepository; 
    }
    
    public async Task<User> Get(int id)
    {
        var user = await _userRepository.GetById(id);
        return user ?? throw new Exception("User not found");
    }

    public async Task Add(User user, string password)
    {
        try
        {
            await _userRepository.Create(user, password);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task Update(User user)
    {
        try
        {
            await _userRepository.Update(user);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task Delete(int id)
    {
        try
        {
            await _userRepository.Delete(id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<IEnumerable<User>> SearchForUsers(string term)
    {
        const int usersToTake = 8;
        var users = await _userRepository.GetAll()
            .Where(u => u.UserName!
                .StartsWith(term))
            .Take(usersToTake)
            .ToListAsync();
        return users;

    }

    public async Task ChangePassword(int id, string oldPassword, string newPassword)
    {
        try
        {
            await _userRepository.ChangePassword(id, oldPassword, newPassword);
        } 
        catch (Exception ex)
        {
            throw;
        }
    }
}