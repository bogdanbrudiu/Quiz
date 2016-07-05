using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizWeb.Data
{

    public interface IRepository<T> where T: class {

        IQueryable<T> Get();
        T Get(Func<T, bool> condition);
        IEnumerable<T> GetList();
        IEnumerable<T> GetList(Func<T, bool> condition);
        T GetByID(object id);
        void Add(T entity);
        void Delete(object id);
        void Delete(T entityToDelete);
        void Update(T entityToUpdate);
        IEnumerable<T> GetWithRawSql(string query, params object[] parameters);
        void Save();

    }
}