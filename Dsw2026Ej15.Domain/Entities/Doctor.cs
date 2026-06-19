using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2026Ej15.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public Speciality Speciality { get; set; }
    }
}
