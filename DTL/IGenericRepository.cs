using System.Linq.Expressions;

namespace EcfDotnet.DTL
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(object id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(object id);
        void Delete(T entity);
        void Save();

        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);
    }
}