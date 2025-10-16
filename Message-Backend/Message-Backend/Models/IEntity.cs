namespace Message_Backend.Models;

public interface IEntity<TKey>
{
   public TKey Id { get; set; } 
}