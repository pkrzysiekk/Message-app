using Message_Backend.Models;
using Message_Backend.Models.Enums;

namespace Message_Backend.Service;

public interface IGroupService :IBaseService<Group,int>
{
    public Task CreateGroup(Group group,int creatorId);
    public Task UpdateGroup(Group group);
    public Task<List<Group>> GetPaginatedUserGroups(int userId,int page, int pageSize);
    public Task AddUserToGroup(int userId, int groupId,GroupRole role);
    public Task RemoveUserFromGroup(int userId, int groupId);
    public Task UpdateUserRoleInGroup(int userId, int groupId, GroupRole role);
    public Task<GroupRole?> GetUserRoleInGroup(int userId, int groupId);
}