using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.AuthRequirements;

public class UserCreatesGroupForThemselves :IAuthorizationRequirement { }