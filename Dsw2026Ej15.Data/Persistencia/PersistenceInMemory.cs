using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Domain.Entities;

namespace Dsw2026Ej15.Data.Persistencia
{
    public class PersistenceInMemory : IPersistence
    {
        private readonly List<BaseEntity> _persistence = new List<BaseEntity>();

        public PersistenceInMemory()
        {
            LoadSpecialities();
        }

        public List<T> GetAll<T>() where T : BaseEntity
        {
            return _persistence.OfType<T>().ToList();
        }

        public T GetById<T>(Guid id) where T : BaseEntity
        {
            return _persistence.OfType<T>().FirstOrDefault(e => e.Id == id);
        }

        public void Add<T>(T entity) where T : BaseEntity
        {
            _persistence.Add(entity);
        }

        public void Update<T>(T entity) where T : BaseEntity
        {
            var existingEntity = GetById<T>(entity.Id); 
            if (existingEntity != null)
            {
                _persistence.Remove(existingEntity);
                _persistence.Add(entity);
            }
        }

        public void Delete<T>(Guid id) where T : BaseEntity
        {
            var entity = GetById<T>(id); 
            if (entity != null)
            {
                _persistence.Remove(entity);
            }
        }

   
        private void LoadSpecialities()
        {
            try
            {
                string filePath = "specialities.json";
                if (File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var specialities = JsonSerializer.Deserialize<List<Speciality>>(jsonString, options);

                    if (specialities != null)
                    {
                        foreach (var spec in specialities)
                        {
                            _persistence.Add(spec);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar JSON: {ex.Message}");
            }
        }
    }
} 