namespace Message_Backend.Models;

public class UserGroup
{
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public Enum Role { get; set; }
    
    public User User { get; set; }
    public Group Group { get; set; }
}