using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IRepositoryBase<T> where T : class
    {
        int GetCount();
        TResult Max<TResult>(Expression<Func<T, TResult>> selector);
        Task<int> GetCountAsync();
        bool Any(Expression<Func<T, bool>> condition);
        Task<List<T>> GetAsync(Expression<Func<T, bool>> condition);
        IQueryable<T> Getq(Expression<Func<T, bool>> condition);
        T SoftDelete(T entity, string UserId);
        T Insert(T model);
        T Update(T model);
        void Delete(T model);
        IEnumerable<T> GetAll();
        T GetById(int? id);
        T GetById(Guid? id);
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> Get(Expression<Func<T, bool>> condition);
        IEnumerable<T> Get(Expression<Func<T, bool>> condition, int pageIndex, int pageSize);
        IEnumerable<T> GetAll(int pageIndex = 0, int pageSize = int.MaxValue);
        void InsertAll(IEnumerable<T> model);
        T GetFirst(Func<T, bool> condition);
        void UpdateAll(IEnumerable<T> model);
        void DeleteAll(IEnumerable<T> model);
        //Async Method 
        Task AddRangeAsync(IEnumerable<T> entities);
        Task<T> AddAsync(T entity);
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        Task<T> FindFirstAsync(Expression<Func<T, bool>> match);
        Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match);
        Task<IList<T>> GetAllIncludeAsync(params Expression<Func<T, object>>[] navigationProperties);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> condition);
        IQueryable<T> GetAllQuery();
        Task<T> GetSingleIncludeAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] navigationProperties);
        void Attach(T entity);
    }
}
