using System.Linq.Expressions;
using EcfDotnet.Context;
using Microsoft.EntityFrameworkCore;

namespace EcfDotnet.DTL
{
    public delegate void BeforeEntitiesSavedHandler<T>(object sender, BeforeEntitiesSavedEventArgs<T> args) where T : class;


    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public event BeforeEntitiesSavedHandler<T>
            OnBeforeSaveEntities; 

        public MyDbContext Context
        {
            get => _context;
            set => throw new NotImplementedException();
        }

        private readonly MyDbContext _context;
        protected readonly IServiceProvider Provider;

        public DbSet<T> DbSet => _context.Set<T>();

        public GenericRepository(IServiceProvider provider)
        {
            Provider = provider;
            _context = Provider.GetRequiredService<MyDbContext>();
        }


        public virtual IEnumerable<T> GetAll(bool asNoTracking = false)
        {
            if (asNoTracking == false)
            {
                return DbSet.ToList();
            }

            return DbSet.AsNoTracking().ToList();

        }

        public IQueryable<T> GetQuery(params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> query = DbSet;

            foreach (var includeExpression in includeExpressions)
            {
                query = query.Include(includeExpression);
            }

            return query;
        }



        public virtual T GetById(object Primarikey)
        {
            return DbSet.Find(Primarikey)!;
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public virtual void Add(T entity)
        {
            DbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Added;
        }


        public virtual void Update(T entity)
        {
            if (entity != null)
            {
                var existingEntity = DbSet.Local.FirstOrDefault(e => e == entity);
                if (existingEntity == null)
                {
                    DbSet.Attach(entity);
                    _context.Entry(entity).State = EntityState.Modified;
                }
            }
        }

        public virtual void Delete(T entityToDelete)
        {
            DbSet.Remove(entityToDelete);
        }

        public virtual void Delete(object id)
        {
            var entityToDelete = DbSet.Find(id);
            if (entityToDelete != null)
            {
                Delete(entityToDelete);
            }
        }

        public virtual async Task SaveAsync()
        {
            var entities = await DbSet.ToListAsync();
            OnBeforeSaveEntities?.Invoke(this, new BeforeEntitiesSavedEventArgs<T>(entities));
            await _context.SaveChangesAsync();
        }

        public virtual void Save()
        {
            var entities = DbSet.ToList();
            OnBeforeSaveEntities?.Invoke(this, new BeforeEntitiesSavedEventArgs<T>(entities));
            _context.SaveChanges();
        }
        public virtual async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = false)
        {
            if (asNoTracking == false)
            {
                return await DbSet.ToListAsync();
            }

            return await DbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(object primaryKey)
        {
            return await DbSet.FindAsync(primaryKey);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.SingleOrDefaultAsync(predicate);
        }


        IEnumerable<T> IGenericRepository<T>.GetAll()
        {
            throw new NotImplementedException();
        }
    }
}