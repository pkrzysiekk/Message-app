using Message_Backend.Models.DTOs;
using Message_Backend.Models.HubRequests;

namespace Message_Backend.Hubs.Contracts;

public interface IChatClient
{
    Task ReceiveMessage(SendMessageRequest request);
    Task SendMessage(MessageDto message);
}