
using System.Collections.Generic;
using Dsw2026Ej15.Domain.Entities; 

namespace Dsw2026Ej15.Domain.Interfaces
{
    public interface IPersistence
    {
       
        List<T> GetAll<T>() where T : BaseEntity;

        T GetById<T>(Guid id) where T : BaseEntity;
        
        void Add<T>(T entity) where T : BaseEntity;

        void Update<T>(T entity) where T : BaseEntity;

        void Delete<T>(Guid id) where T : BaseEntity;
    }
}