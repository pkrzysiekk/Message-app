using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.AuthRequirements;

public class CanCreateChatWithProvidedRole : IAuthorizationRequirement {}