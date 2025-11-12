using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Repository;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Exceptions;
using Message_Backend.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Application.Services;

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
       groupToAddUserTo.AddUser(userToAdd.Id,role);
       await _repository.SaveChanges();
    }

    public async Task RemoveUserFromGroup(int userId, int groupId)
    {
        var groupToRemoveUserFrom = await GetById(groupId);
        var userToRemove = await _userService.GetById(userId);
        groupToRemoveUserFrom.RemoveUser(userToRemove.Id);
        await _repository.SaveChanges();
    }

    public async Task UpdateUserRoleInGroup(int userId, int groupId, GroupRole role)
    {
      var groupToUpdate = await GetById(groupId);
      groupToUpdate.SetUserRole(userId, role);
      await _repository.SaveChanges();
    }

    public async Task<GroupRole?> GetUserRoleInGroup(int userId, int groupId)
    {
        var groupToGet = await GetById(groupId);
        return groupToGet.GetUserRole(userId);
    }
}