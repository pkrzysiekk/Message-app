using Message_Backend.Application.Models.DTOs;
using Message_Backend.Application.Models.HubRequests;

namespace Message_Backend.Presentation.Hubs.Contracts;

public interface IChatClient
{
    Task ReceiveMessage(MessageDto message);
    Task SendMessage(MessageDto message);
    Task RemoveMessage(long messageId);
    Task SendMessageRemovedEvent(long messageId);
    Task SendUserIsTypingEvent(int chatId);
    Task ReceiveUserIsTypingEvent(string username);
}