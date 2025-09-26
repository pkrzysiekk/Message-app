using Message_Backend.Models.Enums;

namespace Message_Backend.Models.DTOs;

public class FriendsDto
{
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public FriendInvitationStatus Status { get; set; }
    public DateTime? FriendsSince { get; set; }
}