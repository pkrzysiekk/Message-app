using Message_Backend.Models.DTOs;

namespace Message_Backend.Hubs.Contracts;

public interface IChatClient
{
    Task ReceiveMessage(MessageDto message);
}