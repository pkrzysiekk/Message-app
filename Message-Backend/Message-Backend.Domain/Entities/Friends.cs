using Message_Backend.Domain.Interfaces;
using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Domain.Entities;

public class Friends : IEntity<int>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public FriendInvitationStatus Status { get; set; }
    public DateTime? FriendsSince { get; set; }
    
    public User User { get; set; }
    public User Friend { get; set; }

    public void SetUserStatus(FriendInvitationStatus status) => Status = status;
}