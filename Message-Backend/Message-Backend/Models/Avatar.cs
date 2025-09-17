namespace Message_Backend.Models;

public class Avatar
{
    public int Id { get; set; }
    public required byte[] Content { get; set; }
    public int UserId { get; set; }
    
    public User? User { get; set; }
}