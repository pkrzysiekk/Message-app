using Message_Backend.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Infrastructure.Data;

public class MessageContext :IdentityUserContext<User,int>
{
   public DbSet<User> AppUsers { get; set; }
   public DbSet<Avatar> UserAvatar { get; set; }
   public DbSet<Message> Messages { get; set; }
   public DbSet<MessageContent> MessageContents { get; set; }
   public DbSet<Friends> Friends { get; set; }
   public DbSet<Group> Groups { get; set; }
   public DbSet<Chat> Chats { get; set; }
   public DbSet<UserGroup> UserGroups { get; set; }
   public DbSet<UserChat>  UserChats { get; set; }
   public MessageContext(DbContextOptions<MessageContext> options):base(options){}

   protected override void OnModelCreating(ModelBuilder modelBuilder)
 {  
      base.OnModelCreating(modelBuilder);
      
      modelBuilder.Entity<User>().ToTable("User");
      modelBuilder.Entity<Message>().ToTable("Message");
      modelBuilder.Entity<MessageContent>().ToTable("MessageContent");
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
         .HasForeignKey<Message>(m=>m.MessageContentId)
         .OnDelete(DeleteBehavior.Cascade);
      
      modelBuilder.Entity<Chat>()
         .HasOne(c=>c.Group)
         .WithMany(g=>g.Chats)
         .HasForeignKey(c=>c.GroupId)
         .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<UserChat>()
         .HasOne(uc=>uc.Chat)
         .WithMany(c=>c.UserChats)
         .HasForeignKey(uc=>uc.ChatId)
         .OnDelete(DeleteBehavior.Cascade);

 }
}