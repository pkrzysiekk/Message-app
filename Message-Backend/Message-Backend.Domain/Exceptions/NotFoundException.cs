namespace Message_Backend.Domain.Exceptions;

public class NotFoundException : DomainException
{
   public NotFoundException(string message) : base(message)
   {
      
   } 
}