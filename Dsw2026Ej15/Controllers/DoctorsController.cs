using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Dsw2026Ej15.Api.Exceptions;
using Dsw2026Ej15.Api.Dtos; // El namespace donde guardaste tu DoctorDto

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

        // =========================================================
        // PRIMER ENDPOINT: POST api/doctors
        // Descripción: Insertar un nuevo médico usando DoctorDto.Request
        // =========================================================
        [HttpPost]
        public IActionResult CreateDoctor([FromBody] DoctorModel.Request request)
        {
            try
            {
                // Validación del punto h: Lanzar ValidationException si fallan las reglas
                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.LicenseNumber))
                {
                    throw new ValidationException("The name and license number are required.");
                }

                if (request.SpecialityId == Guid.Empty)
                {
                    throw new ValidationException("Debe asignar una especialidad válida al médico.");
                }

                // Verificar si la especialidad existe en el sistema (cargada desde el JSON)
                var specialityExists = _persistence.GetById<Speciality>(request.SpecialityId);
                if (specialityExists == null)
                {
                    throw new ValidationException("La especialidad ingresada no existe en el sistema.");
                }

                // Mapear los datos del DTO Request a nuestra Entidad de dominio
                var newDoctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    LicenseNumber = request.LicenseNumber,
                    SpecialityId = request.SpecialityId,
                    Speciality = specialityExists,
                    IsActive = true // Comportamiento: se crea activo
                };

                // Guardar en nuestra persistencia en memoria (Singleton)
                _persistence.Add(newDoctor);

                // Para el CreatedAtAction, lo ideal es retornar el DTO de respuesta que representa al objeto creado
                var response = new DoctorModel.Response(
                    newDoctor.Name,
                    newDoctor.LicenseNumber,
                    newDoctor.Speciality.Name
                );

                // Respuesta exitosa: 201 Created
                return CreatedAtAction(nameof(GetDoctorById), new { id = newDoctor.Id }, response);
            }
            catch (ValidationException ex)
            {
                // Respuesta de error: 400 BadRequest con el mensaje de la excepción (punto g.i)
                return BadRequest(ex.Message);
            }
        }

        // =========================================================
        // SEGUNDO ENDPOINT: GET api/doctors
        // Descripción: Obtener todos los médicos activos mapeados a DTO
        // =========================================================
        [HttpGet]
        public IActionResult GetAllActiveDoctors()
        {
            // 1. Buscamos y filtramos los médicos activos en una sola línea con LINQ
            var activeDoctors = _persistence.GetAll<Doctor>().Where(d => d.IsActive);

            // 2. Transformamos la lista de Entidades a una lista de DoctorModel.Response
            var responseList = activeDoctors.Select(doctor => new DoctorModel.Response(
                doctor.Name,
                doctor.LicenseNumber,
                doctor.Speciality?.Name
            )).ToList();

            // Respuesta exitosa: 200 OK con la colección (incluso si está vacía)
            return Ok(responseList);
        }

        // =========================================================
        // TERCER ENDPOINT: GET api/doctors/{id}
        // Descripción: Obtener un médico activo específico por su Id
        // =========================================================
        [HttpGet("{id}")]
        public IActionResult GetDoctorById(Guid id)
        {
            var doctor = _persistence.GetById<Doctor>(id);

            // Validación estricta: debe existir y estar activo, de lo contrario 404
            if (doctor == null || !doctor.IsActive)
            {
                return NotFound("El médico solicitado no existe o no se encuentra activo.");
            }

            // Mapeamos a nuestro record Response
            var response = new DoctorModel.Response(
                doctor.Name,
                doctor.LicenseNumber,
                doctor.Speciality?.Name
            );

            // Respuesta exitosa: 200 OK con los datos específicos del médico
            return Ok(response);
        }

        // =========================================================
        // CUARTO ENDPOINT: DELETE api/doctors/{id}
        // Descripción: Establecer como inactivo al médico (Baja lógica)
        // =========================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteDoctorById(Guid id)
        {
            var existingDoctor = _persistence.GetById<Doctor>(id);

            // Validación estricta: debe existir y estar activo
            if (existingDoctor == null || !existingDoctor.IsActive)
            {
                return NotFound("El médico solicitado no fue encontrado o ya se encuentra inactivo.");
            }

            // Cambiamos el estado (baja lógica)
            existingDoctor.IsActive = false;

            // Impactamos el cambio usando el método de actualización
            _persistence.Update(existingDoctor);

            // Respuesta exitosa: 204 No Content
            return NoContent();
        }
    }
}