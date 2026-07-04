using System;
using System.Collections.Generic;
using System.Linq;
using Dsw2026Ej15.Data.Context;
using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;

namespace Dsw2026Ej15.Data.Persistencia
{
    public class PersistenceEf : IPersistence
    {
        private readonly AppDbContext _context;

        public PersistenceEf(AppDbContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : BaseEntity
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public void Delete<T>(Guid id) where T : BaseEntity
        {
            var entity = _context.Set<T>().Find(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                _context.SaveChanges();
            }
        }

        public List<T> GetAll<T>() where T : BaseEntity
        {
            return _context.Set<T>().ToList();
        }

        public T GetById<T>(Guid id) where T : BaseEntity
        {
            return _context.Set<T>().Find(id);
        }

        public void Update<T>(T entity) where T : BaseEntity
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }
    }
}
