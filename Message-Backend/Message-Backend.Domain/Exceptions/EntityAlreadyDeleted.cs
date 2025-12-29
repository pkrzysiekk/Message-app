namespace Message_Backend.Domain.Exceptions;

public class EntityAlreadyDeleted :DomainException 
{
   public EntityAlreadyDeleted (string message) : base(message){} 
}