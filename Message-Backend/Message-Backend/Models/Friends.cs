namespace Message_Backend.Models;

public class Friends
{
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public Enum Status { get; set; }
    public DateTime FriendsSince { get; set; }
    
    public User User { get; set; }
    public User Friend { get; set; }
}