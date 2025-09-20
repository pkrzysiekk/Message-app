using Message_Backend.Models;
using Message_Backend.Models.Enums;

namespace Message_Backend.Service;

public interface IGroupService
{
    public Task CreateGroup(Group group);
    public Task UpdateGroup(Group group);
    public Task DeleteGroup(Group group);
    public Task<List<Group>> GetPaginatedUserGroups(int userId,int page, int pageSize);
    public Task<Group> GetGroup(int id);
    public Task AddUserToGroup(int userId, int groupId,GroupRole role);
    public Task RemoveUserFromGroup(int userId, int groupId);
    public Task UpdateUserRoleInGroup(int userId, int groupId, GroupRole role);
}