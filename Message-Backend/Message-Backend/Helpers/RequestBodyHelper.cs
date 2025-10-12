using System.Text.Json;
using Message_Backend.Models.DTOs;

namespace Message_Backend.Helpers;

public static class RequestBodyHelper
{
   public static async Task<T?> ReadBodyAsync<T>(HttpContext context)
   {
      context.Request.EnableBuffering();

      context.Request.Body.Position = 0;

      var result = await JsonSerializer.DeserializeAsync<T>(
         context.Request.Body,
         new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
      );

      context.Request.Body.Position = 0;

      return result;
   }
   
   public static async Task<int?> GetValueFromGroupEndpoint(HttpContext context,string propertyName="groupId")
   {
      if (TryGetIntFromQueryOrRoute(context, propertyName, out var groupId))
         return groupId;

      var bodyDtos = new Func<HttpContext, Task<int?>>[]
      {
         async ctx => (await RequestBodyHelper.ReadBodyAsync<UserGroupRoleRequest>(ctx))?.GroupId,
         async ctx => (await RequestBodyHelper.ReadBodyAsync<GroupDto>(ctx))?.GroupId
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
}