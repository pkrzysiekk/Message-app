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

public class UserHasRequiredRoleInGroupHandler : AuthorizationHandler<UserHasRequiredRoleInGroup>
{
    private readonly IChatService _chatService;
    private readonly IGroupService _groupService;

    public UserHasRequiredRoleInGroupHandler(IChatService chatService, IGroupService groupService)
    {
        _chatService = chatService;
        _groupService = groupService;
    }

    protected override async Task HandleRequirementAsync
        (AuthorizationHandlerContext context, UserHasRequiredRoleInGroup requirement)
    {
        var callersId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId == null || context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return;
        }

        int userId = int.Parse(callersId);

        bool validationResult = await ValidateUserRole(httpContext, userId);
        if (validationResult)
            context.Succeed(requirement);
        else
            context.Fail();
            
    }

    private async Task<bool> ValidateUserRole(HttpContext httpContext, int userId)
    {
        GroupRole? userRole=null;
        
        var chatId = await RequestBodyHelper.GetChatIdFromChatEndpointRequest(httpContext);
        if (!chatId.HasValue) 
            return false;
        var chat = await _chatService.Get(chatId.Value);
        userRole = await _groupService.GetUserRoleInGroup(userId, chat.GroupId);
        return userRole >= chat.ForRole;
    }

}