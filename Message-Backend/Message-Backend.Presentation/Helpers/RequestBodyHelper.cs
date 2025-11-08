using System.Text.Json;
using Message_Backend.Application.Models.DTOs;
using Message_Backend.Presentation.ApiRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Message_Backend.Presentation.Helpers;

public static class RequestBodyHelper
{
   public static async Task<T?> ReadBodyAsync<T>(HttpContext context)
   {
      context.Request.EnableBuffering();

      context.Request.Body.Position = 0;
      T? result;
      try
      {
         result = await JsonSerializer.DeserializeAsync<T>(
            context.Request.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
         );
      }
      catch (JsonException)
      {
         return default;
      }

      context.Request.Body.Position = 0;

      return result;
   }
   
   public static async Task<int?> GetGroupIdFromGroupEndpointRequest(HttpContext context)
   {
      string propertyName = "groupId";
      
      if (TryGetIntFromQueryOrRoute(context, propertyName, out var groupId))
         return groupId;

      var bodyDtos = new Func<HttpContext, Task<int?>>[]
      {
         async ctx => (await ReadBodyAsync<UserGroupRoleRequest>(ctx))?.GroupId,
         async ctx => (await ReadBodyAsync<GroupDto>(ctx))?.GroupId
      };

      foreach (var readDto in bodyDtos)
      {
         var id = await readDto(context);
         if (id.HasValue)
            return id.Value;
      }

      return null;
   }

   private static bool TryGetIntFromQueryOrRoute(HttpContext context, string key, out int value)
   {
      value = 0;

      if (context.Request.Query.TryGetValue(key, out var q) && int.TryParse(q, out value))
         return true;

      if (context.GetRouteValue(key) is string r && int.TryParse(r, out value))
         return true;

      return false;
   }

   public static async Task<int?> GetGroupIdFromChatEndpointRequest(HttpContext context)
   {
      string property="groupId";
      if(TryGetIntFromQueryOrRoute(context,property,out var groupId))
         return groupId;
      
      GroupDto? groupDto=await ReadBodyAsync<GroupDto>(context);
      ChatDto? chatDto;
      if (groupDto == null)
      {
         chatDto = await ReadBodyAsync<ChatDto>(context);
         return chatDto?.GroupId;
      }
      return groupDto?.GroupId;
   }

   public static async Task<int?> GetChatIdFromChatEndpointRequest(HttpContext context)
   {
      string property="chatId";
      int chatId;
      
      if(context.Request.Query.TryGetValue(property,out var q) &&int.TryParse(q,out chatId))
         return chatId;
      if(context.GetRouteValue(property) is string r && int.TryParse(r,out chatId))
         return chatId;
      var chatDto=await ReadBodyAsync<ChatDto>(context);
      if (chatDto != null)
         return chatDto.Id;
      return null;
   }

   public static HttpContext? GetHttpContext(AuthorizationHandlerContext context)
   {
      return context.Resource is not HttpContext httpContext ? null : httpContext;
   }
   
}