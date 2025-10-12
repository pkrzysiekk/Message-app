using System.Security.Claims;
using Message_Backend.AuthRequirements;
using Message_Backend.Helpers;
using Message_Backend.Models.DTOs;
using Message_Backend.Models.Enums;
using Message_Backend.Service;
using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.AuthHandlers;

public class RequireAdminOrOwnerRoleHandler :AuthorizationHandler<AdminRoleRequirement>
{
    private readonly IGroupService _groupService;
    public RequireAdminOrOwnerRoleHandler(IGroupService groupService)
    {
        _groupService = groupService;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRoleRequirement requirement)
    {
        var callersId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null ||
            context.Resource is not HttpContext httpContext )
        {
            context.Fail();
            return;
        }

        int userId=int.Parse(callersId);
        var groupId = await RequestBodyHelper.GetValueFromGroupEndpoint(httpContext);
        if (groupId == null)
            return;
        var userRole=await _groupService.GetUserRoleInGroup(userId, groupId.Value);
        if (userRole is GroupRole.Admin or GroupRole.Owner)
            context.Succeed(requirement);
    }
   
}