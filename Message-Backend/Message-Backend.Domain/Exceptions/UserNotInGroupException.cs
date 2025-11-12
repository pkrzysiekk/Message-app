namespace Message_Backend.Domain.Exceptions;

public class UserNotInGroupException :DomainException
{
   public UserNotInGroupException (string message) : base(message){} 
}