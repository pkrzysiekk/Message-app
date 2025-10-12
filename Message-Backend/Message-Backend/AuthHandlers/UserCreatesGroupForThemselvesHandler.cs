using System.Security.Claims;
using Message_Backend.AuthRequirements;
using Message_Backend.Helpers;
using Message_Backend.Models.DTOs;
using Message_Backend.Service;
using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.AuthHandlers;

public class UserCreatesGroupForThemselvesHandler :AuthorizationHandler<UserCreatesGroupForThemselves>
{

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserCreatesGroupForThemselves requirement)
    {
        var callersId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null ||
            context.Resource is not HttpContext httpContext )
        {
            context.Fail();
            return;
        }

        int userId=int.Parse(callersId);
        var valueIdFromRequest =
            await getUserIdFromGroupRequest(httpContext);
        if (valueIdFromRequest is not int userIdFromRequest)
        { 
            context.Fail();
            return;
        }
        if (userIdFromRequest == userId)
        {
            context.Succeed(requirement);
        }
    }

    private async Task<int?> getUserIdFromGroupRequest(HttpContext context)
    {
        var request = await RequestBodyHelper.ReadBodyAsync<UserGroupRequest>(context);
        if (request == null)
            return null;
        return request.userId;
    }
}