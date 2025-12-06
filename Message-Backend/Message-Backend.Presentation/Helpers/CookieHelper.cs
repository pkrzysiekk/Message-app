using System.Security.Claims;
using Message_Backend.Domain.Exceptions;

namespace Message_Backend.Presentation.Helpers;

public static class CookieHelper
{
    public static int GetUserIdFromCookie(ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userId))
            throw new NotFoundException("Id not found");
        return int.Parse(userId);
    }
}