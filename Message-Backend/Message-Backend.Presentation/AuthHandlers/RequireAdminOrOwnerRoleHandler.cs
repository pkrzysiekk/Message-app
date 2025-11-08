using System.Security.Claims;
using Message_Backend.Application.Helpers;
using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Models.Enums;
using Message_Backend.Presentation.AuthRequirements;
using Message_Backend.Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Message_Backend.Presentation.AuthHandlers;

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
        var groupId = await RequestBodyHelper.GetGroupIdFromGroupEndpointRequest(httpContext);
        if (groupId == null)
            return;
        var userRole=await _groupService.GetUserRoleInGroup(userId, groupId.Value);
        if (userRole is GroupRole.Admin or GroupRole.Owner)
            context.Succeed(requirement);
    }
   
}