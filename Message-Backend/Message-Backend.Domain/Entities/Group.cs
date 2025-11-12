using Message_Backend.Domain.Exceptions;
using Message_Backend.Domain.Interfaces;
using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Domain.Entities;

public class Group : IEntity<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public GroupType Type { get; set; }
     
    public ICollection<UserGroup> UserGroups { get; set; } = [];
    public ICollection<Chat> Chats { get; set; } = [];


    public void AddUser(int userId,GroupRole role)
    {
        bool isAlreadyAdded = UserGroups.Any(ug => ug.UserId == userId);
        if (isAlreadyAdded)
            throw new Exception("User with this group already exists");
       
        var userGroup = new UserGroup()
        {
            UserId = userId,
            GroupId = Id,
            Role = role,
        };
        UserGroups.Add(userGroup); 
    }

    public void RemoveUser(int userId)
    {
        var userGroupToRemove = UserGroups.FirstOrDefault(ug => ug.UserId == userId);
        if (userGroupToRemove == null)
            throw new Exception("User doest not belong to this group");
        UserGroups.Remove(userGroupToRemove);
    }

    public void SetUserRole(int userId, GroupRole role)
    {
        var userIsInGroup = UserGroups.Any(ug => ug.UserId == userId);
        if (!userIsInGroup)
            throw new Exception("User doest not belong to this group");
        var userGroupToUpdate = UserGroups.First(ug => ug.UserId == userId);
        userGroupToUpdate.Role = role;
    }

    public GroupRole? GetUserRole(int userId)
    {
        var user = UserGroups.FirstOrDefault(ug => ug.UserId == userId);
        return user?.Role;
    }
}