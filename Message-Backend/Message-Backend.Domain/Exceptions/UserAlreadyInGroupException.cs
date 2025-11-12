namespace Message_Backend.Domain.Exceptions;

public class UserAlreadyInGroupException :DomainException
{
   public UserAlreadyInGroupException(string message) : base(message){} 
}