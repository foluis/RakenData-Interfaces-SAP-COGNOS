using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranken.ISC.Contracts.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Commit();

        void Dispose();

        void Delete(object id);

        void Delete(TEntity entity);

        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> GetAllEager(string[] include);

        //IEnumerable<TEntity> QueryObjectGraph(Expression<Func<T, bool>> filter, string children);

        IQueryable<TEntity> GetAll(object filter);

        TEntity GetById(object id);

        TEntity GetFullObject(object id);

        IQueryable<TEntity> GetPaged(int top = 20, int skip = 0, object orderBy = null, object filter = null);

        void Insert(TEntity entity);

        void Update(TEntity entity);
    }
}
