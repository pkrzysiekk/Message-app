using Message_Backend.Data;
using Message_Backend.Exceptions;
using Message_Backend.Models;
using Message_Backend.Models.Enums;
using Message_Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Service;

public class GroupService :BaseService<Group,int>,IGroupService
{
    private readonly IUserService _userService;
    public GroupService
        (IRepository<Group,int> repository, IUserService userService):base(repository)
    {
        _userService = userService;
    }

    public async Task CreateGroup(Group group,int creatorId)
    {
        await _repository.Create(group);
        await AddUserToGroup(creatorId,group.Id,GroupRole.Owner);
    }

    public async Task UpdateGroup(Group group)
    {
       await _repository.Update(group); 
    }
    public async Task<List<Group>> GetPaginatedUserGroups(int userId,int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 1 : pageSize;
        
        var groups = await _repository
            .GetAll(q=>q.Include(g=>g.UserGroups))
            .Where(g=>g.UserGroups
                .Any(ug=>ug.UserId==userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return groups;
    }

    public override async Task<Group> GetById(int id)
    {
        var group = await _repository
            .GetAll(q=>q.Include(g=>g.UserGroups))
            .FirstOrDefaultAsync(g=>g.Id == id);
        return group ?? throw new NotFoundException("Group not found");
    }

    public async Task AddUserToGroup(int userId, int groupId,GroupRole role)
    {
       var groupToAddUserTo = await GetById(groupId);
       var userToAdd = await _userService.GetById(userId);
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
       await _repository.SaveChanges();
    }

    public async Task RemoveUserFromGroup(int userId, int groupId)
    {
        var groupToRemoveUserFrom = await GetById(groupId);
        
        var userGroupToRemove = groupToRemoveUserFrom.UserGroups.
            FirstOrDefault
                (g => g.UserId == userId && g.GroupId == groupId);
        if (userGroupToRemove == null)
            throw new KeyNotFoundException("User does not belong to that group");
        groupToRemoveUserFrom.UserGroups.Remove(userGroupToRemove);
        await _repository.SaveChanges();
    }

    public async Task UpdateUserRoleInGroup(int userId, int groupId, GroupRole role)
    {
      var groupToUpdate = await GetById(groupId);
      
      var userGroupToUpdate = groupToUpdate.
          UserGroups.
          FirstOrDefault(ug => ug.UserId == userId && ug.GroupId == groupId);
      if (userGroupToUpdate == null)
          throw new KeyNotFoundException("User does not belong to this group or group doesn't exist");
      userGroupToUpdate.Role = role;
      await _repository.SaveChanges();
    }

    public async Task<GroupRole?> GetUserRoleInGroup(int userId, int groupId)
    {
        var groupToGet = await GetById(groupId);
        var userGroup = groupToGet.UserGroups
            .FirstOrDefault
                (uc => uc.UserId == userId && groupId == uc.GroupId);
        return userGroup?.Role;
    }
}