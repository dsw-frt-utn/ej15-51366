using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Dsw2026Ej15.Api.Exceptions;
using Dsw2026Ej15.Api.Dtos; 

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
        public IActionResult CreateDoctor([FromBody] DoctorModel.Request request)
        {
            // Solo lanzamos las excepciones si algo está mal. ¡Del resto se encarga el Middleware!
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

            var response = new DoctorModel.Response(
                newDoctor.Name,
                newDoctor.LicenseNumber,
                newDoctor.Speciality.Name
            );

            return CreatedAtAction(nameof(GetDoctorById), new { id = newDoctor.Id }, response);
        }

        [HttpGet]
        public IActionResult GetAllActiveDoctors()
        {
            
            var activeDoctors = _persistence.GetAll<Doctor>().Where(d => d.IsActive);
            var responseList = activeDoctors.Select(doctor => new DoctorModel.Response(
                doctor.Name,
                doctor.LicenseNumber,
                doctor.Speciality?.Name
            )).ToList();

            return Ok(responseList);
        }

     
    
        [HttpGet("{id}")]
        public IActionResult GetDoctorById(Guid id)
        {
            var doctor = _persistence.GetById<Doctor>(id);

            if (doctor == null || !doctor.IsActive)
            {
                return NotFound("El médico solicitado no existe o no se encuentra activo.");
            }

            var response = new DoctorModel.Response(
                doctor.Name,
                doctor.LicenseNumber,
                doctor.Speciality?.Name
            );

            return Ok(response);
        }

       
        [HttpDelete("{id}")]
        public IActionResult DeleteDoctorById(Guid id)
        {
            var existingDoctor = _persistence.GetById<Doctor>(id);

      
            if (existingDoctor == null || !existingDoctor.IsActive)
            {
                return NotFound("El médico solicitado no fue encontrado o ya se encuentra inactivo.");
            }

            existingDoctor.IsActive = false;

            _persistence.Update(existingDoctor);

            return NoContent();
        }
    }
}