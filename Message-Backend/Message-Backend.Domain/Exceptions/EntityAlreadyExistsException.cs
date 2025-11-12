namespace Message_Backend.Domain.Exceptions;

public class EntityAlreadyExistsException :DomainException
{
   public EntityAlreadyExistsException(string message) : base(message) {}
}