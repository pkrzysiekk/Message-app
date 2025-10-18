using Message_Backend.Models;

namespace Message_Backend.Repository;

public interface IMessageRepository : IRepository<Message, long>
{

}