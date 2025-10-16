using System.Security.Claims;
using Message_Backend.AuthRequirements;
using Message_Backend.Helpers;
using Message_Backend.Models.DTOs;
using Message_Backend.Models.Enums;
using Message_Backend.Service;
using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.AuthHandlers;

public class CanCreateChatWithProvidedRoleHandler : AuthorizationHandler<CanCreateChatWithProvidedRole>
{
    private readonly IGroupService _groupService;

    public CanCreateChatWithProvidedRoleHandler
        (IGroupService groupService)
    {
        _groupService = groupService;
    }
    
    protected override async Task HandleRequirementAsync
        (AuthorizationHandlerContext context, CanCreateChatWithProvidedRole requirement)
    {
        var callersId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null ||
            context.Resource is not HttpContext httpContext )
        {
            context.Fail();
            return;
        } 
        var groupId=await RequestBodyHelper.GetGroupIdFromChatEndpointRequest(httpContext);
        var chatFromRequest=await RequestBodyHelper.ReadBodyAsync<ChatDto>(httpContext); 
        if (groupId is null || chatFromRequest is null)
        {
            context.Fail();
            return;
        }
        var groupRole = await _groupService.GetUserRoleInGroup(int.Parse(callersId),groupId.Value);
        if (groupRole is null)
        {
            context.Fail();
            return;
        }

        if (chatFromRequest.ForRole > groupRole || groupRole is GroupRole.Member)
        {
            context.Fail();
            return;
        }
        context.Succeed(requirement);
        return;
    }
}