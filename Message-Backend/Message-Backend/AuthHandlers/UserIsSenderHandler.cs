using System.Security.Claims;
using Message_Backend.AuthRequirements;
using Message_Backend.Helpers;
using Message_Backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.AuthHandlers;

public class UserIsSenderHandler :AuthorizationHandler<AuthRequirements.UserIsSender>
{
    protected override async Task HandleRequirementAsync
        (AuthorizationHandlerContext context, AuthRequirements.UserIsSender requirement)
    {
        var callersId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null ||
            context.Resource is not HttpContext httpContext )
        {
            context.Fail();
            return;
        }
        var messageDto=await RequestBodyHelper.ReadBodyAsync<MessageDto>(httpContext);
        if (messageDto is null)
        {
            context.Fail();
            return;
        }
        bool isSenderSameAsCaller= messageDto.SenderId == Int16.Parse(callersId);
        if (isSenderSameAsCaller)
            context.Succeed(requirement);
        else
            context.Fail();
    }
}