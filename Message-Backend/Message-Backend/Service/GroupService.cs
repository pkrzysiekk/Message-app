using Message_Backend.Data;
using Message_Backend.Exceptions;
using Message_Backend.Models;
using Message_Backend.Models.Enums;
using Message_Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Service;

public class GroupService :IGroupService
{
    private readonly IRepository<Group> _groupRepository;
    private readonly IUserService _userService;
    public GroupService
        (IRepository<Group> groupRepository, IUserService userService)
    {
        _groupRepository = groupRepository;
        _userService = userService;
    }

    public async Task CreateGroup(Group group,int creatorId)
    {
        await _groupRepository.Create(group);
        await AddUserToGroup(creatorId,group.Id,GroupRole.Owner);
    }

    public async Task UpdateGroup(Group group)
    {
        await _groupRepository.Update(group);
    }

    public async Task DeleteGroup(int id)
    {
        await _groupRepository.Delete(id);
    }

    public async Task<List<Group>> GetPaginatedUserGroups(int userId,int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 1 : pageSize;
        
        var groups = await _groupRepository.GetAll()
            .Where(g=>g.UserGroups
                .Any(ug=>ug.UserId==userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return groups;
    }

    public async Task<Group> GetGroup(int id)
    {
        var group = await _groupRepository.GetById(id);
        return group ?? throw new NotFoundException("Group not found");
    }

    public async Task AddUserToGroup(int userId, int groupId,GroupRole role)
    {
       var groupToAddUserTo = await GetGroup(groupId);
       var userToAdd = await _userService.Get(userId);
       bool isAlreadyAdded = groupToAddUserTo.UserGroups.Any(ug => ug.UserId == userId);
       if (isAlreadyAdded)
           throw new KeyNotFoundException("User with this group already exists");
       
       var userGroup = new UserGroup()
       {
           UserId = userId,
           GroupId = groupId,
           Role = role,
       };
       groupToAddUserTo.UserGroups.Add(userGroup);
       await _groupRepository.SaveChanges();
    }

    public async Task RemoveUserFromGroup(int userId, int groupId)
    {
        var groupToRemoveUserFrom = await GetGroup(groupId);
        
        var userGroupToRemove = groupToRemoveUserFrom.UserGroups.
            FirstOrDefault
                (g => g.UserId == userId && g.GroupId == groupId);
        if (userGroupToRemove == null)
            throw new KeyNotFoundException("User does not belong to that group");
        groupToRemoveUserFrom.UserGroups.Remove(userGroupToRemove);
        await _groupRepository.SaveChanges();
    }

    public async Task UpdateUserRoleInGroup(int userId, int groupId, GroupRole role)
    {
      var groupToUpdate = await GetGroup(groupId);
      
      var userGroupToUpdate = groupToUpdate.
          UserGroups.
          FirstOrDefault(ug => ug.UserId == userId && ug.GroupId == groupId);
      if (userGroupToUpdate == null)
          throw new KeyNotFoundException("User does not belong to this group or group doesn't exist");
      userGroupToUpdate.Role = role;
      await _groupRepository.SaveChanges();
    }

   
}