using Message_Backend.Exceptions;
using Message_Backend.Models;
using Message_Backend.Repository;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
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
        return user ?? throw new NotFoundException("User not found");
    }

    public async Task Add(User user, string password)
    {
            await _userRepository.Create(user, password);
    }

    public async Task Update(User user)
    {
            await _userRepository.Update(user);
    }

    public async Task Delete(int id)
    {
            await _userRepository.Delete(id);
    }

    public async Task<IEnumerable<User>> SearchForUsers(string term)
    {
        const int usersToTake = 8;
        string normalisedSearchTerm = term.ToUpper();
        
        var users = await _userRepository.GetAll()
            .Where(u => u.NormalizedUserName!
                .StartsWith(normalisedSearchTerm))
            .Take(usersToTake)
            .ToListAsync();
        return users;

    }

    public async Task ChangePassword(int id, string oldPassword, string newPassword)
    {
            await _userRepository.ChangePassword(id, oldPassword, newPassword);
    }

    public async Task ChangeEmail(int id, string email)
    {
        await  _userRepository.ChangeEmail(id, email);
    }

    public async Task SetAvatar(int id, IFormFile avatarContent)
    {
        if (avatarContent.Length == 0)
            throw new Exception("Avatar content is empty");
        using var ms = new MemoryStream();
        await avatarContent.CopyToAsync(ms);

        var avatar = new Avatar()
        {
            Content = ms.ToArray(),
            ContentType = avatarContent.ContentType,
        };
        await _userRepository.SetAvatar(id, avatar);
    }
}