using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Application.Interfaces.Services;

public interface IGroupService :IBaseService<Group,int>
{
    public Task CreateGroup(Group group,int creatorId);
    public Task UpdateGroup(Group group);
    public Task<List<Group>> GetUserGroups(int userId);
    public Task AddUserToGroup(int userId, int groupId,GroupRole role);
    public Task RemoveUserFromGroup(int userId, int groupId);
    public Task UpdateUserRoleInGroup(int userId, int groupId, GroupRole role);
    public Task<GroupRole?> GetUserRoleInGroup(int userId, int groupId);
    public Task<IEnumerable<User>> GetUsersInGroup(int groupId);
    public Task<bool> GroupHasNewMessages(int groupId, int userId);

}