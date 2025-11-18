namespace Message_Backend.Domain.Exceptions;

public class InviteNotValidException : DomainException
{
    public InviteNotValidException(string message) : base(message) {}
}