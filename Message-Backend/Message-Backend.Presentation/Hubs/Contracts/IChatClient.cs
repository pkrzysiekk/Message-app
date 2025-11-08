using Message_Backend.Application.Models.DTOs;
using Message_Backend.Application.Models.HubRequests;

namespace Message_Backend.Presentation.Hubs.Contracts;

public interface IChatClient
{
    Task ReceiveMessage(SendMessageRequest request);
    Task SendMessage(MessageDto message);
    Task SendUserIsTypingEvent(int chatId);
    Task ReceiveUserIsTypingEvent(string username);
}