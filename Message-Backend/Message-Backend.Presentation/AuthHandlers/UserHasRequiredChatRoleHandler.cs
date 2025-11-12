using System.Security.Claims;
using Message_Backend.Application.Helpers;
using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Application.Models.DTOs;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Models.Enums;
using Message_Backend.Presentation.AuthRequirements;
using Message_Backend.Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.Presentation.AuthHandlers;

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

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        UserHasRequiredChatRole requirement)
    {
        var callersId = context.User
            .FindFirstValue(ClaimTypes.NameIdentifier);
        var httpContext = RequestBodyHelper.GetHttpContext(context);

        if (callersId is null || httpContext is null)
        {
            context.Fail();
            return;
        }

        var messageDto = await RequestBodyHelper.ReadBodyAsync<MessageDto>(httpContext);
        if (messageDto is null)
        {
            context.Fail();
            return;
        }

        GroupRole? userRoleInGroup;
        Chat fetchedChat;

        try
        {
            fetchedChat = await _chatService.GetById(messageDto.ChatId);
            userRoleInGroup = await _groupService
                .GetUserRoleInGroup(Int32.Parse(callersId), fetchedChat.GroupId);
        }
        catch
        {
            context.Fail();
            return;
        }

        bool userHasNotRequiredChatRole = userRoleInGroup is null ||
                                          userRoleInGroup < fetchedChat.ForRole;
        bool userIsNotSender = int.Parse(callersId) != messageDto.SenderId;

        if (userHasNotRequiredChatRole || userIsNotSender)
        {
            context.Fail();
            return;
        }
        context.Succeed(requirement);
    }
}