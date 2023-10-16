using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide;

public interface IGenericRepository<T> where T : class
{
    public Task AddAsync(T entity);
    public Task SaveChangesAsync();
    
  //  public Task<T?> GetByIdAsync(int id);
    // public Task<IEnumerable<T>> GetAllAsync();
    /*public Task<bool> RemoveAsync(int id);*/
    /* public Task<bool> Update(T entity);
     public Task<bool> SaveChanges();
     */
}
