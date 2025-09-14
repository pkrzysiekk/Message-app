namespace Message_Backend.Repository;

public interface IRepository<T> where T : class
{
   public Task Create(T item);
   public IQueryable<T> GetAll();
   public Task<T?> GetById(int id);
   public Task Update(T item);
   public Task Delete(int id);
   public Task SaveChanges();
}