using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Ej15.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IPersistence _persistence;

        public DoctorsController(IPersistence persistence)
        {
            _persistence = persistence;
        }

        [HttpPost]
        public IActionResult CreateDoctor([FromBody] Doctor doctor)
        {
            
            if (string.IsNullOrWhiteSpace(doctor.Name) || string.IsNullOrWhiteSpace(doctor.LicenseNumber))
            {
                return BadRequest("The name and license number are required.");
            }

            if (doctor.Speciality == null || doctor.Speciality.Id == Guid.Empty)
            {
                return BadRequest("Debe asignar una especialidad válida al médico.");
            }

          
            var specialityExists = _persistence.GetById<Speciality>(doctor.Speciality.Id);
            if (specialityExists == null)
            {
                return BadRequest("La especialidad ingresada no existe en el sistema.");
            }

          
            doctor.IsActive = true;

            if (doctor.Id == Guid.Empty)
            {
                doctor.Id = Guid.NewGuid();
            }

            _persistence.Add(doctor);

            return Created();
        }

    }
}
