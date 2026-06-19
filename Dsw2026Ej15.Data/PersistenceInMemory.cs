using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Dsw2026Ej15.Domain;

namespace Dsw2026Ej15.Data
{
    public class PersistenceInMemory : IPersistence
    {
        private readonly List<Doctor> _doctors = new();
        private readonly List<Speciality> _specialities;

        public PersistenceInMemory()
        {
            _specialities = LoadSpecialities();
        }

        public IEnumerable<Doctor> GetDoctors() => _doctors.Where(d => d.IsActive);

        public Doctor GetDoctorById(Guid id) => _doctors.FirstOrDefault(d => d.Id == id && d.IsActive);

        public void AddDoctor(Doctor doctor)
        {
            doctor.Id = Guid.NewGuid();
            doctor.IsActive = true;
            _doctors.Add(doctor);
        }

        public void DeactivateDoctor(Guid id)
        {
            var doctor = _doctors.FirstOrDefault(d => d.Id == id && d.IsActive);
            if (doctor != null) doctor.IsActive = false;
        }

        public IEnumerable<Speciality> GetSpecialities() => _specialities;

        public Speciality GetSpecialityById(Guid id) => _specialities.FirstOrDefault(s => s.Id == id);

        private List<Speciality> LoadSpecialities()
        {
            var json = File.ReadAllText("specialities.json");
            return JsonSerializer.Deserialize<List<Speciality>>(json) ?? new List<Speciality>();
        }
    }
}

