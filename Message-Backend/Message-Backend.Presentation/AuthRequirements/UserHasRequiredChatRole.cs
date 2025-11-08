using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.Presentation.AuthRequirements;

public class UserHasRequiredChatRole :IAuthorizationRequirement {}