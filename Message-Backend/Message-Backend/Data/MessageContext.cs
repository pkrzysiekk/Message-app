using Message_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Data;

public class MessageContext :IdentityUserContext<User,int>
{
   public DbSet<User> AppUsers { get; set; }
   public DbSet<Avatar> UserAvatar { get; set; }
   public DbSet<Message> Messages { get; set; }
   public DbSet<Friends> Friends { get; set; }
   public DbSet<Group> Groups { get; set; }
   public DbSet<Chat> Chats { get; set; }
   public DbSet<UserGroup> UserGroups { get; set; }
   public MessageContext(DbContextOptions<MessageContext> options):base(options){}

   protected override void OnModelCreating(ModelBuilder modelBuilder)
 {  
      base.OnModelCreating(modelBuilder);
      
      modelBuilder.Entity<User>().ToTable("User");
      modelBuilder.Entity<Message>().ToTable("Message");
      modelBuilder.Entity<Friends>().ToTable("Friends");
      modelBuilder.Entity<Group>().ToTable("Group");
      modelBuilder.Entity<Chat>().ToTable("Chat");
      modelBuilder.Entity<UserGroup>().ToTable("UserGroup");
      
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

      modelBuilder.Entity<Avatar>()
         .HasOne(a => a.User)
         .WithOne(u => u.Avatar)
         .HasForeignKey<Avatar>(a => a.UserId);
      
      modelBuilder.Entity<Message>()
         .HasOne(m=>m.Content)
         .WithOne(c=>c.Message)
         .HasForeignKey<MessageContent>(m=>m.MessageId)
         .OnDelete(DeleteBehavior.Cascade);
   }
}