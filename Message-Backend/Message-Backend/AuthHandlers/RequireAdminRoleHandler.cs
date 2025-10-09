using System.Security.Claims;
using Message_Backend.AuthRequirements;
using Message_Backend.Models.Enums;
using Message_Backend.Service;
using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.AuthHandlers;

public class RequireAdminRoleHandler :AuthorizationHandler<AdminRoleRequirement>
{
    private readonly IGroupService _groupService;
    public RequireAdminRoleHandler(IGroupService groupService)
    {
        _groupService = groupService;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRoleRequirement requirement)
    {
        var callersId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null)
            context.Fail();

        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return;
        }

        string groupIdFromEndpoint; 
        groupIdFromEndpoint=httpContext.Request.Query["groupId"];
        if (string.IsNullOrEmpty(groupIdFromEndpoint))
            groupIdFromEndpoint=httpContext.GetRouteValue("groupId").ToString();
        if (string.IsNullOrEmpty(groupIdFromEndpoint))
        {
            context.Fail();
            return;
        }
        int userId=int.Parse(groupIdFromEndpoint);
        int groupId=int.Parse(groupIdFromEndpoint);
        var userRole=await _groupService.GetUserRoleInGroup(userId, groupId);
        if (userRole==GroupRole.Admin)
            context.Succeed(requirement);
        
    }
}