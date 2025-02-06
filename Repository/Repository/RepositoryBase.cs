using Core.Data.Context;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly AppDbContext _context;
        internal DbSet<T> dbSet;
        protected readonly IDbConnection _connection;

        public RepositoryBase(AppDbContext context) { _context = context; this.dbSet = _context.Set<T>(); _connection = context.Database.GetDbConnection(); }

     
        public TResult Max<TResult>(Expression<Func<T, TResult>> selector)
        {
            return _context.Set<T>().Max(selector);
        }
        public bool Any(Expression<Func<T, bool>> condition)
        {
            return _context.Set<T>().Any(condition);
        }
     
        private string ConvertExpressionToSql(Expression<Func<T, bool>> expression)
        {
            if (expression == null)
            {
                return string.Empty;
            }

            var body = expression.Body as BinaryExpression;
            if (body == null)
            {
                throw new NotSupportedException("Only binary expressions are supported.");
            }

            var left = body.Left as MemberExpression;
            var right = body.Right as ConstantExpression;

            if (left == null || right == null)
            {
                throw new NotSupportedException("Only member and constant expressions are supported.");
            }

            var propertyName = left.Member.Name;
            var value = right.Value;

            return $"{propertyName} = '{value}'";
        }

        public void Delete(T model)
        {
            _context.Set<T>().Remove(model);
        }

        public T SoftDelete(T entity, string contextUser)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(entity)} entity must not be null");
            }

            try
            {
                dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                if (entity.GetType().GetProperty("DeletedOn") != null)
                {
                    entity.GetType().GetProperty("DeletedOn").SetValue(entity, DateTime.Now);
                    _context.Entry(entity).Property("DeletedOn").IsModified = true;
                }
                if (entity.GetType().GetProperty("IsActive") != null)
                {
                    entity.GetType().GetProperty("IsActive").SetValue(entity, false);
                    _context.Entry(entity).Property("IsActive").IsModified = true;
                }
                if (entity.GetType().GetProperty("DeletedBy") != null)
                {
                    entity.GetType().GetProperty("DeletedBy").SetValue(entity, contextUser);
                    _context.Entry(entity).Property("DeletedBy").IsModified = true;
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)}" + ex.Message);
            }
        }
        public IEnumerable<T> Get(Expression<Func<T, bool>> condition)
        {
            return _context.Set<T>().AsNoTracking().Where(condition);
        }
        //public IEnumerable<T> Getdto(Expression<Func<T, bool>> condition)
        //{
        //    return _context.Set<T>().AsNoTracking().Where(condition);
        //}
        public IEnumerable<T> Get(Expression<Func<T, bool>> condition, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0)
                pageIndex = 1;
            return _context.Set<T>().AsNoTracking().Where(condition).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
        public async Task<List<T>> GetAsync(Expression<Func<T, bool>> condition)
        {
            return await _context.Set<T>().AsNoTracking().Where(condition).ToListAsync();
        }
        public IQueryable<T> Getq(Expression<Func<T, bool>> condition)
        {
            return _context.Set<T>().AsNoTracking().Where(condition);
        }


        public T GetFirst(Func<T, bool> condition)
        {
            return _context.Set<T>().AsNoTracking().FirstOrDefault(condition);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking();
        }
        public IQueryable<T> GetAllQuery()
        {
            return _context.Set<T>().AsNoTracking();
        }
        public int GetCount()
        {
            return _context.Set<T>().Count();
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
            {
                throw new ArgumentNullException($"{nameof(AddRangeAsync)} entities must not be null or empty");
            }

            try
            {
                await _context.AddRangeAsync(entities);
                // You can call SaveChangesAsync here if you want to commit the changes immediately, 
                // but typically, saving should be handled by the Unit of Work pattern at a higher level.
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(AddRangeAsync)} failed: {ex.Message}");
            }
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public IEnumerable<T> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _context.Set<T>().AsNoTracking();
            return query.ToList();
        }

     

        public T GetById(int? id)
        {
            return _context.Set<T>().Find(id);
        }

        public T GetById(Guid? id)
        {
            return _context.Set<T>().Find(id);
        }

        public T Insert(T model)
        {
            var result = _context.Set<T>().Add(model);
            return result.Entity;
        }
        public async Task<T> AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                await _context.AddAsync(entity);
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} " + ex.Message);
            }
        }

        public void InsertAll(IEnumerable<T> model)
        {
            _context.Set<T>().AddRange(model);
        }

        public void UpdateAll(IEnumerable<T> model)
        {
            if (model.Count() > 0) { _context.Set<T>().UpdateRange(model); }
            _context.SaveChanges();

            if (model.Count() > 0)
            {
                _context.Set<T>().UpdateRange(model);
            }
        }

        public T Update(T model)
        {
            //_context.Entry<T>(model).State = EntityState.Detached;
            //_context.Set<T>().Remove(model);
            var result = _context.Set<T>().Update(model);
            return result.Entity;
        }


        public void DeleteAll(IEnumerable<T> model)
        {
            if (model.Count() > 0) { _context.Set<T>().RemoveRange(model); }
        }
        //Async Methods
        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }
        public async Task<int> CountAsync(Expression<Func<T, bool>> condition)
        {
            return await _context.Set<T>().Where(condition).CountAsync();
        }
        public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().Where(match).ToListAsync();
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(match);
        }
        public async Task<T> FindFirstAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(match);
        }

        //public async Task<ICollection<T>> GetAllAsync()
        //{
        //    return await _context.Set<T>().ToListAsync();
        //}
        public async Task<IList<T>> GetAllIncludeAsync(params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> dbQuery = _context.Set<T>();

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<T, object>(navigationProperty);
            return await dbQuery.AsNoTracking().ToListAsync();
        }
        public async Task<T> GetByIdAsync(int? id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<T> GetSingleIncludeAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;
            IQueryable<T> dbQuery = _context.Set<T>();

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<T, object>(navigationProperty);

            item = await dbQuery
                .AsNoTracking() //Don't track any changes for the selected item
                .SingleOrDefaultAsync(where); //Apply where clause
            return item;

        }
        public void Attach(T entity)
        {
            // Attach the entity to the context
            _context.Set<T>().Attach(entity);
        }

    }
}
