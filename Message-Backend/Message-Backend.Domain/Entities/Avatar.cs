namespace Message_Backend.Domain.Entities;

public class Avatar
{
    public int Id { get; set; }
    public required byte[] Content { get; set; }
    public required string ContentType { get; set; }
    public int UserId { get; set; }
    
    public User? User { get; set; }
}