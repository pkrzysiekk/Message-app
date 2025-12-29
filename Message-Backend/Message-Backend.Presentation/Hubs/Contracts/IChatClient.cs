using Message_Backend.Application.Models.DTOs;
using Message_Backend.Application.Models.HubRequests;

namespace Message_Backend.Presentation.Hubs.Contracts;

public interface IChatClient
{
    Task ReceiveMessage(MessageDto message);
    Task ReceiveMessageRemovedEvent(MessageDto message);
    Task ReceiveConnectionStateChanged();
    Task SendUserIsTypingEvent(int chatId);
    Task ReceiveUserIsTypingEvent(string username);
}