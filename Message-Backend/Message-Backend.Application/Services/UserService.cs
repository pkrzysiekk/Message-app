using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Repository;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Application.Models.DTOs;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
       _userRepository = userRepository; 
    }
    
    public async Task<User> GetById(int id)
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

    public async Task<User> GetUserWithAvatar(int userId)
    {
        var user = await _userRepository.GetAll().Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Id == userId);
        return user ?? throw new NotFoundException("User not found");
    }

    public async Task ChangePassword(int id, string oldPassword, string newPassword)
    {
            await _userRepository.ChangePassword(id, oldPassword, newPassword);
    }

    public async Task ChangeEmail(int id, string email)
    {
        await  _userRepository.ChangeEmail(id, email);
    }

    public async Task ChangeOnlineStatus(int id,bool isOnline)
    {
        var user = await GetById(id);
        user.IsOnline = isOnline;
        await Update(user);
    }

    public async Task SetAvatar(int id, IFormFile avatarContent)
    {
        if (avatarContent.Length == 0)
            throw new NotFoundException("Avatar content is empty");
        using var ms = new MemoryStream();
        await avatarContent.CopyToAsync(ms);

        var avatar = new Avatar()
        {
            Content = ms.ToArray(),
            ContentType = avatarContent.ContentType,
        };
        await _userRepository.SetAvatar(id, avatar);
    }

    public async Task<AvatarDto> GetAvatar(int userId)
    {
        var avatar = await _userRepository.GetUserAvatar(userId);
        if (avatar == null)
            throw new NotFoundException("Avatar not found");
        return new AvatarDto()
        {
            Content = avatar.Content,
            ContentType = avatar.ContentType
        };
    }
}