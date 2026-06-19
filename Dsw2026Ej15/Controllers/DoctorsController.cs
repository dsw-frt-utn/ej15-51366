using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Dsw2026Ej15.Api.Exceptions;
using Dsw2026Ej15.Api.Dtos; // Asegúrate de tener este using

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
        public IActionResult CreateDoctor([FromBody] DoctorDto.Request request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.LicenseNumber))
                    throw new ValidationException("The name and license number are required.");

                if (request.SpecialityId == Guid.Empty)
                    throw new ValidationException("Debe asignar una especialidad válida al médico.");

                var specialityExists = _persistence.GetById<Speciality>(request.SpecialityId);
                if (specialityExists == null)
                    throw new ValidationException("La especialidad ingresada no existe en el sistema.");

                var newDoctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    LicenseNumber = request.LicenseNumber,
                    SpecialityId = request.SpecialityId,
                    Speciality = specialityExists,
                    IsActive = true
                };

                _persistence.Add(newDoctor);

                return CreatedAtAction(nameof(GetDoctorById), new { id = newDoctor.Id }, newDoctor);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetDoctorById(Guid id)
        {
            var doctor = _persistence.GetById<Doctor>(id);

            if (doctor == null || !doctor.IsActive)
                return NotFound("El médico solicitado no existe o no se encuentra activo.");

            // Usando el record Response
            var response = new DoctorDto.Response(
                doctor.Name,
                doctor.LicenseNumber,
                doctor.Speciality?.Name
            );

            return Ok(response);
        }

        // ... (El resto de tus métodos GET y DELETE se mantienen iguales)
    }
}