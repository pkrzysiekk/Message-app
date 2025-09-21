using Message_Backend.Models;
using Message_Backend.Models.DTOs;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Message_Backend.Mappers;

public static class GroupMapper
{
   public static GroupDto ToDto(this Group group)
   {
       return new GroupDto()
       {
           GroupId = group.Id,
           GroupName = group.Name,
           GroupType = group.Type,
           CreatedAt = group.CreatedAt,
           UsersInGroup = group.UserGroups.Select(ug=>ug.UserId).ToList(),
       };
   }

   public static Group ToBo(this GroupDto dto)
   {
       return new Group()
       {
           Id = dto.GroupId,
           Name = dto.GroupName,
           Type = dto.GroupType,
           CreatedAt = dto.CreatedAt,
       };
   }
}