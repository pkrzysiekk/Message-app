using System.Security.Claims;
using Message_Backend.AuthRequirements;
using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.AuthHandlers;

public class SameUserHandler :AuthorizationHandler<SameUserRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement)
    {
       var userIdFromToken= context.User.FindFirstValue(ClaimTypes.NameIdentifier); 
       
       if (userIdFromToken == null)
           context.Fail();
       
       if (context.Resource is not HttpContext httpContext)
       {
           context.Fail();
           return Task.CompletedTask; 
       }

       string userIdFromEndpoint;
       userIdFromEndpoint = httpContext.Request.Query["userId"].ToString();
       
       if (string.IsNullOrEmpty(userIdFromEndpoint))
        userIdFromEndpoint = httpContext.GetRouteValue("userId").ToString();
       if (userIdFromEndpoint == userIdFromToken)
        context.Succeed(requirement);
       return Task.CompletedTask;
    }
}