using Dsw2026Ej15.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2026Ej15.Domain.Interfaces
{
    public interface IPersistence
    {
        IEnumerable<Doctor> GetDoctors();
        Doctor GetDoctorById(Guid id);
        void AddDoctor(Doctor doctor);
        void DeactivateDoctor(Guid id);

        IEnumerable<Speciality> GetSpecialities();
        Speciality GetSpecialityById(Guid id);
    }
}
