using Message_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Data;

public class MessageContext :DbContext
{
   public MessageContext(DbContextOptions<MessageContext> options):base(options){}

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<UserGroup>()
         .HasKey(ug => new { ug.UserId, ug.GroupId });
      
      modelBuilder.Entity<UserGroup>()
         .HasOne(ug => ug.User)
         .WithMany(u => u.UserGroups)
         .HasForeignKey(ug => ug.UserId);
      
      modelBuilder.Entity<UserGroup>()
         .HasOne(ug => ug.Group)
         .WithMany(g => g.UserGroups)
         .HasForeignKey(ug => ug.GroupId);
      
      modelBuilder.Entity<Friends>()
         .HasKey(f => new { f.FriendId, f.UserId });
      
      modelBuilder.Entity<Friends>()
         .HasOne(f => f.Friend)
         .WithMany()
         .HasForeignKey(f => f.FriendId);
      
      modelBuilder.Entity<Friends>()
         .HasOne(f => f.User)
         .WithMany(u => u.Friends)
         .HasForeignKey(f => f.UserId);
   }
}