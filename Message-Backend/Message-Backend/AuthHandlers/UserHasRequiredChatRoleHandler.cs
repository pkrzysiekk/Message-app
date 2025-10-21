using System.Security.Claims;
using Message_Backend.AuthRequirements;
using Message_Backend.Helpers;
using Message_Backend.Models.DTOs;
using Message_Backend.Service;
using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.AuthHandlers;

public class UserHasRequiredChatRoleHandler : AuthorizationHandler<UserHasRequiredChatRole>
{
    private readonly IGroupService _groupService;
    private readonly IChatService _chatService;

    public UserHasRequiredChatRoleHandler(IGroupService groupService,
        IChatService chatService)
    {
        _groupService = groupService;
        _chatService = chatService;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserHasRequiredChatRole requirement)
    {
        var callersId = context.User
            .FindFirstValue(ClaimTypes.NameIdentifier);
        var httpContext=RequestBodyHelper.GetHttpContext(context);
        
        if (callersId is null || httpContext is null)
        {
            context.Fail();
            return;
        }
        var messageDto= await RequestBodyHelper.ReadBodyAsync<MessageDto>(httpContext);
        if (messageDto is null)
        {
            context.Fail();
            return;
        }

        var fetchedChat = await _chatService.Get(messageDto.ChatId);
        var userRoleInGroup = await _groupService
            .GetUserRoleInGroup(Int32.Parse(callersId),fetchedChat.GroupId);
        
        if (userRoleInGroup is null || userRoleInGroup < fetchedChat.ForRole)
        {
            context.Fail();
            return;
        }
        context.Succeed(requirement);
    }
}