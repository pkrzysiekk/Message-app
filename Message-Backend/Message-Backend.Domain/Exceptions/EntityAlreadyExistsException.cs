namespace Message_Backend.Domain.Exceptions;

public class EntityAlreadyExistsException :Exception
{
   public EntityAlreadyExistsException(string message) : base(message) {}
}