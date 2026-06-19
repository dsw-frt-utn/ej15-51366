using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


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
        public IActionResult CreateDoctor([FromBody] Doctor newDoctor)
        {

            if (string.IsNullOrWhiteSpace(newDoctor.Name) || string.IsNullOrWhiteSpace(newDoctor.LicenseNumber))
            {
                return BadRequest("The name and license number are required.");
            }

            if (newDoctor.Speciality == null || newDoctor.Speciality.Id == Guid.Empty)
            {
                return BadRequest("Debe asignar una especialidad válida al médico.");
            }


            var specialityExists = _persistence.GetById<Speciality>(newDoctor.Speciality.Id);
            if (specialityExists == null)
            {
                return BadRequest("La especialidad ingresada no existe en el sistema.");
            }

            newDoctor.IsActive = true;

            if (newDoctor.Id == Guid.Empty)
            {
                newDoctor.Id = Guid.NewGuid();
            }

            _persistence.Add(newDoctor);

            return CreatedAtAction(nameof(GetDoctorById), new { id = newDoctor.Id }, newDoctor);
        }

        [HttpGet]
        public IActionResult GetAllActiveDoctors()
        {
            // Pedimos todos los médicos a la base de datos y los filtramos en la misma línea
            var activeDoctors = _persistence.GetAll<Doctor>().Where(d => d.IsActive).ToList();

            // Retornamos 200 OK con la lista (llena o vacía)
            return Ok(activeDoctors);
        }

        [HttpGet]
        public IActionResult GetAllDoctors()
        {
            var doctors = _persistence.GetAll<Doctor>();

            var activeDoctors = GetAllActiveDoctors();
            return Ok(doctors);
        }


        [HttpGet("{id}")]
        public IActionResult GetDoctorById(Guid id)
        {
            var doctor = _persistence.GetById<Doctor>(id);

            if (doctor == null)
            {
                return NotFound("No se encontró el médico solicitado.");
            }

            return Ok(doctor);
        }


        [HttpDelete]
        public IActionResult DeleteDoctorById(Guid id)
        {
            var existingDoctor = _persistence.GetById<Doctor>(id);

            if (existingDoctor == null || !existingDoctor.IsActive)
            {
                return NotFound("El médico solicitado no fue encontrado o ya se encuentra inactivo. ");

            }

            existingDoctor.IsActive = false;

            _persistence.Update(existingDoctor);

            return NoContent();
        } 



    }
}
