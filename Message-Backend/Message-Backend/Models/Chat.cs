namespace Message_Backend.Models;

public class Chat
{
    public int Id { get; set; }
    public int GroupId {get; set;}

    public ICollection<Message> Messages { get; set; } = [];
    public Group Group {get; set;}
}