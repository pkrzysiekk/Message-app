using System.Security.Claims;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Models.Enums;
using Message_Backend.Presentation.AuthRequirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Message_Backend.Application.Helpers;
using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Application.Models.DTOs;
using Message_Backend.Presentation.Helpers;

namespace Message_Backend.Presentation.AuthHandlers;

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