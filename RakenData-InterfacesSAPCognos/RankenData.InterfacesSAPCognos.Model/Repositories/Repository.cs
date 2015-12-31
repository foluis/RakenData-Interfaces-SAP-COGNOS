using Ranken.ISC.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankenData.InterfacesSAPCognos.Model.Repositories
{

    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal InterfasSAPCognosEntities context;
        internal DbSet<TEntity> dbSet;

        public Repository(InterfasSAPCognosEntities context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual TEntity GetById(object id)
        {

            return dbSet.Find(id);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return dbSet;
        }

        public virtual IQueryable<TEntity> GetAllEager(string[] include)
        {
            switch (include.Count())
            {
                case 1:
                    return dbSet.Include(include[0]);
                case 2:
                    return dbSet.Include(include[0]).Include(include[1]);
                case 3:
                    return dbSet.Include(include[0]).Include(include[1]).Include(include[2]);
                default:
                    return dbSet;
            }
        }

        public IQueryable<TEntity> GetPaged(int top = 20, int skip = 0, object orderBy = null, object filter = null)
        {
            return null;
        }

        public virtual IQueryable<TEntity> GetAll(object filter)
        {
            return null;
        }

        public virtual TEntity GetFullObject(object id)
        {
            return null;
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }

            dbSet.Remove(entity);
        }

        public virtual void Commit()
        {
            context.SaveChanges();
        }

        public virtual void Dispose()
        {
            context.Dispose();
        }

        public virtual void Delete(object id)
        {
            TEntity entity = dbSet.Find(id);
            Delete(entity);
        }
    }
}
