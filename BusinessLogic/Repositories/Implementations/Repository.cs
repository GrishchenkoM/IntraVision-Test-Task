using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.Repositories.Interfaces;
using Core.Entities;

namespace BusinessLogic.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : EntityBase
    {
        public Repository(DbDataContext context)
        {
            _context = context;
        }
        public T Get(int id)
        {
            return _context.Set<T>().ToList().FirstOrDefault(x => x.Id == id);
        }

        public List<T> Get()
        {
            return _context.Set<T>().ToList();
        }

        public T Create(T obj)
        {
            _context.Set<T>().Add(obj);
            _context.SaveChanges();

            return _context.Set<T>().Find(obj);
        }

        public T Update(T obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
            _context.SaveChanges();

            return _context.Set<T>().Find(obj.Id);
        }

        public int Delete(T obj)
        {
            if (_context.Set<T>().Find(obj.Id) != null)
                _context.Set<T>().Remove(obj);

            return _context.SaveChanges();
        }

        private readonly DbDataContext _context;
    }
}
