using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Domain.Entities;

public class Friends 
{
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public FriendInvitationStatus Status { get; set; }
    public DateTime? FriendsSince { get; set; }
    
    public User User { get; set; }
    public User Friend { get; set; }
}