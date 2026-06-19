namespace Dsw2026Ej15.Api.Dtos
{
    public record DoctorModel
    {
        public record Request(
            string Name,
            string LicenseNumber,
            Guid SpecialityId
        );

        public record Response(
            string Name,
            string LicenseNumber,
            string SpecialityName
        );
    }
}
