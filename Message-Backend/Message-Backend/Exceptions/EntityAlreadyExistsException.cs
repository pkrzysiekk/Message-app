namespace Message_Backend.Exceptions;

public class EntityAlreadyExistsException :Exception
{
   public EntityAlreadyExistsException(string message) : base(message) {}
}