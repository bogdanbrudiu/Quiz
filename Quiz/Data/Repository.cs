using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data;

namespace QuizWeb.Data
{
    
    public class Repository<T>:IRepository<T> where T: class {

        protected QuizContext ctx;
        protected DbSet<T> dbSet;

        public Repository(QuizContext context)
        {
            ctx = context;
            dbSet = ctx.Set<T>();
        }

        public virtual IQueryable<T> Get() {
            return dbSet;
        }

        public virtual T Get(Func<T, bool> condition) {
            return dbSet.First(condition);
        }

        public virtual IEnumerable<T> GetList() {
            return dbSet.ToList();
        }

        public virtual IEnumerable<T> GetList(Func<T, bool> condition) {
            return dbSet.Where(condition).ToList();
        }

        public virtual T GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            T entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(T entityToDelete)
        {
            if (ctx.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(T entityToUpdate)
        {
            if (ctx.Entry(entityToUpdate).State == EntityState.Detached)
            {
                dbSet.Attach(entityToUpdate);
            }
            ctx.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual IEnumerable<T> GetWithRawSql(string query, params object[] parameters)
        {
            return dbSet.SqlQuery(query, parameters).ToList();
        }

        public void Save()
        {
            ctx.SaveChanges();
        }
    }

}