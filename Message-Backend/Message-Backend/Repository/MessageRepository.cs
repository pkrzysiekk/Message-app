using Message_Backend.Data;
using Message_Backend.Exceptions;
using Message_Backend.Models;
using Message_Backend.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Repository;

public class MessageRepository :Repository<Message,long> 
{
    public MessageRepository(MessageContext context) : base(context) {}
}